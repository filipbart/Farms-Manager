using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.KSeF;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting;

public enum GetKSeFInvoicesOrderBy
{
    InvoiceDate,
    InvoiceNumber,
    GrossAmount,
    SellerName,
    BuyerName,
    ReceivedDate
}

public record GetKSeFInvoicesQueryFilters : OrderedPaginationParams<GetKSeFInvoicesOrderBy>
{
    /// <summary>
    /// NIP kontrahenta (dla faktur sprzedaży to nabywca, dla zakupu to sprzedawca)
    /// </summary>
    public string SubjectNip { get; init; }
    
    /// <summary>
    /// Data początkowa zakresu wyszukiwania
    /// </summary>
    public DateTime? DateFrom { get; init; }
    
    /// <summary>
    /// Data końcowa zakresu wyszukiwania
    /// </summary>
    public DateTime? DateTo { get; init; }
    
    /// <summary>
    /// Typ faktury: "sales" (sprzedaż) lub "purchase" (zakup)
    /// </summary>
    public KSeFInvoiceType? InvoiceType { get; init; }
}

public class KSeFInvoiceDto
{
    public string ReferenceNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTimeOffset InvoiceDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string SellerNip { get; set; }
    public string SellerName { get; set; }
    public string BuyerNip { get; set; }
    public string BuyerName { get; set; }
    public DateTimeOffset ReceivedDate { get; set; }
}

public class GetKSeFInvoicesQueryResponse : PaginationModel<KSeFInvoiceDto>;

public record GetKSeFInvoicesQuery(GetKSeFInvoicesQueryFilters Filters) 
    : IRequest<BaseResponse<GetKSeFInvoicesQueryResponse>>;

public class GetKSeFInvoicesQueryHandler 
    : IRequestHandler<GetKSeFInvoicesQuery, BaseResponse<GetKSeFInvoicesQueryResponse>>
{
    private readonly IKSeFService _ksefService;

    public GetKSeFInvoicesQueryHandler(IKSeFService ksefService)
    {
        _ksefService = ksefService;
    }

    public async Task<BaseResponse<GetKSeFInvoicesQueryResponse>> Handle(
        GetKSeFInvoicesQuery query, 
        CancellationToken cancellationToken)
    {
        var filters = query.Filters;
        
        // Przygotowanie requestu do KSeF
        var ksefRequest = new KSeFInvoicesRequest
        {
            SubjectNip = filters.SubjectNip,
            DateFrom = filters.DateFrom,
            DateTo = filters.DateTo,
            InvoiceType = filters.InvoiceType ?? KSeFInvoiceType.Purchase,
            PageNumber = filters.Page + 1, // Page jest 0-indexed, PageNumber jest 1-indexed
            PageSize = filters.PageSize
        };

        // Pobranie faktur z KSeF
        var ksefResponse = await _ksefService.GetInvoicesAsync(ksefRequest, cancellationToken);

        // Mapowanie na DTO
        var invoiceDtos = ksefResponse.Invoices.Select(inv => new KSeFInvoiceDto
        {
            ReferenceNumber = inv.ReferenceNumber,
            InvoiceNumber = inv.InvoiceNumber,
            InvoiceDate = inv.InvoiceDate,
            GrossAmount = inv.GrossAmount,
            NetAmount = inv.NetAmount,
            VatAmount = inv.VatAmount,
            SellerNip = inv.SellerNip,
            SellerName = inv.SellerName,
            BuyerNip = inv.BuyerNip,
            BuyerName = inv.BuyerName,
            ReceivedDate = inv.ReceivedDate
        }).ToList();

        // Sortowanie lokalne (jeśli potrzebne)
        invoiceDtos = ApplyOrdering(invoiceDtos, filters);

        return BaseResponse.CreateResponse(new GetKSeFInvoicesQueryResponse
        {
            Items = invoiceDtos,
            TotalRows = ksefResponse.TotalCount
        });
    }

    private List<KSeFInvoiceDto> ApplyOrdering(
        List<KSeFInvoiceDto> invoices, 
        GetKSeFInvoicesQueryFilters filters)
    {
        var ordered = filters.OrderBy switch
        {
            GetKSeFInvoicesOrderBy.InvoiceNumber => filters.IsDescending
                ? invoices.OrderByDescending(x => x.InvoiceNumber)
                : invoices.OrderBy(x => x.InvoiceNumber),
            GetKSeFInvoicesOrderBy.GrossAmount => filters.IsDescending
                ? invoices.OrderByDescending(x => x.GrossAmount)
                : invoices.OrderBy(x => x.GrossAmount),
            GetKSeFInvoicesOrderBy.SellerName => filters.IsDescending
                ? invoices.OrderByDescending(x => x.SellerName)
                : invoices.OrderBy(x => x.SellerName),
            GetKSeFInvoicesOrderBy.BuyerName => filters.IsDescending
                ? invoices.OrderByDescending(x => x.BuyerName)
                : invoices.OrderBy(x => x.BuyerName),
            GetKSeFInvoicesOrderBy.ReceivedDate => filters.IsDescending
                ? invoices.OrderByDescending(x => x.ReceivedDate)
                : invoices.OrderBy(x => x.ReceivedDate),
            GetKSeFInvoicesOrderBy.InvoiceDate or _ => filters.IsDescending
                ? invoices.OrderByDescending(x => x.InvoiceDate)
                : invoices.OrderBy(x => x.InvoiceDate)
        };

        return ordered.ToList();
    }
}