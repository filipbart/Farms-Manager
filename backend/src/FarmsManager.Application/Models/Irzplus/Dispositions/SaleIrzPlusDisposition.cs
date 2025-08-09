using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;

namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public class SaleIrzPlusDisposition(SaleEntity sale) : IIrzPlusDisposition
{
    public SaleEntity Sale { get; } = sale;

    public string DoDzialalnosci => Sale.Slaughterhouse.ProducerNumber;
    public int Quantity => Sale.Quantity;
    public string HenhouseCode => Sale.Henhouse.Code;
    public string HenhouseName => Sale.Henhouse.Name;
    public string ZDzialalnosci => Sale.Farm.ProducerNumber;
    public DateOnly EventDate => Sale.SaleDate;
}