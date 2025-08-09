namespace FarmsManager.Application.Models.Irzplus.Dispositions;

public interface IIrzPlusDisposition
{
    string DoDzialalnosci { get; }
    int Quantity { get; }
    string HenhouseCode { get; }
    string HenhouseName { get; }
    string ZDzialalnosci { get; }
    DateOnly EventDate { get; }
}