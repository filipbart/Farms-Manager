using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceFarmAssignmentRules;

public record CreateInvoiceFarmAssignmentRuleCommand(CreateInvoiceFarmAssignmentRuleDto Data) : IRequest<BaseResponse<Guid>>;

public record CreateInvoiceFarmAssignmentRuleDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid TargetFarmId { get; init; }
    public string[] IncludeKeywords { get; init; } = [];
    public string[] ExcludeKeywords { get; init; } = [];
    public Guid? TaxBusinessEntityId { get; init; }
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
}

public class CreateInvoiceFarmAssignmentRuleCommandHandler : IRequestHandler<CreateInvoiceFarmAssignmentRuleCommand, BaseResponse<Guid>>
{
    private readonly IInvoiceFarmAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public CreateInvoiceFarmAssignmentRuleCommandHandler(
        IInvoiceFarmAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<Guid>> Handle(CreateInvoiceFarmAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var nextPriority = await _repository.GetNextPriorityAsync(cancellationToken);

        var rule = InvoiceFarmAssignmentRuleEntity.CreateNew(
            name: request.Data.Name,
            priority: nextPriority,
            targetFarmId: request.Data.TargetFarmId,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            invoiceDirection: request.Data.InvoiceDirection,
            description: request.Data.Description,
            createdBy: userId
        );

        await _repository.AddAsync(rule, cancellationToken);

        return new BaseResponse<Guid> { ResponseData = rule.Id };
    }
}
