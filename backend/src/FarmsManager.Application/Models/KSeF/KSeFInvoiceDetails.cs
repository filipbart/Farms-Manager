namespace FarmsManager.Application.Models.KSeF;

/// <summary>
/// Szczegółowe informacje o fakturze z KSeF
/// </summary>
public class KSeFInvoiceDetails
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
    public DateTime InvoiceDate { get; set; }
    
    /// <summary>
    /// Data sprzedaży
    /// </summary>
    public DateTime? SaleDate { get; set; }
    
    /// <summary>
    /// Termin płatności
    /// </summary>
    public DateTime? DueDate { get; set; }
    
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
    /// Waluta faktury
    /// </summary>
    public string Currency { get; set; }
    
    /// <summary>
    /// NIP sprzedawcy
    /// </summary>
    public string SellerNip { get; set; }
    
    /// <summary>
    /// Nazwa sprzedawcy
    /// </summary>
    public string SellerName { get; set; }
    
    /// <summary>
    /// Adres sprzedawcy
    /// </summary>
    public string SellerAddress { get; set; }
    
    /// <summary>
    /// NIP nabywcy
    /// </summary>
    public string BuyerNip { get; set; }
    
    /// <summary>
    /// Nazwa nabywcy
    /// </summary>
    public string BuyerName { get; set; }
    
    /// <summary>
    /// Adres nabywcy
    /// </summary>
    public string BuyerAddress { get; set; }
    
    /// <summary>
    /// Data otrzymania faktury w KSeF
    /// </summary>
    public DateTime ReceivedDate { get; set; }
    
    /// <summary>
    /// Pozycje faktury
    /// </summary>
    public List<KSeFInvoiceLineItem> LineItems { get; set; } = [];
}

/// <summary>
/// Pozycja faktury
/// </summary>
public class KSeFInvoiceLineItem
{
    /// <summary>
    /// Numer pozycji
    /// </summary>
    public int LineNumber { get; set; }
    
    /// <summary>
    /// Nazwa towaru/usługi
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Ilość
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Jednostka miary
    /// </summary>
    public string Unit { get; set; }
    
    /// <summary>
    /// Cena jednostkowa netto
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Wartość netto
    /// </summary>
    public decimal NetAmount { get; set; }
    
    /// <summary>
    /// Stawka VAT (%)
    /// </summary>
    public decimal VatRate { get; set; }
    
    /// <summary>
    /// Kwota VAT
    /// </summary>
    public decimal VatAmount { get; set; }
    
    /// <summary>
    /// Wartość brutto
    /// </summary>
    public decimal GrossAmount { get; set; }
}
