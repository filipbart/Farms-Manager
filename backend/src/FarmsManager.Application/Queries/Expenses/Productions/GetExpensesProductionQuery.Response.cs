using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public class ExpenseProductionRow
{
    public string Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string ContractorName { get; init; }
    public string InvoiceNumber { get; init; }
    public string ExpenseTypeName { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string FilePath { get; init; }
}

public class GetExpensesProductionQueryResponse : PaginationModel<ExpenseProductionRow>;