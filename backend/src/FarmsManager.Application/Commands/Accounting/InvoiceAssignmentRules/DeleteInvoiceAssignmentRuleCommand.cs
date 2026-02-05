using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceAssignmentRules;

public record DeleteInvoiceAssignmentRuleCommand(Guid RuleId) : IRequest<EmptyBaseResponse>;

public class DeleteInvoiceAssignmentRuleCommandHandler : IRequestHandler<DeleteInvoiceAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceUserAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteInvoiceAssignmentRuleCommandHandler(
        IInvoiceUserAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteInvoiceAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken)
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur nie została znaleziona");

        rule.Delete(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
