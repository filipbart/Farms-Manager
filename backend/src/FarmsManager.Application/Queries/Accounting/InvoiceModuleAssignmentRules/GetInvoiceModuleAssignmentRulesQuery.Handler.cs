using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Application.Extensions;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceModuleAssignmentRules;

public class GetInvoiceModuleAssignmentRulesQueryHandler 
    : IRequestHandler<GetInvoiceModuleAssignmentRulesQuery, BaseResponse<List<InvoiceModuleAssignmentRuleDto>>>
{
    private readonly IInvoiceModuleAssignmentRuleRepository _repository;

    public GetInvoiceModuleAssignmentRulesQueryHandler(IInvoiceModuleAssignmentRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<List<InvoiceModuleAssignmentRuleDto>>> Handle(
        GetInvoiceModuleAssignmentRulesQuery request, 
        CancellationToken cancellationToken)
    {
        var rules = await _repository.ListAsync(
            new GetInvoiceModuleAssignmentRulesSpec(), cancellationToken);

        var items = rules.Select(r => new InvoiceModuleAssignmentRuleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Priority = r.Priority,
            TargetModule = r.TargetModule,
            TargetModuleName = r.TargetModule.GetDescription(),
            IncludeKeywords = r.IncludeKeywords,
            ExcludeKeywords = r.ExcludeKeywords,
            TaxBusinessEntityId = r.TaxBusinessEntityId,
            TaxBusinessEntityName = r.TaxBusinessEntity?.Name,
            FarmId = r.FarmId,
            FarmName = r.Farm?.Name,
            InvoiceDirection = r.InvoiceDirection,
            InvoiceDirectionName = r.InvoiceDirection?.GetDescription(),
            IsActive = r.IsActive,
            DateCreatedUtc = r.DateCreatedUtc
        }).ToList();

        return BaseResponse.CreateResponse(items);
    }
}
