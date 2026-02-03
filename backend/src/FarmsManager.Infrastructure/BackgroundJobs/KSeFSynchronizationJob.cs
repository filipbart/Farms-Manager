using FarmsManager.Application.Commands.Sales.Invoices;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Application.Specifications.KSeF;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.XmlModels;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Infrastructure.Helpers.KSeF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmsManager.Infrastructure.BackgroundJobs;

/// <summary>
/// BackgroundService wykonujący synchronizację faktur z KSeF
/// </summary>
public class KSeFSynchronizationJob : BackgroundService, IKSeFSynchronizationJob
{
    private readonly ILogger<KSeFSynchronizationJob> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly KSeFSyncConfiguration _configuration;
    private Timer _timer;

    public KSeFSynchronizationJob(
        ILogger<KSeFSynchronizationJob> logger,
        IServiceProvider serviceProvider,
        IOptions<KSeFSyncConfiguration> configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Sprawdzenie czy synchronizacja jest włączona
        if (!_configuration.Enabled)
        {
            _logger.LogInformation("KSeF synchronization job is disabled in configuration");
            return;
        }

        _logger.LogInformation(
            "KSeF synchronization job started. Interval: {IntervalHours} hours",
            _configuration.IntervalHours);

        // Obliczenie interwału w milisekundach
        var intervalMilliseconds = TimeSpan.FromHours(_configuration.IntervalHours).TotalMilliseconds;

        // Utworzenie timera wykonującego się cyklicznie
        _timer = new Timer(
            async void (_) =>
            {
                try
                {
                    await ExecuteSynchronizationAsync(isManual: false, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "KSeF synchronization job failed");
                }
            },
            null,
            TimeSpan.Zero, // Pierwsze wykonanie od razu po starcie
            TimeSpan.FromMilliseconds(intervalMilliseconds));

        // Utrzymanie serwisu przy życiu
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    /// <summary>
    /// Wykonuje synchronizację faktur z KSeF
    /// </summary>
    public async Task ExecuteSynchronizationAsync(bool isManual = false, CancellationToken cancellationToken = default)
    {
        // Utworzenie scope dla scoped services
        using var scope = _serviceProvider.CreateScope();

        var syncLogRepository = scope.ServiceProvider.GetRequiredService<IKSeFSynchronizationLogRepository>();
        var ksefService = scope.ServiceProvider.GetRequiredService<IKSeFService>();
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IKSeFInvoiceRepository>();
        var xmlParser = scope.ServiceProvider.GetRequiredService<IKSeFInvoiceXmlParser>();
        var dbContext = scope.ServiceProvider.GetRequiredService<FarmsManagerContext>();
        var invoiceAssignmentService = scope.ServiceProvider.GetRequiredService<IInvoiceAssignmentService>();
        var contractorAutoCreationService = scope.ServiceProvider.GetRequiredService<IContractorAutoCreationService>();
        var encryptionService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
        var nbpExchangeRateService = scope.ServiceProvider.GetRequiredService<INbpExchangeRateService>();

        // Add repositories for fetching authoritative entity names
        var gasContractorRepository = scope.ServiceProvider.GetRequiredService<IGasContractorRepository>();
        var expenseContractorRepository = scope.ServiceProvider.GetRequiredService<IExpenseContractorRepository>();
        var slaughterhouseRepository = scope.ServiceProvider.GetRequiredService<ISlaughterhouseRepository>();
        var feedContractorRepository = scope.ServiceProvider.GetRequiredService<IFeedContractorRepository>();

        // Pobierz wszystkie podmioty gospodarcze do dopasowania (wraz z fermami)
        var taxBusinessEntities = await dbContext.Set<TaxBusinessEntity>()
            .Include(t => t.Farms.Where(f => f.DateDeletedUtc == null))
            .Where(t => t.DateDeletedUtc == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Pobierz wszystkie fermy do niezależnego wyszukiwania (wraz z aktywnym cyklem)
        var allFarms = await dbContext.Set<FarmEntity>()
            .Include(f => f.ActiveCycle)
            .Where(f => f.DateDeletedUtc == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var log = new KSeFSynchronizationLogEntity
        {
            IsManual = isManual
        };

        try
        {
            _logger.LogInformation(
                "Starting KSeF synchronization. Manual: {IsManual}",
                isManual);

            // 1. Pobierz wszystkie podmioty gospodarcze z tokenami KSeF
            var entitiesWithTokens = taxBusinessEntities
                .Where(t => !string.IsNullOrWhiteSpace(t.KSeFToken))
                .ToList();

            if (entitiesWithTokens.Count == 0)
            {
                _logger.LogWarning("No TaxBusinessEntity with KSeF token found. Skipping synchronization.");
                log.MarkAsCompleted(0, 0);
                return;
            }

            _logger.LogInformation(
                "Found {Count} TaxBusinessEntity with KSeF tokens",
                entitiesWithTokens.Count);

            var allInvoices = new List<KSeFInvoiceSyncItem>();

            // 2. Pobierz faktury z KSeF dla każdego podmiotu
            foreach (var entity in entitiesWithTokens)
            {
                try
                {
                    _logger.LogInformation(
                        "Fetching invoices for TaxBusinessEntity: {Name} (NIP: {Nip})",
                        entity.Name, entity.Nip);

                    // Odszyfruj token przed użyciem
                    var decryptedToken = encryptionService.Decrypt(entity.KSeFToken);

                    var entityInvoices = await ksefService.GetInvoicesForSyncAsync(
                        decryptedToken,
                        entity.Nip,
                        cancellationToken);

                    _logger.LogInformation(
                        "Found {Count} invoices for {Name}",
                        entityInvoices.Count, entity.Name);

                    allInvoices.AddRange(entityInvoices);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to fetch invoices for TaxBusinessEntity: {Name} (NIP: {Nip})",
                        entity.Name, entity.Nip);
                    log.ErrorsCount++;
                    if (isManual)
                        throw;
                }
            }

            // Deduplikacja faktur po KsefNumber (mogą się powtarzać między podmiotami)
            var invoices = allInvoices
                .Where(inv => !string.IsNullOrWhiteSpace(inv.KsefNumber))
                .DistinctBy(inv => inv.KsefNumber, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var downloadedCount = invoices.Count;

            _logger.LogInformation(
                "Total unique invoices found: {Count} (before deduplication: {BeforeDedup})",
                downloadedCount, allInvoices.Count);

            if (downloadedCount == 0)
            {
                _logger.LogInformation("No invoices found in KSeF");
                log.MarkAsCompleted(0, 0);
                return;
            }

            // 2. Sprawdź które faktury już istnieją w bazie
            var ksefNumbers = invoices.Select(i => i.KsefNumber).ToList();
            var existingSpec = new GetKSeFInvoicesByNumbersSpec(ksefNumbers);
            var existingNumbers = await invoiceRepository.ListAsync(existingSpec, cancellationToken);
            var existingNumbersSet = new HashSet<string>(existingNumbers, StringComparer.OrdinalIgnoreCase);

            // 3. Przefiltruj tylko nowe faktury
            var newInvoices = invoices
                .Where(i => !existingNumbersSet.Contains(i.KsefNumber))
                .ToList();

            _logger.LogInformation(
                "Found {Total} invoices in KSeF, {Existing} already exist, {New} new to sync",
                downloadedCount, existingNumbersSet.Count, newInvoices.Count);

            var savedCount = 0;

            // 4. Dla każdej nowej faktury pobierz XML i zapisz do bazy
            foreach (var invoiceSummary in newInvoices)
            {
                try
                {
                    // Znajdź podmiot gospodarczy, który odpowiada tej fakturze
                    var matchingEntity = entitiesWithTokens.FirstOrDefault(e =>
                        e.Nip == NormalizeNip(invoiceSummary.SellerNip) ||
                        e.Nip == NormalizeNip(invoiceSummary.BuyerNip));

                    if (matchingEntity == null)
                    {
                        _logger.LogWarning(
                            "No matching TaxBusinessEntity found for invoice {KsefNumber}. Using first available entity.",
                            invoiceSummary.KsefNumber);
                        matchingEntity = entitiesWithTokens.First();
                    }

                    // Odszyfruj token przed użyciem
                    var decryptedToken = encryptionService.Decrypt(matchingEntity.KSeFToken);

                    var invoiceXml = await ksefService.GetInvoiceXmlAsync(
                        invoiceSummary.KsefNumber,
                        decryptedToken,
                        matchingEntity.Nip,
                        cancellationToken);

                    // Krok 0: Sprawdź czy istnieją podobne faktury (potencjalne duplikaty)
                    var similarInvoices = await CheckForSimilarInvoicesAsync(invoiceSummary, invoiceRepository, cancellationToken);

                    // Krok 1: Dopasuj podmiot gospodarczy po NIP lub nazwie
                    var (taxBusinessEntityId, fallbackFarmId, fallbackCycleId) =
                        MatchTaxBusinessEntityAndFarm(invoiceSummary, taxBusinessEntities, allFarms, invoiceXml,
                            xmlParser);

                    // Krok 2: Utwórz encję faktury (bez fermy na razie - ferma będzie przypisana przez reguły lub fallback)
                    var invoiceEntity = await CreateInvoiceEntityAsync(invoiceSummary, invoiceXml, xmlParser, taxBusinessEntityId,
                        farmId: null, cycleId: null, nbpExchangeRateService, cancellationToken);

                    // Sprawdź czy faktura wymaga powiązania z inną fakturą
                    if (InvoiceRequiresLinking(invoiceEntity.InvoiceType))
                    {
                        invoiceEntity.MarkAsRequiresLinking();
                    }

                    // Krok 3: PRIORYTET 1 - Automatyczne przypisanie fermy na podstawie reguł (InvoiceFarmAssignmentRuleEntity)
                    var ruleAssignedFarmId =
                        await invoiceAssignmentService.FindFarmForInvoiceAsync(invoiceEntity, cancellationToken);
                    if (ruleAssignedFarmId.HasValue)
                    {
                        var assignedFarm = allFarms.FirstOrDefault(f => f.Id == ruleAssignedFarmId.Value);
                        var assignedCycleId = assignedFarm?.ActiveCycleId;

                        invoiceEntity.Update(farmId: ruleAssignedFarmId.Value, cycleId: assignedCycleId);
                        _logger.LogDebug("Invoice {KsefNumber} assigned to farm {FarmId} by RULE with cycle {CycleId}",
                            invoiceSummary.KsefNumber, ruleAssignedFarmId.Value, assignedCycleId);
                    }
                    // Krok 4: PRIORYTET 2 - Fallback do dopasowania po NIP/nazwie (jeśli reguły nie dopasowały)
                    else if (fallbackFarmId.HasValue)
                    {
                        invoiceEntity.Update(farmId: fallbackFarmId.Value, cycleId: fallbackCycleId);
                        _logger.LogDebug(
                            "Invoice {KsefNumber} assigned to farm {FarmId} by NIP/NAME match with cycle {CycleId}",
                            invoiceSummary.KsefNumber, fallbackFarmId.Value, fallbackCycleId);
                    }

                    // Automatyczne przypisanie pracownika na podstawie reguł
                    var assignedUserId =
                        await invoiceAssignmentService.FindAssignedUserForInvoiceAsync(invoiceEntity,
                            cancellationToken);
                    if (assignedUserId.HasValue)
                    {
                        invoiceEntity.Update(assignedUserId: assignedUserId.Value);
                        _logger.LogDebug("Invoice {KsefNumber} auto-assigned to user {UserId}",
                            invoiceSummary.KsefNumber, assignedUserId.Value);
                    }

                    // Automatyczne przypisanie modułu na podstawie reguł
                    var assignedModule =
                        await invoiceAssignmentService.FindModuleForInvoiceAsync(invoiceEntity, cancellationToken);
                    if (assignedModule.HasValue)
                    {
                        invoiceEntity.Update(moduleType: assignedModule.Value);
                        _logger.LogDebug("Invoice {KsefNumber} auto-assigned to module {ModuleType}",
                            invoiceSummary.KsefNumber, assignedModule.Value);

                        // Automatyczne tworzenie kontrahenta jeśli nie istnieje
                        try
                        {
                            await contractorAutoCreationService.EnsureContractorExistsAsync(
                                invoiceSummary.SellerNip,
                                invoiceSummary.SellerName,
                                null,
                                assignedModule.Value,
                                cancellationToken);

                            // After contractor creation, fetch authoritative names and update invoice
                            await UpdateInvoiceWithAuthoritativeNamesAsync(
                                invoiceEntity,
                                assignedModule.Value,
                                invoiceSummary.SellerNip,
                                invoiceSummary.BuyerNip,
                                gasContractorRepository,
                                expenseContractorRepository,
                                slaughterhouseRepository,
                                feedContractorRepository,
                                cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to auto-create contractor for invoice {KsefNumber}",
                                invoiceSummary.KsefNumber);
                        }
                    }

                    await invoiceRepository.AddAsync(invoiceEntity, cancellationToken);
                    savedCount++;

                    _logger.LogDebug("Saved invoice {KsefNumber}", invoiceSummary.KsefNumber);
                }
                catch (Exception ex)
                {
                    log.ErrorsCount++;
                    _logger.LogWarning(ex, "Failed to save invoice {KsefNumber}", invoiceSummary.KsefNumber);
                    if (isManual)
                        throw;
                }
            }

            await invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            log.MarkAsCompleted(downloadedCount, savedCount);

            _logger.LogInformation(
                "KSeF synchronization completed successfully. Downloaded: {Downloaded}, Saved: {Saved}, Duration: {Duration}s",
                downloadedCount,
                savedCount,
                log.DurationSeconds);
        }
        catch (Exception ex)
        {
            log.ErrorsCount++;
            log.MarkAsFailed(ex.Message);

            _logger.LogError(ex,
                "KSeF synchronization failed. Error: {ErrorMessage}",
                ex.Message);
            if (isManual)
                throw;
        }
        finally
        {
            // Zapisz log do bazy
            await syncLogRepository.AddAsync(log, cancellationToken);
            await syncLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Dopasowuje podmiot gospodarczy, fermę i cykl do faktury na podstawie NIP lub nazwy
    /// Logika przypisywania fermy:
    /// 1. Jeśli podmiot ma przypisane fermy - użyj pierwszej
    /// 2. Jeśli podmiot nie ma ferm - szukaj fermy po NIP
    /// 3. Jeśli nie znaleziono po NIP - szukaj fermy po nazwie (uwzględniając stopkę faktury)
    /// Dodatkowo: przypisuje aktywny cykl z dopasowanej fermy
    /// Stopka faktury może zawierać informacje o miejscu rozładunku, np. "Miejsce rozładunku: Jaworowo Kłódź K5"
    /// </summary>
    private static (Guid? TaxBusinessEntityId, Guid? FarmId, Guid? CycleId) MatchTaxBusinessEntityAndFarm(
        KSeFInvoiceSyncItem invoiceItem,
        List<TaxBusinessEntity> taxBusinessEntities,
        List<FarmEntity> allFarms,
        string invoiceXml,
        IKSeFInvoiceXmlParser xmlParser)
    {
        // Normalizuj NIP-y z faktury
        var sellerNip = NormalizeNip(invoiceItem.SellerNip);
        var buyerNip = NormalizeNip(invoiceItem.BuyerNip);
        var sellerName = invoiceItem.SellerName?.ToLowerInvariant();
        var buyerName = invoiceItem.BuyerName?.ToLowerInvariant();

        // Wyciągnij stopkę i dodatkowe opisy z XML faktury - mogą zawierać informacje o miejscu rozładunku/kurniku
        string additionalText = null;
        if (!string.IsNullOrWhiteSpace(invoiceXml))
        {
            var parsedXml = xmlParser.ParseInvoiceXml(invoiceXml);
            var parts = new List<string>();

            // Stopka faktury (np. "Miejsce rozładunku: Jaworowo Kłódź K5")
            if (!string.IsNullOrWhiteSpace(parsedXml?.Stopka?.Informacje?.StopkaFaktury))
                parts.Add(parsedXml.Stopka.Informacje.StopkaFaktury);

            // Dodatkowe opisy (DodatkowyOpis - klucz/wartość, zgodne z FA(4))
            if (parsedXml?.Fa?.DodatkoweOpisy != null)
            {
                foreach (var opis in parsedXml.Fa.DodatkoweOpisy)
                {
                    if (!string.IsNullOrWhiteSpace(opis.Klucz))
                        parts.Add(opis.Klucz);
                    if (!string.IsNullOrWhiteSpace(opis.Wartosc))
                        parts.Add(opis.Wartosc);
                }
            }

            if (parts.Count > 0)
                additionalText = string.Join(" ", parts).ToLowerInvariant();
        }

        Guid? taxBusinessEntityId = null;
        Guid? farmId = null;

        // 1. Szukaj podmiotu po NIP
        var matchedEntity = taxBusinessEntities.FirstOrDefault(t =>
            t.Nip == sellerNip || t.Nip == buyerNip);

        // 2. Jeśli nie znaleziono po NIP, szukaj po nazwie
        if (matchedEntity == null)
        {
            var entitiesMatchedByName = taxBusinessEntities.Where(t =>
            {
                var entityName = t.Name?.ToLowerInvariant();
                if (string.IsNullOrEmpty(entityName))
                    return false;

                return (!string.IsNullOrEmpty(sellerName) && sellerName.Contains(entityName)) ||
                       (!string.IsNullOrEmpty(buyerName) && buyerName.Contains(entityName)) ||
                       (!string.IsNullOrEmpty(sellerName) && entityName.Contains(sellerName)) ||
                       (!string.IsNullOrEmpty(buyerName) && entityName.Contains(buyerName));
            }).ToList();

            if (entitiesMatchedByName.Count == 1)
            {
                matchedEntity = entitiesMatchedByName.First();
            }
            else if (entitiesMatchedByName.Count > 1)
            {
                // 2a. Szukaj po typie podmiotu w nazwie sprzedawcy/nabywcy
                matchedEntity = entitiesMatchedByName.FirstOrDefault(t =>
                {
                    var businessType = t.BusinessType?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(businessType))
                        return false;

                    return (!string.IsNullOrEmpty(sellerName) && sellerName.Contains(businessType)) ||
                           (!string.IsNullOrEmpty(buyerName) && buyerName.Contains(businessType));
                });

                // 2b. Jeśli nie znaleziono po typie, wybierz podmiot z typem "Faktura prywatna" lub "Nieruchomości"
                if (matchedEntity == null)
                {
                    matchedEntity = entitiesMatchedByName.FirstOrDefault(t =>
                    {
                        var businessType = t.BusinessType?.ToLowerInvariant();
                        return businessType is "faktura prywatna" or "nieruchomości";
                    });
                }
            }
        }

        if (matchedEntity != null)
        {
            taxBusinessEntityId = matchedEntity.Id;

            // Sprawdź czy podmiot ma przypisane fermy - tylko gdy ma dokładnie 1 fermę
            if (matchedEntity.Farms.Count == 1)
            {
                var entityFarm = matchedEntity.Farms.First();
                farmId = entityFarm.Id;
                // Znajdź fermę w allFarms aby uzyskać aktywny cykl
                var farmWithCycle = allFarms.FirstOrDefault(f => f.Id == farmId);
                var cycleId = farmWithCycle?.ActiveCycleId;
                return (taxBusinessEntityId, farmId, cycleId);
            }
            // Gdy podmiot ma więcej niż 1 fermę - nie przypisuj fermy automatycznie
            else if (matchedEntity.Farms.Count > 1)
            {
                return (taxBusinessEntityId, null, null);
            }
        }

        // Jeśli podmiot nie ma ferm lub nie znaleziono podmiotu - szukaj fermy niezależnie
        // 3. Szukaj fermy po NIP
        var farmsMatchedByNip = allFarms.Where(f =>
        {
            var farmNip = NormalizeNip(f.Nip);
            return farmNip == sellerNip || farmNip == buyerNip;
        }).ToList();

        FarmEntity matchedFarm = null;

        if (farmsMatchedByNip.Count == 1)
        {
            // Tylko jedna ferma z tym NIP - użyj jej
            matchedFarm = farmsMatchedByNip.First();
        }
        else if (farmsMatchedByNip.Count > 1)
        {
            // Wiele ferm z tym samym NIP - doszukaj po nazwie (w danych faktury i stopce)
            matchedFarm = farmsMatchedByNip.FirstOrDefault(f =>
            {
                var farmName = f.Name?.ToLowerInvariant();
                if (string.IsNullOrEmpty(farmName))
                    return false;

                // Sprawdź w nazwie sprzedawcy/nabywcy
                if ((!string.IsNullOrEmpty(sellerName) && sellerName.Contains(farmName)) ||
                    (!string.IsNullOrEmpty(buyerName) && buyerName.Contains(farmName)) ||
                    (!string.IsNullOrEmpty(sellerName) && farmName.Contains(sellerName)) ||
                    (!string.IsNullOrEmpty(buyerName) && farmName.Contains(buyerName)))
                {
                    return true;
                }

                // Sprawdź w stopce i dodatkowych opisach faktury (np. "Miejsce rozładunku: Jaworowo Kłódź K5")
                if (!string.IsNullOrEmpty(additionalText) && additionalText.Contains(farmName))
                {
                    return true;
                }

                return false;
            });
            // Nie przypisuj fermy jeśli nie udało się jednoznacznie dopasować po nazwie
        }

        // 4. Jeśli nie znaleziono po NIP, szukaj po nazwie (w danych faktury, stopce i dodatkowych opisach)
        if (matchedFarm == null)
        {
            matchedFarm = allFarms.FirstOrDefault(f =>
            {
                var farmName = f.Name?.ToLowerInvariant();
                if (string.IsNullOrEmpty(farmName))
                    return false;

                // Sprawdź w nazwie sprzedawcy/nabywcy
                if ((!string.IsNullOrEmpty(sellerName) && sellerName.Contains(farmName)) ||
                    (!string.IsNullOrEmpty(buyerName) && buyerName.Contains(farmName)) ||
                    (!string.IsNullOrEmpty(sellerName) && farmName.Contains(sellerName)) ||
                    (!string.IsNullOrEmpty(buyerName) && farmName.Contains(buyerName)))
                {
                    return true;
                }

                // Sprawdź w stopce i dodatkowych opisach faktury (np. "Miejsce rozładunku: Jaworowo Kłódź K5")
                if (!string.IsNullOrEmpty(additionalText) && additionalText.Contains(farmName))
                {
                    return true;
                }

                return false;
            });
        }

        if (matchedFarm != null)
        {
            farmId = matchedFarm.Id;
            return (taxBusinessEntityId, farmId, matchedFarm.ActiveCycleId);
        }

        return (taxBusinessEntityId, farmId, null);
    }

    /// <summary>
    /// Normalizuje NIP usuwając prefiks kraju, myślniki i spacje
    /// </summary>
    private static string NormalizeNip(string nip)
    {
        return nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    }

    /// <summary>
    /// Tworzy encję faktury na podstawie danych z KSeF
    /// Jeśli faktura jest w walucie obcej, konwertuje kwoty na PLN używając kursu NBP z daty płatności (lub faktury)
    /// </summary>
    private static async Task<KSeFInvoiceEntity> CreateInvoiceEntityAsync(
        KSeFInvoiceSyncItem invoiceItem,
        string invoiceXml,
        IKSeFInvoiceXmlParser xmlParser,
        Guid? taxBusinessEntityId = null,
        Guid? farmId = null,
        Guid? cycleId = null,
        INbpExchangeRateService nbpExchangeRateService = null,
        CancellationToken cancellationToken = default)
    {
        // Parsuj XML aby wyciągnąć dodatkowe dane
        var parsedInvoice = xmlParser.ParseInvoiceXml(invoiceXml);

        // Mapuj kierunek faktury
        var invoiceDirection = invoiceItem.Direction == KSeFInvoiceItemDirection.Sales
            ? KSeFInvoiceDirection.Sales
            : KSeFInvoiceDirection.Purchase;

        // Parsuj typ płatności z XML
        var paymentType = ParsePaymentType(parsedInvoice?.Fa?.Platnosc?.FormaPlatnosci);

        // Sprawdź czy faktura jest opłacona (Zaplacono = "1") lub częściowo opłacona
        var paymentStatus = ParsePaymentStatus(
            parsedInvoice?.Fa?.Platnosc?.Zaplacono,
            parsedInvoice?.Fa?.Platnosc?.ZnacznikZaplatyCzesciowej,
            paymentType);

        // Parsuj datę płatności z XML (DataZaplaty)
        var paymentDate = ParsePaymentDate(parsedInvoice?.Fa?.Platnosc?.DataZaplaty);

        // Wyciągnij ilość z pozycji faktury (suma wszystkich pozycji)
        var quantity = ExtractQuantity(parsedInvoice);

        // Wyciągnij kod waluty z XML
        var currencyCode = parsedInvoice?.Fa?.KodWaluty ?? "PLN";

        // Konwersja walut jeśli nie jest PLN
        decimal grossAmountInPLN = invoiceItem.GrossAmount;
        decimal netAmountInPLN = invoiceItem.NetAmount;
        decimal vatAmountInPLN = invoiceItem.VatAmount;
        decimal? exchangeRate = null;
        decimal? originalGrossAmount = null;
        decimal? originalNetAmount = null;
        decimal? originalVatAmount = null;

        if (!string.IsNullOrWhiteSpace(currencyCode) && 
            !string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // Zapisz oryginalne kwoty
                originalGrossAmount = invoiceItem.GrossAmount;
                originalNetAmount = invoiceItem.NetAmount;
                originalVatAmount = invoiceItem.VatAmount;

                // Użyj daty płatności lub daty faktury do pobrania kursu
                var exchangeRateDate = paymentDate ?? invoiceItem.InvoiceDate;

                // Pobierz kurs z NBP API
                if (nbpExchangeRateService != null)
                {
                    exchangeRate = await nbpExchangeRateService.GetExchangeRateAsync(
                        currencyCode, 
                        exchangeRateDate, 
                        cancellationToken);

                    // Konwertuj kwoty na PLN
                    grossAmountInPLN = Math.Round(invoiceItem.GrossAmount * exchangeRate.Value, 2);
                    netAmountInPLN = Math.Round(invoiceItem.NetAmount * exchangeRate.Value, 2);
                    vatAmountInPLN = Math.Round(invoiceItem.VatAmount * exchangeRate.Value, 2);
                }
            }
            catch (Exception)
            {
                // Jeśli nie udało się pobrać kursu, użyj oryginalnych kwot
                // i zaloguj ostrzeżenie (logowanie jest w serwisie NBP)
            }
        }

        return KSeFInvoiceEntity.CreateNew(
            kSeFNumber: invoiceItem.KsefNumber,
            invoiceNumber: invoiceItem.InvoiceNumber,
            invoiceDate: invoiceItem.InvoiceDate,
            paymentDueDate:
            ParsePaymentDueDate(parsedInvoice?.Fa?.Platnosc?.TerminyPlatnosci?.FirstOrDefault()?.Termin),
            sellerNip: invoiceItem.SellerNip,
            sellerName: invoiceItem.SellerName,
            buyerNip: invoiceItem.BuyerNip,
            buyerName: invoiceItem.BuyerName,
            invoiceType: invoiceItem.InvoiceType.ToFarmsInvoiceType(),
            status: KSeFInvoiceStatus.New,
            paymentStatus: paymentStatus,
            paymentType: paymentType,
            vatDeductionType: KSeFVatDeductionType.Full,
            moduleType: ModuleType.None,
            invoiceXml: invoiceXml,
            invoiceDirection: invoiceDirection,
            invoiceSource: KSeFInvoiceSource.KSeF,
            grossAmount: grossAmountInPLN,
            netAmount: netAmountInPLN,
            vatAmount: vatAmountInPLN,
            taxBusinessEntityId: taxBusinessEntityId,
            farmId: farmId,
            cycleId: cycleId,
            paymentDate: paymentDate,
            quantity: quantity,
            currencyCode: currencyCode,
            exchangeRate: exchangeRate,
            originalGrossAmount: originalGrossAmount,
            originalNetAmount: originalNetAmount,
            originalVatAmount: originalVatAmount
        );
    }

    /// <summary>
    /// Parsuje typ płatności z XML KSeF
    /// Wg schematu FA(3): 1-gotówka, 2-karta, 3-bon, 4-barterowa, 5-sprawdzian, 6-przelew
    /// </summary>
    private static KSeFInvoicePaymentType ParsePaymentType(string formaPlatnosci)
    {
        return formaPlatnosci switch
        {
            "1" => KSeFInvoicePaymentType.Cash, // gotówka
            _ => KSeFInvoicePaymentType.BankTransfer // przelew i inne
        };
    }

    /// <summary>
    /// Parsuje status płatności na podstawie pól Zaplacono i ZnacznikZaplatyCzesciowej z XML KSeF
    /// Zaplacono = "1" oznacza że faktura została opłacona
    /// ZnacznikZaplatyCzesciowej:
    ///   "1" = należność zapłacona częściowo
    ///   "2" = należność zapłacona w całości (ale w co najmniej 2 częściach)
    /// </summary>
    private static KSeFPaymentStatus ParsePaymentStatus(
        string zaplacono,
        string znacznikZaplatyCzesciowej,
        KSeFInvoicePaymentType paymentType)
    {
        // Sprawdź najpierw znacznik zapłaty częściowej
        if (znacznikZaplatyCzesciowej == "1")
        {
            // Faktura opłacona częściowo
            return KSeFPaymentStatus.PartiallyPaid;
        }

        if (znacznikZaplatyCzesciowej == "2")
        {
            // Faktura opłacona w całości (w częściach) - traktuj jako w pełni opłaconą
            return paymentType == KSeFInvoicePaymentType.Cash
                ? KSeFPaymentStatus.PaidCash
                : KSeFPaymentStatus.PaidTransfer;
        }

        // Standardowa logika - pole Zaplacono
        if (zaplacono == "1")
        {
            return paymentType == KSeFInvoicePaymentType.Cash
                ? KSeFPaymentStatus.PaidCash
                : KSeFPaymentStatus.PaidTransfer;
        }

        return KSeFPaymentStatus.Unpaid;
    }

    /// <summary>
    /// Parsuje termin płatności z XML KSeF
    /// </summary>
    private static DateOnly? ParsePaymentDueDate(DateTime? terminPlatnosci)
    {
        if (!terminPlatnosci.HasValue)
            return null;

        return DateOnly.FromDateTime(terminPlatnosci.Value);
    }

    /// <summary>
    /// Parsuje datę płatności z XML KSeF (pole DataZaplaty)
    /// </summary>
    private static DateOnly? ParsePaymentDate(DateTime? dataZaplaty)
    {
        if (!dataZaplaty.HasValue)
            return null;

        return DateOnly.FromDateTime(dataZaplaty.Value);
    }

    /// <summary>
    /// Wyciąga ilość z pozycji faktury (suma wszystkich pozycji)
    /// </summary>
    private static decimal? ExtractQuantity(KSeFInvoiceXml parsedInvoice)
    {
        if (parsedInvoice?.Fa?.FaWiersze == null || parsedInvoice.Fa.FaWiersze.Count == 0)
            return null;

        var totalQuantity = parsedInvoice.Fa.FaWiersze
            .Where(w => w.P_8B.HasValue)
            .Sum(w => w.P_8B.Value);

        return totalQuantity > 0 ? totalQuantity : null;
    }

    /// <summary>
    /// Updates KSeFInvoiceEntity with authoritative names from module entities
    /// </summary>
    private static async Task UpdateInvoiceWithAuthoritativeNamesAsync(
        KSeFInvoiceEntity invoiceEntity,
        ModuleType moduleType,
        string sellerNip,
        string buyerNip,
        IGasContractorRepository gasContractorRepository,
        IExpenseContractorRepository expenseContractorRepository,
        ISlaughterhouseRepository slaughterhouseRepository,
        IFeedContractorRepository feedContractorRepository,
        CancellationToken cancellationToken)
    {
        var normalizedSellerNip = NormalizeNip(sellerNip);
        var normalizedBuyerNip = NormalizeNip(buyerNip);

        string authoritativeSellerName = null;
        string authoritativeSellerNip = null;
        string authoritativeBuyerName = null;
        string authoritativeBuyerNip = null;

        switch (moduleType)
        {
            case ModuleType.Gas:
                if (!string.IsNullOrWhiteSpace(normalizedSellerNip))
                {
                    var gasContractor = await gasContractorRepository.FirstOrDefaultAsync(
                        new GasContractorByNipSpec(normalizedSellerNip), cancellationToken);
                    if (gasContractor != null)
                    {
                        authoritativeSellerName = gasContractor.Name;
                        authoritativeSellerNip = gasContractor.Nip;
                    }
                }

                break;

            case ModuleType.ProductionExpenses:
                if (!string.IsNullOrWhiteSpace(normalizedSellerNip))
                {
                    var expenseContractor = await expenseContractorRepository.FirstOrDefaultAsync(
                        new ExpenseContractorByNipSpec(normalizedSellerNip), cancellationToken);
                    if (expenseContractor != null)
                    {
                        authoritativeSellerName = expenseContractor.Name;
                        authoritativeSellerNip = expenseContractor.Nip;
                    }
                }

                break;

            case ModuleType.Sales:
                if (!string.IsNullOrWhiteSpace(normalizedBuyerNip))
                {
                    var slaughterhouse = await slaughterhouseRepository.FirstOrDefaultAsync(
                        new SlaughterhouseByNipSpec(normalizedBuyerNip), cancellationToken);
                    if (slaughterhouse != null)
                    {
                        authoritativeBuyerName = slaughterhouse.Name;
                        authoritativeBuyerNip = slaughterhouse.Nip;
                    }
                }

                break;

            case ModuleType.Feeds:
                if (!string.IsNullOrWhiteSpace(normalizedSellerNip))
                {
                    var feedContractor = await feedContractorRepository.FirstOrDefaultAsync(
                        new FeedContractorByNipSpec(normalizedSellerNip), cancellationToken);
                    if (feedContractor != null)
                    {
                        authoritativeSellerName = feedContractor.Name;
                        authoritativeSellerNip = feedContractor.Nip;
                    }
                }

                break;
        }

        // Update invoice with authoritative names if any were found
        if (authoritativeSellerName != null || authoritativeSellerNip != null ||
            authoritativeBuyerName != null || authoritativeBuyerNip != null)
        {
            invoiceEntity.UpdateSellerBuyerInfo(
                authoritativeSellerName,
                authoritativeSellerNip,
                authoritativeBuyerName,
                authoritativeBuyerNip);
        }
    }

    /// <summary>
    /// Sprawdza czy typ faktury wymaga powiązania z inną fakturą
    /// </summary>
    private static bool InvoiceRequiresLinking(FarmsInvoiceType invoiceType)
    {
        return invoiceType switch
        {
            FarmsInvoiceType.Zal => true, // Zaliczkowa
            FarmsInvoiceType.Roz => true, // Rozliczeniowa
            FarmsInvoiceType.Kor => true, // Korygująca
            FarmsInvoiceType.KorZal => true, // Korygująca zaliczkową
            FarmsInvoiceType.KorRoz => true, // Korygująca rozliczeniową
            FarmsInvoiceType.KorPef => true, // PEF Korygująca
            FarmsInvoiceType.KorVatRr => true, // RR Korygująca
            FarmsInvoiceType.CostInvoiceCorrection => true, // Korekta rachunku kosztowego
            _ => false
        };
    }

    /// <summary>
    /// Sprawdza czy istnieją podobne faktury w systemie (potencjalne duplikaty)
    /// Wykrywa faktury o podobnych kwotach, od tych samych podmiotów i w zbliżonym czasie
    /// </summary>
    private async Task<List<KSeFInvoiceEntity>> CheckForSimilarInvoicesAsync(
        KSeFInvoiceSyncItem invoiceItem,
        IKSeFInvoiceRepository invoiceRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedSellerNip = NormalizeNip(invoiceItem.SellerNip);
            var normalizedBuyerNip = NormalizeNip(invoiceItem.BuyerNip);

            // Sprawdź tylko jeśli mamy NIP do porównania
            if (string.IsNullOrWhiteSpace(normalizedSellerNip) && string.IsNullOrWhiteSpace(normalizedBuyerNip))
                return new List<KSeFInvoiceEntity>();

            var similarInvoicesSpec = new SimilarKSeFInvoicesSpec(
                normalizedSellerNip,
                normalizedBuyerNip,
                invoiceItem.GrossAmount,
                invoiceItem.InvoiceDate,
                amountTolerancePercentage: 5.0m, // 5% tolerancji kwoty
                dateRangeDays: 30); // 30 dni tolerancji daty

            var similarInvoices = await invoiceRepository.ListAsync(similarInvoicesSpec, cancellationToken);

            // Filtruj faktury o identycznym numerze (te są już sprawdzane gdzie indziej)
            var filteredInvoices = similarInvoices
                .Where(i => !string.Equals(i.InvoiceNumber?.Trim(), invoiceItem.InvoiceNumber?.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Dodatkowo sprawdź podobne numery faktur (np. "66/2025" vs "66 /2025")
            var similarNumberInvoices = filteredInvoices
                .Where(i => AreInvoiceNumbersSimilar(i.InvoiceNumber, invoiceItem.InvoiceNumber))
                .ToList();

            if (similarNumberInvoices.Count > 0)
            {
                _logger.LogWarning(
                    "Found {Count} potential duplicate invoices for KSeF {KsefNumber} ({InvoiceNumber}) from {SellerName} - Amount: {GrossAmount:C}",
                    similarNumberInvoices.Count,
                    invoiceItem.KsefNumber,
                    invoiceItem.InvoiceNumber,
                    invoiceItem.SellerName,
                    invoiceItem.GrossAmount);

                foreach (var similar in similarNumberInvoices)
                {
                    var similarityType = AreInvoiceNumbersSimilar(similar.InvoiceNumber, invoiceItem.InvoiceNumber) 
                        ? "SIMILAR NUMBER" 
                        : "SIMILAR AMOUNT/ENTITY";
                    
                    _logger.LogWarning(
                        "  {SimilarityType}: {InvoiceNumber} from {SellerName} - Amount: {GrossAmount:C}, Date: {InvoiceDate}",
                        similarityType,
                        similar.InvoiceNumber,
                        similar.SellerName,
                        similar.GrossAmount,
                        similar.InvoiceDate);
                }
            }

            return similarNumberInvoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for similar invoices for KSeF {KsefNumber}", invoiceItem.KsefNumber);
            return new List<KSeFInvoiceEntity>();
        }
    }

    /// <summary>
    /// Normalizuje numer faktury do porównań (usuwa zbędne spacje, slashe itp.)
    /// </summary>
    private static string NormalizeInvoiceNumber(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return string.Empty;

        // Usuń zbędne spacje wokół slashe i innych znaków
        return System.Text.RegularExpressions.Regex.Replace(invoiceNumber.Trim(), @"\s*[/\\-]\s*", "/");
    }

    /// <summary>
    /// Sprawdza czy numery faktur są podobne (z tolerancją na błędy OCR)
    /// </summary>
    private static bool AreInvoiceNumbersSimilar(string number1, string number2)
    {
        if (string.IsNullOrWhiteSpace(number1) || string.IsNullOrWhiteSpace(number2))
            return false;

        var normalized1 = NormalizeInvoiceNumber(number1);
        var normalized2 = NormalizeInvoiceNumber(number2);

        // Dokładne dopasowanie po normalizacji
        if (string.Equals(normalized1, normalized2, StringComparison.OrdinalIgnoreCase))
            return true;

        // Sprawdź czy są bardzo podobne (Levenshtein distance)
        var distance = ComputeLevenshteinDistance(normalized1, normalized2);
        var maxLength = Math.Max(normalized1.Length, normalized2.Length);
        
        // Jeśli różnica jest mniejsza niż 20% długości, uznaj za podobne
        return maxLength > 0 && (double)distance / maxLength <= 0.2;
    }

    /// <summary>
    /// Oblicza odległość Levenshteina między dwoma stringami
    /// </summary>
    private static int ComputeLevenshteinDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1))
            return string.IsNullOrEmpty(s2) ? 0 : s2.Length;
        if (string.IsNullOrEmpty(s2))
            return s1.Length;

        var matrix = new int[s1.Length + 1, s2.Length + 1];

        for (var i = 0; i <= s1.Length; i++)
            matrix[i, 0] = i;
        for (var j = 0; j <= s2.Length; j++)
            matrix[0, j] = j;

        for (var i = 1; i <= s1.Length; i++)
        {
            for (var j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[s1.Length, s2.Length];
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}