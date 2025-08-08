using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;

namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public class FallenStocksIrzPlusDisposition(FallenStockEntity fallenStock) : IIrzPlusDisposition
{
    public FallenStockEntity FallenStock { get; } = fallenStock;
    public string ProducerNumber { get; }//todo
    public int Quantity => FallenStock.Quantity;
    public string HenhouseCode => FallenStock.Henhouse.Code;
    public string HenhouseName => FallenStock.Henhouse.Name;
    public string ZdDzialalnosci { get; }
    public DateOnly EventDate => FallenStock.Date;
}