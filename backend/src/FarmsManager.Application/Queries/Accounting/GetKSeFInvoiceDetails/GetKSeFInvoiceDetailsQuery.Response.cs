namespace FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;

public class KSeFInvoiceDetailsDto
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
    public Guid? CycleId { get; set; }
    public string Source { get; set; }
    public string Location { get; set; }
    public string FilePath { get; set; }
    public Guid? FarmId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly InvoiceDate { get; set; }
    public DateOnly? PaymentDueDate { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public string ModuleType { get; set; }
    public string VatDeductionType { get; set; }
    public string Status { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentType { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public bool HasXml { get; set; }
    public bool HasPdf { get; set; }
    public string InvoiceXml { get; set; }
    public string Comment { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string AssignedUserName { get; set; }
    public Guid? AssignedEntityInvoiceId { get; set; }
    public string RelatedInvoiceNumber { get; set; }
    
    // Gas module fields
    public decimal? GasQuantity { get; set; }
    public decimal? GasUnitPrice { get; set; }
    public decimal? GasInvoiceTotal { get; set; }
    public Guid? GasContractorId { get; set; }
    
    // Feeds module fields
    public Guid? FeedHenhouseId { get; set; }
    public string FeedItemName { get; set; }
    public decimal? FeedQuantity { get; set; }
    public decimal? FeedUnitPrice { get; set; }
    public string FeedVendorName { get; set; }
    public string FeedBankAccountNumber { get; set; }
    
    // Expenses module fields
    public Guid? ExpenseContractorId { get; set; }
    public Guid? ExpenseTypeId { get; set; }
    
    // Sales module fields
    public Guid? SaleSlaughterhouseId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public int AttachmentsCount { get; set; }
    public int AuditLogsCount { get; set; }
}