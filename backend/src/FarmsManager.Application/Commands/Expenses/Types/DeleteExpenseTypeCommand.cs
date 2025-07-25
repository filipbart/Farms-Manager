using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Types;

public record DeleteExpenseTypeCommand(Guid ExpenseTypeId) : IRequest<EmptyBaseResponse>;

public class DeleteExpenseTypeCommandHandler : IRequestHandler<DeleteExpenseTypeCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseTypeRepository _expenseTypeRepository;

    public DeleteExpenseTypeCommandHandler(IUserDataResolver userDataResolver,
        IExpenseTypeRepository expenseTypeRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseTypeRepository = expenseTypeRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteExpenseTypeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseTypeRepository.GetAsync(new GetExpenseTypeByIdSpec(request.ExpenseTypeId),
                cancellationToken);

        entity.Delete(userId);
        await _expenseTypeRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class GetExpenseTypeByIdSpec : BaseSpecification<ExpenseTypeEntity>,
    ISingleResultSpecification<ExpenseTypeEntity>
{
    public GetExpenseTypeByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(e => e.Id == id);
    }
}