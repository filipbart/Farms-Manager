using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class GetExpenseProductionInvoiceByInvoiceNumberSpec : BaseSpecification<ExpenseProductionEntity>,
    ISingleResultSpecification<ExpenseProductionEntity>
{
    public GetExpenseProductionInvoiceByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();
        DisableTracking();

        Query.Where(t => EF.Functions.ILike(invoiceNumber, t.InvoiceNumber));
    }
}