namespace FarmsManager.Application.Models.KSeF;

/// <summary>
/// Request do wyszukiwania faktur w KSeF
/// </summary>
public class KSeFInvoicesRequest
{
    /// <summary>
    /// NIP podmiotu (kontrahenta) - dla faktur sprzedaży to nabywca, dla zakupu to sprzedawca
    /// </summary>
    public string SubjectNip { get; set; }

    /// <summary>
    /// Data początkowa zakresu wyszukiwania (data wystawienia faktury)
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Data końcowa zakresu wyszukiwania (data wystawienia faktury)
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Typ faktury: "sales" (sprzedaż) lub "purchase" (zakup)
    /// </summary>
    public KSeFInvoiceType InvoiceType { get; set; }

    /// <summary>
    /// Numer strony dla paginacji
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Rozmiar strony dla paginacji
    /// </summary>
    public int PageSize { get; set; } = 100;
}

/// <summary>
/// Typ faktury w KSeF
/// </summary>
public enum KSeFInvoiceType
{
    /// <summary>
    /// Faktury sprzedaży (wystawione przez nas)
    /// </summary>
    Sales,

    /// <summary>
    /// Faktury zakupu (wystawione dla nas)
    /// </summary>
    Purchase
}