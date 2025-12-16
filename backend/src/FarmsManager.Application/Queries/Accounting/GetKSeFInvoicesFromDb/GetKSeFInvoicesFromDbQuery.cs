using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;

public enum KSeFInvoicesFromDbOrderBy
{
    InvoiceDate,
    InvoiceNumber,
    GrossAmount,
    SellerName,
    BuyerName,
    KSeFNumber,
    Status,
    PaymentStatus
}

/// <summary>
/// Typ faktury w systemie (kierunek)
/// </summary>
public enum InvoiceDirectionFilter
{
    /// <summary>
    /// Wszystkie faktury
    /// </summary>
    All,
    
    /// <summary>
    /// Faktury sprzedaży (wystawione przez nas)
    /// </summary>
    Sales,

    /// <summary>
    /// Faktury zakupu (wystawione dla nas)
    /// </summary>
    Purchase
}

/// <summary>
/// Źródło faktury
/// </summary>
public enum InvoiceSourceFilter
{
    /// <summary>
    /// Wszystkie źródła
    /// </summary>
    All,
    
    /// <summary>
    /// Faktury z KSeF
    /// </summary>
    KSeF,
    
    /// <summary>
    /// Faktury dodane manualnie (poza KSeF)
    /// </summary>
    Manual
}

public record GetKSeFInvoicesFromDbQueryFilters : OrderedPaginationParams<KSeFInvoicesFromDbOrderBy>
{
    /// <summary>
    /// Typ/kierunek faktury (sprzedaż/zakup)
    /// </summary>
    public InvoiceDirectionFilter? InvoiceType { get; init; }
    
    /// <summary>
    /// Źródło faktury (KSeF/Manual)
    /// </summary>
    public InvoiceSourceFilter? Source { get; init; }
    
    /// <summary>
    /// Data początkowa zakresu wyszukiwania
    /// </summary>
    public DateOnly? DateFrom { get; init; }
    
    /// <summary>
    /// Data końcowa zakresu wyszukiwania
    /// </summary>
    public DateOnly? DateTo { get; init; }
    
    /// <summary>
    /// Wyszukiwanie po tekście (numer faktury, NIP, nazwa kontrahenta)
    /// </summary>
    public string SearchQuery { get; init; }
    
    /// <summary>
    /// Filtrowanie po statusie faktury
    /// </summary>
    public KSeFInvoiceStatus? Status { get; init; }
    
    /// <summary>
    /// Filtrowanie po statusie płatności
    /// </summary>
    public KSeFPaymentStatus? PaymentStatus { get; init; }
    
    /// <summary>
    /// Filtrowanie po module
    /// </summary>
    public ModuleType? ModuleType { get; init; }
}

public record GetKSeFInvoicesFromDbQuery(GetKSeFInvoicesFromDbQueryFilters Filters) 
    : IRequest<BaseResponse<GetKSeFInvoicesFromDbQueryResponse>>;
