using FarmsManager.Shared.Attributes;

namespace FarmsManager.Shared.Models.AzureDi;

public class InvoiceItem
{
    [AzureDiField("Description")]
    public string Name { get; init; }
    
    [AzureDiField("Quantity")]
    public decimal? Quantity { get; init; }
    
    [AzureDiField("UnitPrice", customField: true)]
    public decimal? UnitPrice { get; init; }
}