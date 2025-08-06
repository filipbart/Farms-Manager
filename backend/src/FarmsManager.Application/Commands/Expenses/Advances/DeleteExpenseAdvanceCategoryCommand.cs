using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Advances;

public record DeleteAdvanceCategoryCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteAdvanceCategoryCommandHandler : IRequestHandler<DeleteAdvanceCategoryCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseAdvanceCategoryRepository _expenseAdvanceCategoryRepository;
    private readonly IExpenseAdvanceRepository _expenseAdvanceRepository;

    public DeleteAdvanceCategoryCommandHandler(IUserDataResolver userDataResolver,
        IExpenseAdvanceCategoryRepository expenseAdvanceCategoryRepository,
        IExpenseAdvanceRepository expenseAdvanceRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseAdvanceCategoryRepository = expenseAdvanceCategoryRepository;
        _expenseAdvanceRepository = expenseAdvanceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteAdvanceCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseAdvanceCategoryRepository.GetAsync(new GetAdvanceCategoryByIdSpec(request.Id),
                cancellationToken);

        var advances =
            await _expenseAdvanceRepository.ListAsync(new GetExpenseAdvancesByCategoryIdSpec(entity.Id),
                cancellationToken);

        if (advances.Count != 0)
        {
            foreach (var advance in advances)
            {
                advance.SetExpenseAdvanceCategory(null);
                advance.SetModified(userId);
            }

            await _expenseAdvanceRepository.UpdateRangeAsync(advances, cancellationToken);
        }

        entity.Delete(userId);
        await _expenseAdvanceCategoryRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class GetAdvanceCategoryByIdSpec : BaseSpecification<ExpenseAdvanceCategoryEntity>,
    ISingleResultSpecification<ExpenseAdvanceCategoryEntity>
{
    public GetAdvanceCategoryByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(e => e.Id == id);
    }
}

public sealed class GetExpenseAdvancesByCategoryIdSpec : BaseSpecification<ExpenseAdvanceEntity>
{
    public GetExpenseAdvancesByCategoryIdSpec(Guid categoryId)
    {
        EnsureExists();
        Query.Where(t => t.ExpenseAdvanceCategoryId == categoryId);
    }
}