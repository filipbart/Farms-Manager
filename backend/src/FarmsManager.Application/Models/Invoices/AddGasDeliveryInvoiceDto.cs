namespace FarmsManager.Application.Models.Invoices;

public class AddGasDeliveryInvoiceDto
{
    public Guid? FarmId { get; set; }
    public Guid? ContractorId { get; set; }
    public string ContractorName { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly? InvoiceDate { get; set; }
    public decimal? InvoiceTotal { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Quantity { get; set; }
    public string Comment { get; set; }
}