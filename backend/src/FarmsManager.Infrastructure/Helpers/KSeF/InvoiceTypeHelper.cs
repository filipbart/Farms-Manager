using KSeF.Client.Core.Models.Invoices.Common;

namespace FarmsManager.Infrastructure.Helpers.KSeF;

public static class InvoiceTypeHelper
{
    /// <summary>
    /// Zwraca opis typu faktury w języku polskim.
    /// </summary>
    public static string ToDescription(this InvoiceType type) => type switch
    {
        // (FA) Faktura
        InvoiceType.Vat => "(FA) Podstawowa",
        InvoiceType.Zal => "(FA) Zaliczkowa",
        InvoiceType.Kor => "(FA) Korygująca",
        InvoiceType.Roz => "(FA) Rozliczeniowa",
        InvoiceType.Upr => "(FA) Uproszczona",
        InvoiceType.KorZal => "(FA) Korygująca fakturę zaliczkową",
        InvoiceType.KorRoz => "(FA) Korygująca fakturę rozliczeniową",

        // (PEF) Platforma Elektronicznego Fakturowania
        InvoiceType.VatPef => "(PEF) Podstawowa",
        InvoiceType.VatPefSp => "(PEF) Specjalizowana",
        InvoiceType.KorPef => "(PEF) Korygująca",

        // (RR) Faktura RR
        InvoiceType.VatRr => "(RR) Podstawowa",
        InvoiceType.KorVatRr => "(RR) Korygująca",

        // Domyślna ścieżka na wypadek rozszerzenia enum w przyszłości
        _ => type.ToString()
    };
}