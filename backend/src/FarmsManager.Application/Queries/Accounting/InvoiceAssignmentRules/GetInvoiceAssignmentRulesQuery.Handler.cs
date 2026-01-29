using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;

public class GetInvoiceAssignmentRulesQueryHandler 
    : IRequestHandler<GetInvoiceAssignmentRulesQuery, BaseResponse<List<InvoiceAssignmentRuleDto>>>
{
    private readonly IInvoiceAssignmentRuleRepository _repository;
    private readonly IFarmRepository _farmRepository;

    public GetInvoiceAssignmentRulesQueryHandler(
        IInvoiceAssignmentRuleRepository repository,
        IFarmRepository farmRepository)
    {
        _repository = repository;
        _farmRepository = farmRepository;
    }

    public async Task<BaseResponse<List<InvoiceAssignmentRuleDto>>> Handle(
        GetInvoiceAssignmentRulesQuery request, 
        CancellationToken cancellationToken)
    {
        var rules = await _repository.ListAsync(
            new GetInvoiceAssignmentRulesSpec(), cancellationToken);

        // Zbierz wszystkie unikalne FarmIds z reguł
        var allFarmIds = rules
            .SelectMany(r => r.FarmIds)
            .Distinct()
            .ToList();

        // Pobierz fermy dla tych ID
        var farms = allFarmIds.Count > 0
            ? await _farmRepository.ListAsync(new FarmsByIdsSpec(allFarmIds), cancellationToken)
            : new List<FarmEntity>();

        var farmDictionary = farms.ToDictionary(f => f.Id, f => f.Name);

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
            FarmIds = r.FarmIds,
            FarmNames = r.FarmIds
                .Where(farmId => farmDictionary.ContainsKey(farmId))
                .ToDictionary(farmId => farmId, farmId => farmDictionary[farmId]),
            IsActive = r.IsActive,
            DateCreatedUtc = r.DateCreatedUtc
        }).ToList();

        return BaseResponse.CreateResponse(items);
    }
}

internal sealed class FarmsByIdsSpec : BaseSpecification<FarmEntity>
{
    public FarmsByIdsSpec(List<Guid> farmIds)
    {
        Query.Where(f => farmIds.Contains(f.Id));
        DisableTracking();
    }
}
