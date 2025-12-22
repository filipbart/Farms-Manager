using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceAssignmentRules;

public record CreateInvoiceAssignmentRuleCommand(CreateInvoiceAssignmentRuleDto Data) : IRequest<BaseResponse<Guid>>;

public record CreateInvoiceAssignmentRuleDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; }
    public Guid AssignedUserId { get; init; }
    public string[] IncludeKeywords { get; init; } = Array.Empty<string>();
    public string[] ExcludeKeywords { get; init; } = Array.Empty<string>();
    public Guid? TaxBusinessEntityId { get; init; }
    public Guid? FarmId { get; init; }
}

public class CreateInvoiceAssignmentRuleCommandHandler : IRequestHandler<CreateInvoiceAssignmentRuleCommand, BaseResponse<Guid>>
{
    private readonly IInvoiceAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public CreateInvoiceAssignmentRuleCommandHandler(
        IInvoiceAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<Guid>> Handle(CreateInvoiceAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var nextPriority = await _repository.GetNextPriorityAsync(cancellationToken);

        var rule = InvoiceAssignmentRuleEntity.CreateNew(
            name: request.Data.Name,
            priority: nextPriority,
            assignedUserId: request.Data.AssignedUserId,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            farmId: request.Data.FarmId,
            description: request.Data.Description,
            createdBy: userId
        );

        await _repository.AddAsync(rule, cancellationToken);

        return new BaseResponse<Guid> { ResponseData = rule.Id };
    }
}
