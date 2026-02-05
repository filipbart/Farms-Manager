using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceAssignmentRules;

public record ReorderInvoiceAssignmentRulesCommand(List<Guid> OrderedRuleIds) : IRequest<EmptyBaseResponse>;

public class ReorderInvoiceAssignmentRulesCommandHandler : IRequestHandler<ReorderInvoiceAssignmentRulesCommand, EmptyBaseResponse>
{
    private readonly IInvoiceUserAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public ReorderInvoiceAssignmentRulesCommandHandler(
        IInvoiceUserAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(ReorderInvoiceAssignmentRulesCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        for (int i = 0; i < request.OrderedRuleIds.Count; i++)
        {
            var ruleId = request.OrderedRuleIds[i];
            var rule = await _repository.GetByIdAsync(ruleId, cancellationToken);
            
            if (rule == null)
                continue;

            rule.SetPriority(i + 1); // Priorytet od 1
            rule.SetModified(userId);
            await _repository.UpdateAsync(rule, cancellationToken);
        }

        return BaseResponse.EmptyResponse;
    }
}
