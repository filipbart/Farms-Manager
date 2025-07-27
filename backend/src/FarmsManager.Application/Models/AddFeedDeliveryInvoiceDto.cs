namespace FarmsManager.Application.Models;

public class AddFeedDeliveryInvoiceDto
{
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? HenhouseId { get; set; }
    public string InvoiceNumber { get; set; }
    public string BankAccountNumber { get; set; }
    public string VendorName { get; set; }
    public string ItemName { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public DateOnly? InvoiceDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? InvoiceTotal { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? VatAmount { get; set; }
    public string Comment { get; set; }
}