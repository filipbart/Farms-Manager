using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;

public class KSeFInvoiceFromDbDto
{
    public Guid Id { get; set; }
    public string KSeFNumber { get; set; }
    public string Nip { get; set; }
    public string BuyerName { get; set; }
    public string BuyerNip { get; set; }
    public string SellerName { get; set; }
    public string SellerNip { get; set; }
    public string InvoiceType { get; set; }
    public int? CycleIdentifier { get; set; }
    public int? CycleYear { get; set; }
    public string Source { get; set; }
    public string Location { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public string ModuleType { get; set; }
    public string VatDeductionType { get; set; }
    public string Status { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentType { get; set; }
    public string Comment { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public bool HasXml { get; set; }
    public bool HasPdf { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string AssignedUserName { get; set; }
}

public class GetKSeFInvoicesFromDbQueryResponse : PaginationModel<KSeFInvoiceFromDbDto>;
