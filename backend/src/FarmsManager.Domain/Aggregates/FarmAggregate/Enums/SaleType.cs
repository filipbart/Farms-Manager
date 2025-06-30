using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.FarmAggregate.Enums;

public enum SaleType
{
    [Description("Ubiórka")] PartSale,

    [Description("Całkowita")] TotalSale
}