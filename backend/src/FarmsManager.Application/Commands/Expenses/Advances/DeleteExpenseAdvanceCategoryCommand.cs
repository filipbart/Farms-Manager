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

    public DeleteAdvanceCategoryCommandHandler(IUserDataResolver userDataResolver,
        IExpenseAdvanceCategoryRepository expenseAdvanceCategoryRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseAdvanceCategoryRepository = expenseAdvanceCategoryRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteAdvanceCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseAdvanceCategoryRepository.GetAsync(new GetAdvanceCategoryByIdSpec(request.Id),
                cancellationToken);


        // if (advances.Any())
        // {
        //     foreach (var advance in advances)
        //     {
        //         advance.ClearCategory();
        //         advance.SetModified(userId);
        //     }
        //
        //     await _expenseAdvanceRepository.UpdateRangeAsync(advances, cancellationToken);
        // } TODO jak dodam ExpenseAdvanceEntity

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