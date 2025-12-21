using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using KSeF.Client.Core.Models.Invoices.Common;
using MediatR;

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

    private static string GetInvoiceTypeDescription(InvoiceType type) => type switch
    {
        InvoiceType.Vat => "(FA) Podstawowa",
        InvoiceType.Zal => "(FA) Zaliczkowa",
        InvoiceType.Kor => "(FA) Korygująca",
        InvoiceType.Roz => "(FA) Rozliczeniowa",
        InvoiceType.Upr => "(FA) Uproszczona",
        InvoiceType.KorZal => "(FA) Korygująca fakturę zaliczkową",
        InvoiceType.KorRoz => "(FA) Korygująca fakturę rozliczeniową",
        InvoiceType.VatPef => "(PEF) Podstawowa",
        InvoiceType.VatPefSp => "(PEF) Specjalizowana",
        InvoiceType.KorPef => "(PEF) Korygująca",
        InvoiceType.VatRr => "(RR) Podstawowa",
        InvoiceType.KorVatRr => "(RR) Korygująca",
        _ => type.ToString()
    };
}

public class GetSourceInvoiceSpec : Specification<KSeFInvoiceEntity>, ISingleResultSpecification<KSeFInvoiceEntity>
{
    public GetSourceInvoiceSpec(Guid invoiceId)
    {
        Query.Where(x => x.Id == invoiceId && x.DateDeletedUtc == null);
    }
}

public class GetLinkableInvoicesSpec : Specification<KSeFInvoiceEntity>
{
    public GetLinkableInvoicesSpec(
        Guid sourceInvoiceId,
        InvoiceType sourceInvoiceType,
        string sellerNip,
        string buyerNip,
        string searchPhrase,
        int limit)
    {
        // Exclude the source invoice itself and deleted invoices
        Query.Where(x => x.Id != sourceInvoiceId && x.DateDeletedUtc == null);

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

    private static List<InvoiceType> GetCompatibleInvoiceTypes(InvoiceType sourceType)
    {
        return sourceType switch
        {
            // Korekty mogą być powiązane z fakturami podstawowymi tego samego typu
            InvoiceType.Kor => [InvoiceType.Vat, InvoiceType.Upr],
            InvoiceType.KorZal => [InvoiceType.Zal],
            InvoiceType.KorRoz => [InvoiceType.Roz],
            InvoiceType.KorPef => [InvoiceType.VatPef, InvoiceType.VatPefSp],
            InvoiceType.KorVatRr => [InvoiceType.VatRr],

            // Zaliczka może być powiązana z fakturą końcową/rozliczeniową
            InvoiceType.Zal => [InvoiceType.Vat, InvoiceType.Roz],

            // Rozliczeniowa może być powiązana z zaliczkami
            InvoiceType.Roz => [InvoiceType.Zal],

            // Domyślnie - wszystkie typy (na wszelki wypadek)
            _ => []
        };
    }
}
