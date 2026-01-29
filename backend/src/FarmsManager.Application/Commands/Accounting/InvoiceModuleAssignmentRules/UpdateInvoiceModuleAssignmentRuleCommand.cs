using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceModuleAssignmentRules;

public record UpdateInvoiceModuleAssignmentRuleCommand(Guid RuleId, UpdateInvoiceModuleAssignmentRuleDto Data) : IRequest<EmptyBaseResponse>;

public record UpdateInvoiceModuleAssignmentRuleDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ModuleType? TargetModule { get; init; }
    public string[] IncludeKeywords { get; init; } = [];
    public string[] ExcludeKeywords { get; init; } = [];
    public Guid? TaxBusinessEntityId { get; init; }
    public Guid? FarmId { get; init; }
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
    public bool? IsActive { get; init; }
    public bool ClearTaxBusinessEntity { get; init; }
    public bool ClearFarm { get; init; }
    public bool ClearInvoiceDirection { get; init; }
}

public class UpdateInvoiceModuleAssignmentRuleCommandHandler : IRequestHandler<UpdateInvoiceModuleAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceModuleAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateInvoiceModuleAssignmentRuleCommandHandler(
        IInvoiceModuleAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateInvoiceModuleAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken) 
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur do modułów nie została znaleziona");

        rule.Update(
            name: request.Data.Name,
            description: request.Data.Description,
            targetModule: request.Data.TargetModule,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            farmId: request.Data.FarmId,
            invoiceDirection: request.Data.InvoiceDirection,
            isActive: request.Data.IsActive,
            clearTaxBusinessEntity: request.Data.ClearTaxBusinessEntity,
            clearFarm: request.Data.ClearFarm,
            clearInvoiceDirection: request.Data.ClearInvoiceDirection
        );

        rule.SetModified(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
