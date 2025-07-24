namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public interface IIrzPlusDisposition
{
    string ProducerNumber { get; }
    int Quantity { get; }
    string HenhouseCode { get; }
    string HenhouseName { get; }
    string ZdDzialalnosci { get; }
    DateOnly EventDate { get; }
}