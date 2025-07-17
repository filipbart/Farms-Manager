using FarmsManager.Shared.Attributes;
using FarmsManager.Shared.Models.AzureDi;

namespace FarmsManager.Application.Models.AzureDi;

public class FeedDeliveryInvoiceModel
{
    [AzureDiField("vendorName", customField: true)]
    public string VendorName { get; init; }

    [AzureDiField("InvoiceDate")]
    public DateOnly? InvoiceDate { get; init; }
    
    [AzureDiField("DueDate", customField: true)]
    public DateOnly? DueDate { get; init; }
    
    [AzureDiField("InvoiceTotal")]
    public decimal? InvoiceTotal { get; init; }
    
    [AzureDiField("SubTotal")]
    public decimal? SubTotal { get; init; }
    
    [AzureDiField("TotalTax")]
    public decimal? VatAmount { get; init; }

    [AzureDiField("Items")]
    public List<InvoiceItem> Items { get; init; }
}