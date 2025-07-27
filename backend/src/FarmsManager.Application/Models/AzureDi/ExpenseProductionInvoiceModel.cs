using FarmsManager.Shared.Attributes;

namespace FarmsManager.Application.Models.AzureDi;

public class ExpenseProductionInvoiceModel
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
    
    [AzureDiField("SubTotal")]
    public decimal? SubTotal { get; init; }
    
    [AzureDiField("TotalTax")]
    public decimal? VatAmount { get; init; }
    
    [AzureDiField("CustomerTaxId")]
    public string CustomerNip { get; init; }
    
    [AzureDiField("CustomerName")]
    public string CustomerName { get; init; }
}