using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using FarmsManager.Application.Specifications.KSeF;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
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

        // Pobierz wszystkie podmioty gospodarcze do dopasowania (wraz z fermami)
        var taxBusinessEntities = await dbContext.Set<TaxBusinessEntity>()
            .Include(t => t.Farms.Where(f => f.DateDeletedUtc == null))
            .Where(t => t.DateDeletedUtc == null)
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

            // 1. Pobierz wszystkie faktury z KSeF
            var invoices = await ksefService.GetInvoicesForSyncAsync(cancellationToken);
            var downloadedCount = invoices.Count;

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
                    var invoiceXml = await ksefService.GetInvoiceXmlAsync(invoiceSummary.KsefNumber, cancellationToken);

                    // Dopasuj podmiot gospodarczy po NIP lub nazwie
                    var (taxBusinessEntityId, farmId) = MatchTaxBusinessEntityAndFarm(invoiceSummary, taxBusinessEntities);

                    var invoiceEntity = CreateInvoiceEntity(invoiceSummary, invoiceXml, xmlParser, taxBusinessEntityId, farmId);

                    // Sprawdź czy faktura wymaga powiązania z inną fakturą
                    if (InvoiceRequiresLinking(invoiceSummary.InvoiceType))
                    {
                        invoiceEntity.MarkAsRequiresLinking();
                    }

                    // Automatyczne przypisanie pracownika na podstawie reguł
                    var assignedUserId = await invoiceAssignmentService.FindAssignedUserForInvoiceAsync(invoiceEntity, cancellationToken);
                    if (assignedUserId.HasValue)
                    {
                        invoiceEntity.Update(assignedUserId: assignedUserId.Value);
                        _logger.LogDebug("Invoice {KsefNumber} auto-assigned to user {UserId}", invoiceSummary.KsefNumber, assignedUserId.Value);
                    }

                    // Automatyczne przypisanie modułu na podstawie reguł
                    var assignedModule = await invoiceAssignmentService.FindModuleForInvoiceAsync(invoiceEntity, cancellationToken);
                    if (assignedModule.HasValue)
                    {
                        invoiceEntity.Update(moduleType: assignedModule.Value);
                        _logger.LogDebug("Invoice {KsefNumber} auto-assigned to module {ModuleType}", invoiceSummary.KsefNumber, assignedModule.Value);
                    }

                    await invoiceRepository.AddAsync(invoiceEntity, cancellationToken);
                    savedCount++;

                    _logger.LogDebug("Saved invoice {KsefNumber}", invoiceSummary.KsefNumber);
                }
                catch (Exception ex)
                {
                    log.ErrorsCount++;
                    _logger.LogWarning(ex, "Failed to save invoice {KsefNumber}", invoiceSummary.KsefNumber);
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
        }
        finally
        {
            // Zapisz log do bazy
            await syncLogRepository.AddAsync(log, cancellationToken);
            await syncLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Dopasowuje podmiot gospodarczy i fermę do faktury na podstawie NIP lub nazwy
    /// </summary>
    private static (Guid? TaxBusinessEntityId, Guid? FarmId) MatchTaxBusinessEntityAndFarm(
        KSeFInvoiceSyncItem invoiceItem,
        List<TaxBusinessEntity> taxBusinessEntities)
    {
        if (taxBusinessEntities.Count == 0)
            return (null, null);

        // Normalizuj NIP-y z faktury
        var sellerNip = NormalizeNip(invoiceItem.SellerNip);
        var buyerNip = NormalizeNip(invoiceItem.BuyerNip);

        // Szukaj po NIP sprzedawcy lub nabywcy (dokładne dopasowanie)
        var matchedByNip = taxBusinessEntities.FirstOrDefault(t =>
            t.Nip == sellerNip || t.Nip == buyerNip);

        if (matchedByNip != null)
        {
            var farmId = matchedByNip.Farms.FirstOrDefault()?.Id;
            return (matchedByNip.Id, farmId);
        }

        // Szukaj po nazwie (częściowe dopasowanie - case insensitive)
        var sellerName = invoiceItem.SellerName?.ToLowerInvariant();
        var buyerName = invoiceItem.BuyerName?.ToLowerInvariant();

        var matchedByName = taxBusinessEntities.FirstOrDefault(t =>
        {
            var entityName = t.Name?.ToLowerInvariant();
            if (string.IsNullOrEmpty(entityName))
                return false;

            return (!string.IsNullOrEmpty(sellerName) && sellerName.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(buyerName) && buyerName.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(sellerName) && entityName.Contains(sellerName)) ||
                   (!string.IsNullOrEmpty(buyerName) && entityName.Contains(buyerName));
        });

        if (matchedByName != null)
        {
            var farmId = matchedByName.Farms.FirstOrDefault()?.Id;
            return (matchedByName.Id, farmId);
        }

        return (null, null);
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
    /// </summary>
    private static KSeFInvoiceEntity CreateInvoiceEntity(
        KSeFInvoiceSyncItem invoiceItem,
        string invoiceXml,
        IKSeFInvoiceXmlParser xmlParser,
        Guid? taxBusinessEntityId = null,
        Guid? farmId = null)
    {
        // Parsuj XML aby wyciągnąć dodatkowe dane
        var parsedInvoice = xmlParser.ParseInvoiceXml(invoiceXml);

        // Mapuj kierunek faktury
        var invoiceDirection = invoiceItem.Direction == KSeFInvoiceItemDirection.Sales
            ? KSeFInvoiceDirection.Sales
            : KSeFInvoiceDirection.Purchase;

        // Parsuj typ płatności z XML
        var paymentType = ParsePaymentType(parsedInvoice?.Fa?.Platnosc?.FormaPlatnosci);

        // Sprawdź czy faktura jest opłacona (Zaplacono = "1")
        var paymentStatus = ParsePaymentStatus(
            parsedInvoice?.Fa?.Platnosc?.Zaplacono,
            paymentType);

        return KSeFInvoiceEntity.CreateNew(
            kSeFNumber: invoiceItem.KsefNumber,
            invoiceNumber: invoiceItem.InvoiceNumber,
            invoiceDate: invoiceItem.InvoiceDate,
            sellerNip: invoiceItem.SellerNip,
            sellerName: invoiceItem.SellerName,
            buyerNip: invoiceItem.BuyerNip,
            buyerName: invoiceItem.BuyerName,
            invoiceType: invoiceItem.InvoiceType,
            status: KSeFInvoiceStatus.New,
            paymentStatus: paymentStatus,
            paymentType: paymentType,
            vatDeductionType: KSeFVatDeductionType.Full,
            moduleType: ModuleType.None,
            invoiceXml: invoiceXml,
            invoiceDirection: invoiceDirection,
            invoiceSource: KSeFInvoiceSource.KSeF,
            grossAmount: invoiceItem.GrossAmount,
            netAmount: invoiceItem.NetAmount,
            vatAmount: invoiceItem.VatAmount,
            taxBusinessEntityId: taxBusinessEntityId,
            farmId: farmId
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
    /// Parsuje status płatności na podstawie pola Zaplacono z XML KSeF
    /// Zaplacono = "1" oznacza że faktura została opłacona
    /// </summary>
    private static KSeFPaymentStatus ParsePaymentStatus(string zaplacono, KSeFInvoicePaymentType paymentType)
    {
        if (zaplacono == "1")
        {
            return paymentType == KSeFInvoicePaymentType.Cash
                ? KSeFPaymentStatus.PaidCash
                : KSeFPaymentStatus.PaidTransfer;
        }

        return KSeFPaymentStatus.Unpaid;
    }

    /// <summary>
    /// Sprawdza czy typ faktury wymaga powiązania z inną fakturą
    /// </summary>
    private static bool InvoiceRequiresLinking(KSeF.Client.Core.Models.Invoices.Common.InvoiceType invoiceType)
    {
        return invoiceType switch
        {
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.Zal => true,      // Zaliczkowa
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.Roz => true,      // Rozliczeniowa
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.Kor => true,      // Korygująca
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.KorZal => true,   // Korygująca zaliczkową
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.KorRoz => true,   // Korygująca rozliczeniową
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.KorPef => true,   // PEF Korygująca
            KSeF.Client.Core.Models.Invoices.Common.InvoiceType.KorVatRr => true, // RR Korygująca
            _ => false
        };
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}