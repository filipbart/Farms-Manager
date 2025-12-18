using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using KSeF.Client.Core.Models.Invoices.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Accounting;

public record SaveAccountingInvoiceDto
{
    public Guid DraftId { get; init; }
    public string FilePath { get; init; }
    public string InvoiceNumber { get; init; }
    public string InvoiceDate { get; init; }
    public string DueDate { get; init; }
    public string SellerName { get; init; }
    public string SellerNip { get; init; }
    public string BuyerName { get; init; }
    public string BuyerNip { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal NetAmount { get; init; }
    public decimal VatAmount { get; init; }
    public string InvoiceType { get; init; }
    public string Comment { get; init; }
}

public record SaveAccountingInvoiceCommand(SaveAccountingInvoiceDto Data)
    : IRequest<BaseResponse<Guid>>;

public class SaveAccountingInvoiceCommandHandler : IRequestHandler<SaveAccountingInvoiceCommand, BaseResponse<Guid>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IS3Service _s3Service;
    private readonly DbContext _dbContext;

    public SaveAccountingInvoiceCommandHandler(
        IKSeFInvoiceRepository invoiceRepository,
        IUserDataResolver userDataResolver,
        IS3Service s3Service,
        DbContext dbContext)
    {
        _invoiceRepository = invoiceRepository;
        _userDataResolver = userDataResolver;
        _s3Service = s3Service;
        _dbContext = dbContext;
    }

    public async Task<BaseResponse<Guid>> Handle(SaveAccountingInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var data = request.Data;

        // Parsuj datę faktury
        if (!DateOnly.TryParse(data.InvoiceDate, out var invoiceDate))
        {
            throw new Exception("Nieprawidłowy format daty faktury");
        }

        // Określ kierunek faktury
        var invoiceDirection = data.InvoiceType == "Sales"
            ? KSeFInvoiceDirection.Sales
            : KSeFInvoiceDirection.Purchase;

        // Przenieś plik z draft do stałej lokalizacji
        var permanentPath = $"accounting/{data.DraftId}{Path.GetExtension(data.FilePath)}";
        await _s3Service.MoveFileAsync(FileType.AccountingInvoice, data.FilePath, permanentPath);

        // Dopasuj podmiot gospodarczy po NIP sprzedawcy lub nabywcy
        var taxBusinessEntityId = await MatchTaxBusinessEntityAsync(
            data.SellerNip, data.SellerName, data.BuyerNip, data.BuyerName, cancellationToken);

        // Utwórz encję faktury
        var invoice = KSeFInvoiceEntity.CreateNew(
            kSeFNumber: $"MANUAL-{data.DraftId}",
            invoiceNumber: data.InvoiceNumber,
            invoiceDate: invoiceDate,
            sellerNip: data.SellerNip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            sellerName: data.SellerName,
            buyerNip: data.BuyerNip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim(),
            buyerName: data.BuyerName,
            invoiceType: InvoiceType.Vat,
            status: KSeFInvoiceStatus.New,
            paymentStatus: KSeFPaymentStatus.Unpaid,
            paymentType: KSeFInvoicePaymentType.BankTransfer,
            vatDeductionType: KSeFVatDeductionType.Full,
            moduleType: ModuleType.None,
            invoiceXml: string.Empty, // Brak XML dla manualnych faktur
            invoiceDirection: invoiceDirection,
            invoiceSource: KSeFInvoiceSource.Manual,
            grossAmount: data.GrossAmount,
            netAmount: data.NetAmount,
            vatAmount: data.VatAmount,
            comment: data.Comment,
            userId: userId,
            taxBusinessEntityId: taxBusinessEntityId
        );

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(invoice.Id);
    }

    /// <summary>
    /// Dopasowuje podmiot gospodarczy na podstawie NIP lub nazwy
    /// </summary>
    private async Task<Guid?> MatchTaxBusinessEntityAsync(
        string sellerNip, string sellerName, string buyerNip, string buyerName,
        CancellationToken cancellationToken)
    {
        var taxBusinessEntities = await _dbContext.Set<TaxBusinessEntity>()
            .Where(t => t.DateDeletedUtc == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (taxBusinessEntities.Count == 0)
            return null;

        // Normalizuj NIP-y
        sellerNip = NormalizeNip(sellerNip);
        buyerNip = NormalizeNip(buyerNip);

        // Szukaj po NIP (dokładne dopasowanie)
        var matchedByNip = taxBusinessEntities.FirstOrDefault(t =>
            t.Nip == sellerNip || t.Nip == buyerNip);

        if (matchedByNip != null)
            return matchedByNip.Id;

        // Szukaj po nazwie (częściowe dopasowanie)
        var sellerNameLower = sellerName?.ToLowerInvariant();
        var buyerNameLower = buyerName?.ToLowerInvariant();

        var matchedByName = taxBusinessEntities.FirstOrDefault(t =>
        {
            var entityName = t.Name?.ToLowerInvariant();
            if (string.IsNullOrEmpty(entityName))
                return false;

            return (!string.IsNullOrEmpty(sellerNameLower) && sellerNameLower.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(buyerNameLower) && buyerNameLower.Contains(entityName)) ||
                   (!string.IsNullOrEmpty(sellerNameLower) && entityName.Contains(sellerNameLower)) ||
                   (!string.IsNullOrEmpty(buyerNameLower) && entityName.Contains(buyerNameLower));
        });

        return matchedByName?.Id;
    }

    private static string NormalizeNip(string nip)
    {
        return nip?.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
    }
}

public class SaveAccountingInvoiceCommandValidator : AbstractValidator<SaveAccountingInvoiceCommand>
{
    public SaveAccountingInvoiceCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().WithMessage("Dane są wymagane.");
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty().WithMessage("Numer faktury jest wymagany.");
        RuleFor(t => t.Data.InvoiceDate).NotEmpty().WithMessage("Data faktury jest wymagana.");
        RuleFor(t => t.Data.InvoiceType).NotEmpty().WithMessage("Typ faktury jest wymagany.");
        RuleFor(t => t.Data.GrossAmount).GreaterThan(0).WithMessage("Kwota brutto musi być większa od 0.");
    }
}
