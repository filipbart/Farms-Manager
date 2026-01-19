using KSeF.Client.Core.Models.Invoices.Common;

namespace FarmsManager.Application.Models.KSeF;

/// <summary>
/// Odpowiedź z listą faktur z KSeF
/// </summary>
public class KSeFInvoicesResponse
{
    /// <summary>
    /// Lista faktur
    /// </summary>
    public List<KSeFInvoiceItem> Invoices { get; set; } = [];

    /// <summary>
    /// Całkowita liczba faktur spełniających kryteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Numer strony
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Rozmiar strony
    /// </summary>
    public int PageSize { get; set; }
}

/// <summary>
/// Kierunek faktury dla synchronizacji
/// </summary>
public enum KSeFInvoiceItemDirection
{
    Sales,
    Purchase
}

/// <summary>
/// Model faktury do synchronizacji z KSeF
/// </summary>
public class KSeFInvoiceSyncItem
{
    public string KsefNumber { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public string SellerNip { get; set; }
    public string SellerName { get; set; }
    public string BuyerNip { get; set; }
    public string BuyerName { get; set; }
    public KSeFInvoiceItemDirection Direction { get; set; }
    public InvoiceType InvoiceType { get; set; }
}

/// <summary>
/// Podstawowe informacje o fakturze z KSeF
/// </summary>
public class KSeFInvoiceItem
{
    /// <summary>
    /// Numer referencyjny faktury w KSeF
    /// </summary>
    public string ReferenceNumber { get; set; }

    /// <summary>
    /// Numer faktury
    /// </summary>
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// Data wystawienia faktury
    /// </summary>
    public DateTimeOffset InvoiceDate { get; set; }

    /// <summary>
    /// Kwota brutto faktury
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Kwota netto faktury
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Kwota VAT
    /// </summary>
    public decimal VatAmount { get; set; }

    /// <summary>
    /// NIP sprzedawcy
    /// </summary>
    public string SellerNip { get; set; }

    /// <summary>
    /// Nazwa sprzedawcy
    /// </summary>
    public string SellerName { get; set; }

    /// <summary>
    /// NIP nabywcy
    /// </summary>
    public string BuyerNip { get; set; }

    /// <summary>
    /// Nazwa nabywcy
    /// </summary>
    public string BuyerName { get; set; }

    /// <summary>
    /// Data otrzymania faktury w KSeF
    /// </summary>
    public DateTimeOffset ReceivedDate { get; set; }
}