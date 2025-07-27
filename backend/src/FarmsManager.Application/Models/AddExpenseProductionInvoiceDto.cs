namespace FarmsManager.Application.Models;

public class AddExpenseProductionInvoiceDto
{
    public Guid? FarmId { get; init; }
    public Guid? CycleId { get; init; }
    public string InvoiceNumber { get; init; }
    public string ContractorName { get; init; }
    public string ExpenseTypeName { get; init; }
    public Guid? ExpenseTypeId { get; init; }
    public decimal? InvoiceTotal { get; init; }
    public decimal? SubTotal { get; init; }
    public decimal? VatAmount { get; init; }
    public DateOnly? InvoiceDate { get; init; }
}