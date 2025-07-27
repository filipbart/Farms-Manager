namespace FarmsManager.Application.Models;

public class AddExpenseProductionInvoiceDto
{
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? ContractorId { get; set; }
    public Guid? ExpenseTypeId { get; set; }
    public string InvoiceNumber { get; set; }
    public string ExpenseTypeName { get; set; }
    public decimal? InvoiceTotal { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? VatAmount { get; set; }
    public DateOnly? InvoiceDate { get; set; }
}