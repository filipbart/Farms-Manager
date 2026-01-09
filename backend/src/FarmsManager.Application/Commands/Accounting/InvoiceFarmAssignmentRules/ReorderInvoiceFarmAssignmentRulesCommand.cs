using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceFarmAssignmentRules;

public record ReorderInvoiceFarmAssignmentRulesCommand(List<Guid> OrderedRuleIds) : IRequest<EmptyBaseResponse>;

public class ReorderInvoiceFarmAssignmentRulesCommandHandler : IRequestHandler<ReorderInvoiceFarmAssignmentRulesCommand, EmptyBaseResponse>
{
    private readonly IInvoiceFarmAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public ReorderInvoiceFarmAssignmentRulesCommandHandler(
        IInvoiceFarmAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(ReorderInvoiceFarmAssignmentRulesCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        for (int i = 0; i < request.OrderedRuleIds.Count; i++)
        {
            var ruleId = request.OrderedRuleIds[i];
            var rule = await _repository.GetByIdAsync(ruleId, cancellationToken);
            
            if (rule == null)
                continue;

            rule.SetPriority(i + 1);
            rule.SetModified(userId);
            await _repository.UpdateAsync(rule, cancellationToken);
        }

        return BaseResponse.EmptyResponse;
    }
}
