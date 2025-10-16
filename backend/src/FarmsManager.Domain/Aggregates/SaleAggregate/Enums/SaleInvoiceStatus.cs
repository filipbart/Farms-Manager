using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.SaleAggregate.Enums;

public enum SaleInvoiceStatus
{
    [Description("Niezrealizowany")] Unrealized,

    [Description("Zrealizowany")] Realized
}
