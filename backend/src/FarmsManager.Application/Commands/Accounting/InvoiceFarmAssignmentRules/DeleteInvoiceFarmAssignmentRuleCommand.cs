using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceFarmAssignmentRules;

public record DeleteInvoiceFarmAssignmentRuleCommand(Guid RuleId) : IRequest<EmptyBaseResponse>;

public class DeleteInvoiceFarmAssignmentRuleCommandHandler : IRequestHandler<DeleteInvoiceFarmAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceFarmAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteInvoiceFarmAssignmentRuleCommandHandler(
        IInvoiceFarmAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteInvoiceFarmAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken)
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur do lokalizacji nie została znaleziona");

        rule.Delete(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
