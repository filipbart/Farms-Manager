using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.FallenStockAggregate.Enums;

public enum FallenStockType
{
    [Description("Padnięcia/stłuczki")]
    FallCollision,
    
    [Description("Zamknięcie cyklu")]
    EndCycle
}