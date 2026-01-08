using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Accounting;

public record UploadKSeFXmlInvoicesCommandDto
{
    public List<IFormFile> Files { get; init; }
    public string InvoiceType { get; init; } // Purchase or Sales
}

public record UploadKSeFXmlInvoicesCommandResponse
{
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public List<string> Errors { get; init; } = new();
}

public record UploadKSeFXmlInvoicesCommand(UploadKSeFXmlInvoicesCommandDto Data)
    : IRequest<BaseResponse<UploadKSeFXmlInvoicesCommandResponse>>;

public class UploadKSeFXmlInvoicesCommandHandler : IRequestHandler<UploadKSeFXmlInvoicesCommand,
    BaseResponse<UploadKSeFXmlInvoicesCommandResponse>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IKSeFInvoiceXmlParser _xmlParser;
    private readonly IUserDataResolver _userDataResolver;
    private readonly DbContext _dbContext;

    public UploadKSeFXmlInvoicesCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IKSeFInvoiceXmlParser xmlParser,
        IUserDataResolver userDataResolver,
        DbContext dbContext)
    {
        _invoiceRepository = invoiceRepository;
        _xmlParser = xmlParser;
        _userDataResolver = userDataResolver;
        _dbContext = dbContext;
    }

    public async Task<BaseResponse<UploadKSeFXmlInvoicesCommandResponse>> Handle(
        UploadKSeFXmlInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var importedCount = 0;
        var skippedCount = 0;
        var errors = new List<string>();

        // Pobierz podmioty gospodarcze do dopasowania
        var taxBusinessEntities = await _dbContext.Set<TaxBusinessEntity>()
            .Where(t => t.DateDeletedUtc == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var file in request.Data.Files)
        {
            try
            {
                // Wczytaj zawartość XML
                using var reader = new StreamReader(file.OpenReadStream());
                var xmlContent = await reader.ReadToEndAsync(cancellationToken);

                // Parsuj XML
                var parsedInvoice = _xmlParser.ParseInvoiceXml(xmlContent);
                if (parsedInvoice == null)
                {
                    errors.Add($"Nie można sparsować pliku: {file.FileName}");
                    continue;
                }

                // Wyciągnij dane z XML
                var invoiceNumber = parsedInvoice.Fa?.P_2 ?? $"MANUAL-{Guid.NewGuid():N}";
                var invoiceDate = parsedInvoice.Fa?.P_1 != null 
                    ? DateOnly.FromDateTime(parsedInvoice.Fa.P_1) 
                    : DateOnly.FromDateTime(DateTime.UtcNow);
                var sellerNip = parsedInvoice.Podmiot1?.DaneIdentyfikacyjne?.NIP ?? "";
                var sellerName = parsedInvoice.Podmiot1?.DaneIdentyfikacyjne?.Nazwa ?? "";
                var buyerNip = parsedInvoice.Podmiot2?.DaneIdentyfikacyjne?.NIP ?? "";
                var buyerName = parsedInvoice.Podmiot2?.DaneIdentyfikacyjne?.Nazwa ?? "";

                // Oblicz kwoty z wierszy faktury
                decimal netAmount = 0;
                decimal vatAmount = 0;

                if (parsedInvoice.Fa?.FaWiersze?.Any() == true)
                {
                    foreach (var wiersz in parsedInvoice.Fa.FaWiersze)
                    {
                        netAmount += wiersz.P_11 ?? 0;
                    }
                }
                
                // Pobierz VAT z podsumowania faktury
                if (parsedInvoice.Fa?.P_13_1 != null)
                {
                    vatAmount = parsedInvoice.Fa.P_13_1.Value;
                }
                
                var grossAmount = netAmount + vatAmount;

                // Generuj unikalny numer KSeF dla manualnych faktur
                var kSeFNumber = $"MANUAL-{Guid.NewGuid():N}";

                // Sprawdź czy faktura z tym numerem już istnieje
                var existingInvoice = await _dbContext.Set<KSeFInvoiceEntity>()
                    .AnyAsync(i => i.InvoiceNumber == invoiceNumber && i.DateDeletedUtc == null, cancellationToken);
                if (existingInvoice)
                {
                    skippedCount++;
                    errors.Add($"Faktura o numerze {invoiceNumber} już istnieje");
                    continue;
                }

                // Dopasuj podmiot gospodarczy
                var taxBusinessEntityId = MatchTaxBusinessEntity(sellerNip, buyerNip, sellerName, buyerName, taxBusinessEntities);

                // Określ kierunek faktury
                var invoiceDirection = request.Data.InvoiceType == "Sales"
                    ? KSeFInvoiceDirection.Sales
                    : KSeFInvoiceDirection.Purchase;

                // Parsuj typ płatności
                var paymentType = ParsePaymentType(parsedInvoice.Fa?.Platnosc?.FormaPlatnosci);
                var paymentStatus = ParsePaymentStatus(parsedInvoice.Fa?.Platnosc?.Zaplacono, paymentType);

                // Utwórz encję faktury
                var invoiceEntity = KSeFInvoiceEntity.CreateNew(
                    kSeFNumber: kSeFNumber,
                    invoiceNumber: invoiceNumber,
                    invoiceDate: invoiceDate,
                    sellerNip: sellerNip,
                    sellerName: sellerName,
                    buyerNip: buyerNip,
                    buyerName: buyerName,
                    invoiceType: KSeF.Client.Core.Models.Invoices.Common.InvoiceType.Vat,
                    status: KSeFInvoiceStatus.Accepted,
                    paymentStatus: paymentStatus,
                    paymentType: paymentType,
                    vatDeductionType: KSeFVatDeductionType.Full,
                    moduleType: ModuleType.None,
                    invoiceXml: xmlContent,
                    invoiceDirection: invoiceDirection,
                    invoiceSource: KSeFInvoiceSource.Manual,
                    grossAmount: grossAmount,
                    netAmount: netAmount,
                    vatAmount: vatAmount,
                    taxBusinessEntityId: taxBusinessEntityId
                );

                await _invoiceRepository.AddAsync(invoiceEntity, cancellationToken);
                importedCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Błąd podczas przetwarzania pliku {file.FileName}: {ex.Message}");
            }
        }

        await _invoiceRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(new UploadKSeFXmlInvoicesCommandResponse
        {
            ImportedCount = importedCount,
            SkippedCount = skippedCount,
            Errors = errors
        });
    }


    private static Guid? MatchTaxBusinessEntity(
        string sellerNip, string buyerNip, string sellerName, string buyerName,
        List<TaxBusinessEntity> taxBusinessEntities)
    {
        if (taxBusinessEntities.Count == 0)
            return null;

        var normalizedSellerNip = NormalizeNip(sellerNip);
        var normalizedBuyerNip = NormalizeNip(buyerNip);

        var matchedByNip = taxBusinessEntities.FirstOrDefault(t =>
            t.Nip == normalizedSellerNip || t.Nip == normalizedBuyerNip);

        if (matchedByNip != null)
            return matchedByNip.Id;

        var sellerNameLower = sellerName?.ToLowerInvariant();
        var buyerNameLower = buyerName?.ToLowerInvariant();

        var matchedByName = taxBusinessEntities.FirstOrDefault(t =>
        {
            var entityName = t.Name?.ToLowerInvariant();
            if (string.IsNullOrEmpty(entityName))
                return false;

            return (!string.IsNullOrEmpty(sellerNameLower) && sellerNameLower.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(buyerNameLower) && buyerNameLower.Contains(entityName));
        });

        return matchedByName?.Id;
    }

    private static string NormalizeNip(string nip)
    {
        return nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    }

    private static KSeFInvoicePaymentType ParsePaymentType(string formaPlatnosci)
    {
        return formaPlatnosci switch
        {
            "1" => KSeFInvoicePaymentType.Cash,
            _ => KSeFInvoicePaymentType.BankTransfer
        };
    }

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
}

public class UploadKSeFXmlInvoicesCommandValidator : AbstractValidator<UploadKSeFXmlInvoicesCommand>
{
    public UploadKSeFXmlInvoicesCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().WithMessage("Dane są wymagane.");
        RuleFor(t => t.Data.Files).NotNull().NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
        RuleFor(t => t.Data.InvoiceType).NotEmpty().WithMessage("Typ faktury jest wymagany.");
    }
}
