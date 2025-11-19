using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Specifications.Expenses;

public sealed class GetExpenseProductionInvoiceByInvoiceNumberAndContractorSpec : BaseSpecification<ExpenseProductionEntity>,
    ISingleResultSpecification<ExpenseProductionEntity>
{
    public GetExpenseProductionInvoiceByInvoiceNumberAndContractorSpec(string invoiceNumber, Guid contractorId)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(t => EF.Functions.ILike(t.InvoiceNumber, invoiceNumber));
        Query.Where(t => t.ExpenseContractorId == contractorId);
    }
}
