using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceFarmAssignmentRules;

public record UpdateInvoiceFarmAssignmentRuleCommand(Guid RuleId, UpdateInvoiceFarmAssignmentRuleDto Data) : IRequest<EmptyBaseResponse>;

public record UpdateInvoiceFarmAssignmentRuleDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public Guid? TargetFarmId { get; init; }
    public string[]? IncludeKeywords { get; init; }
    public string[]? ExcludeKeywords { get; init; }
    public Guid? TaxBusinessEntityId { get; init; }
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
    public bool? IsActive { get; init; }
    public bool ClearTaxBusinessEntity { get; init; }
    public bool ClearInvoiceDirection { get; init; }
}

public class UpdateInvoiceFarmAssignmentRuleCommandHandler : IRequestHandler<UpdateInvoiceFarmAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceFarmAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateInvoiceFarmAssignmentRuleCommandHandler(
        IInvoiceFarmAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateInvoiceFarmAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken)
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur do lokalizacji nie została znaleziona");

        rule.Update(
            name: request.Data.Name,
            description: request.Data.Description,
            targetFarmId: request.Data.TargetFarmId,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            invoiceDirection: request.Data.InvoiceDirection,
            isActive: request.Data.IsActive,
            clearTaxBusinessEntity: request.Data.ClearTaxBusinessEntity,
            clearInvoiceDirection: request.Data.ClearInvoiceDirection
        );

        rule.SetModified(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
