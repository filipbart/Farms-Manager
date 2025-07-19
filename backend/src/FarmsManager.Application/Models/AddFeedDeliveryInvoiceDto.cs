namespace FarmsManager.Application.Models;

public class AddFeedDeliveryInvoiceDto
{
    public Guid? FarmId { get; init; }
    public Guid? CycleId { get; init; }
    public Guid? HenhouseId { get; init; }
    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public string VendorName { get; init; }
    public string ItemName { get; init; }
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public DateOnly? InvoiceDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public decimal? InvoiceTotal { get; init; }
    public decimal? SubTotal { get; init; }
    public decimal? VatAmount { get; init; }
}