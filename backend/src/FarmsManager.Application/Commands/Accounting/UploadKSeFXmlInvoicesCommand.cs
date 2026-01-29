using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Application.Specifications.TaxBusinessEntities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Accounting;

public record UploadKSeFXmlInvoicesCommandDto
{
    public List<IFormFile> Files { get; init; }
    public string InvoiceType { get; init; } // Purchase or Sales
    public string PaymentStatus { get; init; } // Unpaid, PaidCash, PaidTransfer, etc.
    public DateOnly? PaymentDate { get; init; } // Optional: Payment date when status is paid
}

public record UploadKSeFXmlInvoicesCommandResponse
{
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public List<string> Errors { get; init; } = [];
}

public record UploadKSeFXmlInvoicesCommand(UploadKSeFXmlInvoicesCommandDto Data)
    : IRequest<BaseResponse<UploadKSeFXmlInvoicesCommandResponse>>;

public class UploadKSeFXmlInvoicesCommandHandler : IRequestHandler<UploadKSeFXmlInvoicesCommand,
    BaseResponse<UploadKSeFXmlInvoicesCommandResponse>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IKSeFInvoiceXmlParser _xmlParser;
    private readonly IUserDataResolver _userDataResolver;
    private readonly ITaxBusinessEntityRepository _taxBusinessEntityRepository;

    public UploadKSeFXmlInvoicesCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IKSeFInvoiceXmlParser xmlParser,
        IUserDataResolver userDataResolver,
        ITaxBusinessEntityRepository taxBusinessEntityRepository)
    {
        _invoiceRepository = invoiceRepository;
        _xmlParser = xmlParser;
        _userDataResolver = userDataResolver;
        _taxBusinessEntityRepository = taxBusinessEntityRepository;
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
        var taxBusinessEntities = await _taxBusinessEntityRepository.ListAsync(
            new AllActiveTaxBusinessEntitiesSpec(),
            cancellationToken);

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
                var paymentDueDate = ParsePaymentDueDate(parsedInvoice.Fa?.Platnosc?.TerminyPlatnosci?.FirstOrDefault()?.Termin);
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
                

                // Sprawdź czy faktura z tym numerem już istnieje
                var existingInvoice = await _invoiceRepository.AnyAsync(
                    new KSeFInvoiceByInvoiceNumberSpec(invoiceNumber),
                    cancellationToken);
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
                var paymentStatus = ParsePaymentStatusFromRequest(request.Data.PaymentStatus);

                // Utwórz encję faktury
                var invoiceEntity = KSeFInvoiceEntity.CreateNew(
                    kSeFNumber: null,
                    invoiceNumber: invoiceNumber,
                    invoiceDate: invoiceDate,
                    paymentDueDate: paymentDueDate,
                    sellerNip: sellerNip,
                    sellerName: sellerName,
                    buyerNip: buyerNip,
                    buyerName: buyerName,
                    invoiceType: FarmsInvoiceType.Vat,
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
                    taxBusinessEntityId: taxBusinessEntityId,
                    paymentDate: request.Data.PaymentDate
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

    private static KSeFPaymentStatus ParsePaymentStatusFromRequest(string paymentStatus)
    {
        return paymentStatus switch
        {
            "Unpaid" => KSeFPaymentStatus.Unpaid,
            "PartiallyPaid" => KSeFPaymentStatus.PartiallyPaid,
            "Suspended" => KSeFPaymentStatus.Suspended,
            "PaidCash" => KSeFPaymentStatus.PaidCash,
            "PaidTransfer" => KSeFPaymentStatus.PaidTransfer,
            _ => KSeFPaymentStatus.Unpaid
        };
    }

    private static DateOnly? ParsePaymentDueDate(DateTime? terminPlatnosci)
    {
        if (!terminPlatnosci.HasValue)
            return null;

        return DateOnly.FromDateTime(terminPlatnosci.Value);
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
