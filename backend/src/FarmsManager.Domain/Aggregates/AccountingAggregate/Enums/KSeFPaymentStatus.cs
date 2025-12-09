using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum KSeFPaymentStatus
{
    [Description("Nieopłacona")] Unpaid,

    [Description("Częściowo opłacona")] PartiallyPaid,

    [Description("Wstrzymana")] Suspended,

    [Description("Opłacona gotówką")] PaidCash,

    [Description("Opłacona przelewem")] PaidTransfer
}