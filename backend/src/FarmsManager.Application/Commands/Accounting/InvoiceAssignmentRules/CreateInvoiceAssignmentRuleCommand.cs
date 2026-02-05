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
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public Guid AssignedUserId { get; set; }
    public string[] IncludeKeywords { get; set; } = [];
    public string[] ExcludeKeywords { get; set; } = [];
    public Guid? TaxBusinessEntityId { get; set; }
    public Guid[] FarmIds { get; set; } = [];
}

public class CreateInvoiceAssignmentRuleCommandHandler : IRequestHandler<CreateInvoiceAssignmentRuleCommand, BaseResponse<Guid>>
{
    private readonly IInvoiceUserAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public CreateInvoiceAssignmentRuleCommandHandler(
        IInvoiceUserAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<Guid>> Handle(CreateInvoiceAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var nextPriority = await _repository.GetNextPriorityAsync(cancellationToken);

        var rule = InvoiceUserAssignmentRuleEntity.CreateNew(
            name: request.Data.Name,
            priority: nextPriority,
            assignedUserId: request.Data.AssignedUserId,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            farmIds: request.Data.FarmIds,
            description: request.Data.Description,
            createdBy: userId
        );

        await _repository.AddAsync(rule, cancellationToken);

        return new BaseResponse<Guid> { ResponseData = rule.Id };
    }
}
