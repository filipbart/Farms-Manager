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
    PaymentStatus,
    PaymentDueDate,
    DateCreatedUtc
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
    /// Filtrowanie po nazwie nabywcy
    /// </summary>
    public string BuyerName { get; init; }
    
    /// <summary>
    /// Filtrowanie po nazwie sprzedawcy
    /// </summary>
    public string SellerName { get; init; }
    
    /// <summary>
    /// Filtrowanie po numerze faktury
    /// </summary>
    public string InvoiceNumber { get; init; }
    
    /// <summary>
    /// Data wystawienia - początek zakresu
    /// </summary>
    public DateOnly? InvoiceDateFrom { get; init; }
    
    /// <summary>
    /// Data wystawienia - koniec zakresu
    /// </summary>
    public DateOnly? InvoiceDateTo { get; init; }
    
    /// <summary>
    /// Termin płatności - początek zakresu
    /// </summary>
    public DateOnly? PaymentDueDateFrom { get; init; }
    
    /// <summary>
    /// Termin płatności - koniec zakresu
    /// </summary>
    public DateOnly? PaymentDueDateTo { get; init; }
    
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
    public List<KSeFPaymentStatus>? PaymentStatuses { get; init; }
    
    /// <summary>
    /// Filtrowanie po module
    /// </summary>
    public ModuleType? ModuleType { get; init; }
    
    /// <summary>
    /// Filtrowanie po przypisanym pracowniku
    /// </summary>
    public Guid? AssignedUserId { get; init; }

    /// <summary>
    /// Filtrowanie po lokalizacji (fermie)
    /// </summary>
    public Guid? FarmId { get; init; }

    /// <summary>
    /// Wykluczenia - filtrowanie po NIP i nazwach sprzedawców/kupców
    /// </summary>
    public string Exclusions { get; init; }
}

public record GetKSeFInvoicesFromDbQuery(GetKSeFInvoicesFromDbQueryFilters Filters) 
    : IRequest<BaseResponse<GetKSeFInvoicesFromDbQueryResponse>>;
