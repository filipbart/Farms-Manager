using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.FeedAggregate.Enums;

public enum FeedPaymentStatus
{
    [Description("Niezrealizowany")] Unrealized,

    [Description("Zrealizowany")] Realized
}