namespace FarmsManager.Application.Models.KSeF;

/// <summary>
/// Odpowiedź z listą faktur z KSeF
/// </summary>
public class KSeFInvoicesResponse
{
    /// <summary>
    /// Lista faktur
    /// </summary>
    public List<KSeFInvoiceItem> Invoices { get; set; } = new();

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