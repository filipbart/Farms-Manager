using FarmsManager.Shared.Attributes;

namespace FarmsManager.Application.Models.AzureDi;

/// <summary>
/// Model faktury zaczytywany przez Azure Document Intelligence
/// </summary>
public class AccountingInvoiceModel
{
    [AzureDiField("InvoiceId")]
    public string InvoiceNumber { get; init; }

    [AzureDiField("InvoiceDate")]
    public DateOnly? InvoiceDate { get; init; }

    [AzureDiField("DueDate", customField: true)]
    public DateOnly? DueDate { get; init; }

    [AzureDiField("VendorName")]
    public string SellerName { get; init; }

    [AzureDiField("VendorTaxId")]
    public string SellerNip { get; init; }

    [AzureDiField("VendorAddress")]
    public string SellerAddress { get; init; }

    [AzureDiField("CustomerName")]
    public string BuyerName { get; init; }

    [AzureDiField("CustomerTaxId")]
    public string BuyerNip { get; init; }

    [AzureDiField("CustomerAddress")]
    public string BuyerAddress { get; init; }

    [AzureDiField("InvoiceTotal")]
    public decimal? GrossAmount { get; init; }

    [AzureDiField("SubTotal")]
    public decimal? NetAmount { get; init; }

    [AzureDiField("TotalTax")]
    public decimal? VatAmount { get; init; }

    [AzureDiField("BankAccountNumber", customField: true)]
    public string BankAccountNumber { get; init; }
}
