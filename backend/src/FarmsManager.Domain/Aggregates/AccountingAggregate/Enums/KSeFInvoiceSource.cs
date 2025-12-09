using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

/// <summary>
/// Źródło faktury
/// </summary>
public enum KSeFInvoiceSource
{
    [Description("KSeF")] KSeF,

    [Description("Poza KSeF")] Manual
}
