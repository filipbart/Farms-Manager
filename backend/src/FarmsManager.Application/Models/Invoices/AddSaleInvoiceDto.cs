namespace FarmsManager.Application.Models.Invoices;

public class AddSaleInvoiceDto
{
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? SlaughterhouseId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly? InvoiceDate { get; set; }
    public DateOnly? DueDate { get; set; }   
    public decimal? InvoiceTotal { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? VatAmount { get; set; }
}