using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public class FallenStocksIrzPlusDisposition(FallenStockEntity fallenStock) : IIrzPlusDisposition
{
    public FallenStockEntity FallenStock { get; } = fallenStock;
    public string DoDzialalnosci => FallenStock.UtilizationPlant.IrzNumber;
    public int Quantity => FallenStock.Quantity;
    public string HenhouseCode => FallenStock.Henhouse.Code;
    public string HenhouseName => FallenStock.Henhouse.Name;
    public string ZDzialalnosci => FallenStock.Farm.ProducerNumber;
    public DateOnly EventDate => FallenStock.Date;
}