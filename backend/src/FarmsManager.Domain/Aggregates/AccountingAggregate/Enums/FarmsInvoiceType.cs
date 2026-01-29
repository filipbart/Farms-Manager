using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum FarmsInvoiceType
{
    [Description("(FA) Podstawowa")] Vat,

    [Description("(FA) Zaliczkowa")] Zal,

    [Description("(FA) Korygująca")] Kor,

    [Description("(FA) Rozliczeniowa")] Roz,

    [Description("(FA) Uproszczona")] Upr,

    [Description("(FA) Korygująca fakturę zaliczkową")]
    KorZal,

    [Description("(FA) Korygująca fakturę rozliczeniową")]
    KorRoz,

    [Description("(PEF) Podstawowa")] VatPef,

    [Description("(PEF) Specjalizowana")] VatPefSp,

    [Description("(PEF) Korygująca")] KorPef,

    [Description("(RR) Podstawowa")] VatRr,

    [Description("(RR) Korygująca")] KorVatRr,

    [Description("(RK) Rachunek kosztowy")] CostInvoice,

    [Description("(RK-KOR) Korekta rachunku kosztowego")]
    CostInvoiceCorrection,

    [Description("Inny dokument")] Other,
}
