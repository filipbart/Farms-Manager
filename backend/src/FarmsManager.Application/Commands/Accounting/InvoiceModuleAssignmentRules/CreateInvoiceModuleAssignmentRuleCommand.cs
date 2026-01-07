using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceModuleAssignmentRules;

public record CreateInvoiceModuleAssignmentRuleCommand(CreateInvoiceModuleAssignmentRuleDto Data) : IRequest<BaseResponse<Guid>>;

public record CreateInvoiceModuleAssignmentRuleDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public ModuleType TargetModule { get; init; }
    public string[] IncludeKeywords { get; init; } = Array.Empty<string>();
    public string[] ExcludeKeywords { get; init; } = Array.Empty<string>();
    public Guid? TaxBusinessEntityId { get; init; }
    public Guid? FarmId { get; init; }
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
}

public class CreateInvoiceModuleAssignmentRuleCommandHandler : IRequestHandler<CreateInvoiceModuleAssignmentRuleCommand, BaseResponse<Guid>>
{
    private readonly IInvoiceModuleAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public CreateInvoiceModuleAssignmentRuleCommandHandler(
        IInvoiceModuleAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<Guid>> Handle(CreateInvoiceModuleAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var nextPriority = await _repository.GetNextPriorityAsync(cancellationToken);

        var rule = InvoiceModuleAssignmentRuleEntity.CreateNew(
            name: request.Data.Name,
            priority: nextPriority,
            targetModule: request.Data.TargetModule,
            includeKeywords: request.Data.IncludeKeywords,
            excludeKeywords: request.Data.ExcludeKeywords,
            taxBusinessEntityId: request.Data.TaxBusinessEntityId,
            farmId: request.Data.FarmId,
            invoiceDirection: request.Data.InvoiceDirection,
            description: request.Data.Description,
            createdBy: userId
        );

        await _repository.AddAsync(rule, cancellationToken);

        return new BaseResponse<Guid> { ResponseData = rule.Id };
    }
}
