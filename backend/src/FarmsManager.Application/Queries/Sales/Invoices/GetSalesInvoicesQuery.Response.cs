using FarmsManager.Application.Common;
using FarmsManager.Application.Helpers;
using FarmsManager.Application.Models.Notifications;

namespace FarmsManager.Application.Queries.Sales.Invoices;

public class SalesInvoiceRowDto
{
    public Guid Id { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public Guid FarmId { get; init; }
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
    public DateOnly? PaymentDate { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
    public NotificationPriority? Priority => PriorityCalculator.CalculatePriority(DueDate, PaymentDate);
}

public class GetSalesInvoicesQueryResponse : PaginationModel<SalesInvoiceRowDto>;