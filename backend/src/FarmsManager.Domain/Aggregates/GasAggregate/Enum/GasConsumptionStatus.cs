using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.GasAggregate.Enum;

public enum GasConsumptionStatus
{
    [Description("Aktywny")] Active,
    [Description("Anulowany")] Cancelled
}