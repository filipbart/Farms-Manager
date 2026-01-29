using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

/// <summary>
/// Typ powiązania między fakturami
/// </summary>
public enum InvoiceRelationType
{
    /// <summary>
    /// Korekta → Faktura pierwotna
    /// </summary>
    [Description("Korekta do faktury")] CorrectionToOriginal,

    /// <summary>
    /// Faktura zaliczkowa → Faktura końcowa
    /// </summary>
    [Description("Zaliczka do faktury końcowej")] AdvanceToFinal,

    /// <summary>
    /// Faktura rozliczeniowa → Faktury zaliczkowe
    /// </summary>
    [Description("Rozliczenie zaliczek")] FinalToAdvances,
}
