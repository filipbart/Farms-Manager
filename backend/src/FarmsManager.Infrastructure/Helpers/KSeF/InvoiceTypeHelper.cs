using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Infrastructure.Helpers.KSeF;

public static class InvoiceTypeHelper
{
    /// <summary>
    /// Zwraca opis typu faktury w języku polskim.
    /// </summary>
    public static string ToDescription(this FarmsInvoiceType type) => type switch
    {
        // (FA) Faktura
        FarmsInvoiceType.Vat => "(FA) Podstawowa",
        FarmsInvoiceType.Zal => "(FA) Zaliczkowa",
        FarmsInvoiceType.Kor => "(FA) Korygująca",
        FarmsInvoiceType.Roz => "(FA) Rozliczeniowa",
        FarmsInvoiceType.Upr => "(FA) Uproszczona",
        FarmsInvoiceType.KorZal => "(FA) Korygująca fakturę zaliczkową",
        FarmsInvoiceType.KorRoz => "(FA) Korygująca fakturę rozliczeniową",

        // (PEF) Platforma Elektronicznego Fakturowania
        FarmsInvoiceType.VatPef => "(PEF) Podstawowa",
        FarmsInvoiceType.VatPefSp => "(PEF) Specjalizowana",
        FarmsInvoiceType.KorPef => "(PEF) Korygująca",

        // (RR) Faktura RR
        FarmsInvoiceType.VatRr => "(RR) Podstawowa",
        FarmsInvoiceType.KorVatRr => "(RR) Korygująca",

        // Nowe typy
        FarmsInvoiceType.CostInvoice => "(RK) Rachunek kosztowy",
        FarmsInvoiceType.CostInvoiceCorrection => "(RK-KOR) Korekta rachunku kosztowego",
        FarmsInvoiceType.Other => "Inny dokument",

        // Domyślna ścieżka na wypadek rozszerzenia enum w przyszłości
        _ => type.ToString()
    };
}