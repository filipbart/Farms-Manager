using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;

namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public class InsertionIrzPlusDisposition(InsertionEntity insertion) : IIrzPlusDisposition
{
    public InsertionEntity Insertion { get; } = insertion;

    public string DoDzialalnosci => Insertion.Farm.ProducerNumber;
    public int Quantity => Insertion.Quantity;
    public string HenhouseCode => Insertion.Henhouse.Code;
    public string HenhouseName => Insertion.Henhouse.Name;
    public string ZDzialalnosci => Insertion.Hatchery.ProducerNumber;
    public DateOnly EventDate => Insertion.InsertionDate;
}