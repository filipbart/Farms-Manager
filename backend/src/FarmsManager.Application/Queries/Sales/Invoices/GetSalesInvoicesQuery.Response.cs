using FarmsManager.Application.Common;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public class SalesInvoiceRowDto
{
    public Guid Id { get; init; }
    public string CycleText { get; init; }
    public string FarmName { get; init; }
    public string SlaughterhouseName { get; init; }
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string FilePath { get; init; }
}

public class GetSalesInvoicesQueryResponse : PaginationModel<SalesInvoiceRowDto>;