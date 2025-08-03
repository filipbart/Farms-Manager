using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.SaleAggregate.Enums;

public enum SaleType
{
    [Description("Ubiórka")] PartSale,

    [Description("Całkowita")] TotalSale
}