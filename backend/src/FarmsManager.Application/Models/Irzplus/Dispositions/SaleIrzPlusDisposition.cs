using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public class SaleIrzPlusDisposition(SaleEntity sale) : IIrzPlusDisposition
{
    public SaleEntity Sale { get; } = sale;

    public string ProducerNumber => Sale.Farm.ProducerNumber;
    public int Quantity => Sale.Quantity;
    public string HenhouseCode => Sale.Henhouse.Code;
    public string HenhouseName => Sale.Henhouse.Name;
    public string ZdDzialalnosci => Sale.Farm.ProducerNumber;
    public DateOnly EventDate => Sale.SaleDate;
}