using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceFarmAssignmentRules;

public class GetInvoiceFarmAssignmentRulesQueryHandler 
    : IRequestHandler<GetInvoiceFarmAssignmentRulesQuery, BaseResponse<List<InvoiceFarmAssignmentRuleResponse>>>
{
    private readonly IInvoiceFarmAssignmentRuleRepository _repository;

    public GetInvoiceFarmAssignmentRulesQueryHandler(IInvoiceFarmAssignmentRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<List<InvoiceFarmAssignmentRuleResponse>>> Handle(
        GetInvoiceFarmAssignmentRulesQuery request, 
        CancellationToken cancellationToken)
    {
        var spec = new GetInvoiceFarmAssignmentRulesSpec();
        var rules = await _repository.ListAsync(spec, cancellationToken);

        var response = rules.Select(r => new InvoiceFarmAssignmentRuleResponse
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Priority = r.Priority,
            TargetFarmId = r.TargetFarmId,
            TargetFarmName = r.TargetFarm?.Name,
            IncludeKeywords = r.IncludeKeywords,
            ExcludeKeywords = r.ExcludeKeywords,
            TaxBusinessEntityId = r.TaxBusinessEntityId,
            TaxBusinessEntityName = r.TaxBusinessEntity?.Name,
            InvoiceDirection = r.InvoiceDirection,
            InvoiceDirectionName = r.InvoiceDirection switch
            {
                KSeFInvoiceDirection.Purchase => "Zakup",
                KSeFInvoiceDirection.Sales => "Sprzedaż",
                _ => null
            },
            IsActive = r.IsActive,
            DateCreatedUtc = r.DateCreatedUtc
        }).ToList();

        return new BaseResponse<List<InvoiceFarmAssignmentRuleResponse>> { ResponseData = response };
    }
}
