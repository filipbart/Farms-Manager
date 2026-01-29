using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceAssignmentRules;

public record UpdateInvoiceAssignmentRuleCommand(Guid RuleId, UpdateInvoiceAssignmentRuleDto Data) : IRequest<EmptyBaseResponse>;

public record UpdateInvoiceAssignmentRuleDto
{
    public string Name { get; init; }
    public string Description { get; init; }
    public Guid? AssignedUserId { get; init; }
    public string[] IncludeKeywords { get; init; }
    public string[] ExcludeKeywords { get; init; }
    public Guid? TaxBusinessEntityId { get; init; }
    public Guid[] FarmIds { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateInvoiceAssignmentRuleCommandHandler : IRequestHandler<UpdateInvoiceAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateInvoiceAssignmentRuleCommandHandler(
        IInvoiceAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateInvoiceAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken) 
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur nie została znaleziona");

        rule.Update(
            name: request.Data.Name,
            description: request.Data.Description,
            assignedUserId: request.Data.AssignedUserId,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            farmIds: request.Data.FarmIds,
            isActive: request.Data.IsActive
        );

        rule.SetModified(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
