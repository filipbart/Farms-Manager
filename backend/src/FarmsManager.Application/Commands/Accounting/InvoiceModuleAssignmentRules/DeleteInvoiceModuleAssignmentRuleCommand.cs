using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting.InvoiceModuleAssignmentRules;

public record DeleteInvoiceModuleAssignmentRuleCommand(Guid RuleId) : IRequest<EmptyBaseResponse>;

public class DeleteInvoiceModuleAssignmentRuleCommandHandler : IRequestHandler<DeleteInvoiceModuleAssignmentRuleCommand, EmptyBaseResponse>
{
    private readonly IInvoiceModuleAssignmentRuleRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteInvoiceModuleAssignmentRuleCommandHandler(
        IInvoiceModuleAssignmentRuleRepository repository,
        IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteInvoiceModuleAssignmentRuleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var rule = await _repository.GetByIdAsync(request.RuleId, cancellationToken)
            ?? throw DomainException.RecordNotFound("Reguła przypisywania faktur do modułów nie została znaleziona");

        rule.Delete(userId);
        await _repository.UpdateAsync(rule, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}
