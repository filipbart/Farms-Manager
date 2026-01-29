using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using KSeF.Client.Core.Models.Invoices.Common;

namespace FarmsManager.Infrastructure.Helpers.KSeF;

public static class InvoiceTypeMapper
{
    public static FarmsInvoiceType ToFarmsInvoiceType(this InvoiceType ksefType)
    {
        return ksefType switch
        {
            InvoiceType.Vat => FarmsInvoiceType.Vat,
            InvoiceType.Zal => FarmsInvoiceType.Zal,
            InvoiceType.Kor => FarmsInvoiceType.Kor,
            InvoiceType.Roz => FarmsInvoiceType.Roz,
            InvoiceType.Upr => FarmsInvoiceType.Upr,
            InvoiceType.KorZal => FarmsInvoiceType.KorZal,
            InvoiceType.KorRoz => FarmsInvoiceType.KorRoz,
            InvoiceType.VatPef => FarmsInvoiceType.VatPef,
            InvoiceType.VatPefSp => FarmsInvoiceType.VatPefSp,
            InvoiceType.KorPef => FarmsInvoiceType.KorPef,
            InvoiceType.VatRr => FarmsInvoiceType.VatRr,
            InvoiceType.KorVatRr => FarmsInvoiceType.KorVatRr,
            _ => FarmsInvoiceType.Other
        };
    }

    public static InvoiceType? ToKSeFInvoiceType(this FarmsInvoiceType farmsType)
    {
        return farmsType switch
        {
            FarmsInvoiceType.Vat => InvoiceType.Vat,
            FarmsInvoiceType.Zal => InvoiceType.Zal,
            FarmsInvoiceType.Kor => InvoiceType.Kor,
            FarmsInvoiceType.Roz => InvoiceType.Roz,
            FarmsInvoiceType.Upr => InvoiceType.Upr,
            FarmsInvoiceType.KorZal => InvoiceType.KorZal,
            FarmsInvoiceType.KorRoz => InvoiceType.KorRoz,
            FarmsInvoiceType.VatPef => InvoiceType.VatPef,
            FarmsInvoiceType.VatPefSp => InvoiceType.VatPefSp,
            FarmsInvoiceType.KorPef => InvoiceType.KorPef,
            FarmsInvoiceType.VatRr => InvoiceType.VatRr,
            FarmsInvoiceType.KorVatRr => InvoiceType.KorVatRr,
            FarmsInvoiceType.CostInvoice => null,
            FarmsInvoiceType.CostInvoiceCorrection => null,
            FarmsInvoiceType.Other => null,
            _ => null
        };
    }
}
