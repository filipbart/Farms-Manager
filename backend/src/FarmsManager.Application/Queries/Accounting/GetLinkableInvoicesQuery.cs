using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;
using System.ComponentModel;

namespace FarmsManager.Application.Queries.Accounting;

public record GetLinkableInvoicesQuery(GetLinkableInvoicesFilters Filters) : IRequest<BaseResponse<List<LinkableInvoiceDto>>>;

public class GetLinkableInvoicesFilters
{
    public Guid SourceInvoiceId { get; set; }
    public string SearchPhrase { get; set; }
    public int Limit { get; set; } = 20;
}

public class LinkableInvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public string KSeFNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string SellerName { get; set; }
    public string SellerNip { get; set; }
    public string BuyerName { get; set; }
    public string BuyerNip { get; set; }
    public decimal GrossAmount { get; set; }
    public string InvoiceTypeDescription { get; set; }
}

public class GetLinkableInvoicesQueryHandler : IRequestHandler<GetLinkableInvoicesQuery, BaseResponse<List<LinkableInvoiceDto>>>
{
    private readonly IKSeFInvoiceRepository _invoiceRepository;

    public GetLinkableInvoicesQueryHandler(IKSeFInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<BaseResponse<List<LinkableInvoiceDto>>> Handle(GetLinkableInvoicesQuery request, CancellationToken cancellationToken)
    {
        // First get the source invoice to determine filtering criteria
        var sourceSpec = new GetSourceInvoiceSpec(request.Filters.SourceInvoiceId);
        var sourceInvoice = await _invoiceRepository.GetAsync(sourceSpec, cancellationToken);


        // Get linkable invoices based on source invoice type
        var spec = new GetLinkableInvoicesSpec(
            sourceInvoiceId: request.Filters.SourceInvoiceId,
            sourceInvoiceType: sourceInvoice.InvoiceType,
            sellerNip: sourceInvoice.SellerNip,
            buyerNip: sourceInvoice.BuyerNip,
            searchPhrase: request.Filters.SearchPhrase,
            limit: request.Filters.Limit
        );

        var invoices = await _invoiceRepository.ListAsync(spec, cancellationToken);

        var result = invoices.Select(i => new LinkableInvoiceDto
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            KSeFNumber = i.KSeFNumber,
            InvoiceDate = i.InvoiceDate,
            SellerName = i.SellerName,
            SellerNip = i.SellerNip,
            BuyerName = i.BuyerName,
            BuyerNip = i.BuyerNip,
            GrossAmount = i.GrossAmount,
            InvoiceTypeDescription = GetInvoiceTypeDescription(i.InvoiceType)
        }).ToList();

        return BaseResponse.CreateResponse(result);
    }

    private static string GetInvoiceTypeDescription(FarmsInvoiceType type)
    {
        var fieldInfo = type.GetType().GetField(type.ToString());
        var attribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? type.ToString();
    }
}

public class GetSourceInvoiceSpec : BaseSpecification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public GetSourceInvoiceSpec(Guid invoiceId)
    {
        EnsureExists();
        Query.Where(x => x.Id == invoiceId);
    }
}

public class GetLinkableInvoicesSpec : BaseSpecification<KSeFInvoiceEntity>
{
    public GetLinkableInvoicesSpec(
        Guid sourceInvoiceId,
        FarmsInvoiceType sourceInvoiceType,
        string sellerNip,
        string buyerNip,
        string searchPhrase,
        int limit)
    {
        // Exclude the source invoice itself and deleted invoices
        EnsureExists();
        Query.Where(x => x.Id != sourceInvoiceId);

        // Filter by contractor (same seller or buyer NIP)
        if (!string.IsNullOrEmpty(sellerNip) || !string.IsNullOrEmpty(buyerNip))
        {
            Query.Where(x =>
                (sellerNip != null && (x.SellerNip == sellerNip || x.BuyerNip == sellerNip)) ||
                (buyerNip != null && (x.SellerNip == buyerNip || x.BuyerNip == buyerNip))
            );
        }

        // Filter by compatible invoice types based on source invoice type
        var compatibleTypes = GetCompatibleInvoiceTypes(sourceInvoiceType);
        if (compatibleTypes.Any())
        {
            Query.Where(x => compatibleTypes.Contains(x.InvoiceType));
        }

        // Search phrase filter
        if (!string.IsNullOrEmpty(searchPhrase))
        {
            var phrase = searchPhrase.ToLower();
            Query.Where(x =>
                x.InvoiceNumber.ToLower().Contains(phrase) ||
                x.KSeFNumber.ToLower().Contains(phrase) ||
                (x.SellerName != null && x.SellerName.ToLower().Contains(phrase)) ||
                (x.BuyerName != null && x.BuyerName.ToLower().Contains(phrase))
            );
        }

        Query.OrderByDescending(x => x.InvoiceDate);
        Query.Take(limit);
    }

    private static List<FarmsInvoiceType> GetCompatibleInvoiceTypes(FarmsInvoiceType sourceType)
    {
        return sourceType switch
        {
            // Korekty mogą być powiązane z fakturami podstawowymi tego samego typu
            FarmsInvoiceType.Kor => [FarmsInvoiceType.Vat, FarmsInvoiceType.Upr],
            FarmsInvoiceType.KorZal => [FarmsInvoiceType.Zal],
            FarmsInvoiceType.KorRoz => [FarmsInvoiceType.Roz],
            FarmsInvoiceType.KorPef => [FarmsInvoiceType.VatPef, FarmsInvoiceType.VatPefSp],
            FarmsInvoiceType.KorVatRr => [FarmsInvoiceType.VatRr],
            FarmsInvoiceType.CostInvoiceCorrection => [FarmsInvoiceType.CostInvoice],

            // Zaliczka może być powiązana z fakturą końcową/rozliczeniową
            FarmsInvoiceType.Zal => [FarmsInvoiceType.Vat, FarmsInvoiceType.Roz],

            // Rozliczeniowa może być powiązana z zaliczkami
            FarmsInvoiceType.Roz => [FarmsInvoiceType.Zal],

            // Domyślnie - wszystkie typy (na wszelki wypadek)
            _ => []
        };
    }
}
