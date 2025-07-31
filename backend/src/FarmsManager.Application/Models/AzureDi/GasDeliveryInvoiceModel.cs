using FarmsManager.Shared.Attributes;
using FarmsManager.Shared.Models.AzureDi;

namespace FarmsManager.Application.Models.AzureDi;

public class GasDeliveryInvoiceModel
{
    [AzureDiField("InvoiceId")]
    public string InvoiceNumber { get; init; }
    
    [AzureDiField("vendorName", customField: true)]
    public string VendorName { get; init; }
    
    [AzureDiField("VendorTaxId")]
    public string VendorNip { get; init; }
    
    [AzureDiField("VendorAddress")]
    public string VendorAddress { get; init; }

    [AzureDiField("InvoiceDate")]
    public DateOnly? InvoiceDate { get; init; }
    
    [AzureDiField("InvoiceTotal")]
    public decimal? InvoiceTotal { get; init; }
    
    [AzureDiField("UnitPrice")]
    public decimal? UnitPrice { get; init; }
    
    [AzureDiField("Quantity", customField: true)]
    public string Quantity { get; init; }
    
    [AzureDiField("CustomerTaxId")]
    public string CustomerNip { get; init; }
    
    [AzureDiField("CustomerName")]
    public string CustomerName { get; init; }
    
    [AzureDiField("Items")]
    public List<InvoiceItem> Items { get; init; }
}