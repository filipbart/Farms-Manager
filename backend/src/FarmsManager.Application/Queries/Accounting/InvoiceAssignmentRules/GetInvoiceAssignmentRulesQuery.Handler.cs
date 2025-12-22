using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;

public class GetInvoiceAssignmentRulesQueryHandler 
    : IRequestHandler<GetInvoiceAssignmentRulesQuery, BaseResponse<List<InvoiceAssignmentRuleDto>>>
{
    private readonly IInvoiceAssignmentRuleRepository _repository;

    public GetInvoiceAssignmentRulesQueryHandler(IInvoiceAssignmentRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<List<InvoiceAssignmentRuleDto>>> Handle(
        GetInvoiceAssignmentRulesQuery request, 
        CancellationToken cancellationToken)
    {
        var rules = await _repository.ListAsync(
            new GetInvoiceAssignmentRulesSpec(), cancellationToken);

        var items = rules.Select(r => new InvoiceAssignmentRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Priority = r.Priority,
            AssignedUserId = r.AssignedUserId,
            AssignedUserName = r.AssignedUser?.Name ?? string.Empty,
            IncludeKeywords = r.IncludeKeywords,
            ExcludeKeywords = r.ExcludeKeywords,
            TaxBusinessEntityId = r.TaxBusinessEntityId,
            TaxBusinessEntityName = r.TaxBusinessEntity?.Name,
            FarmId = r.FarmId,
            FarmName = r.Farm?.Name,
            IsActive = r.IsActive,
            DateCreatedUtc = r.DateCreatedUtc
        }).ToList();

        return BaseResponse.CreateResponse(items);
    }
}
