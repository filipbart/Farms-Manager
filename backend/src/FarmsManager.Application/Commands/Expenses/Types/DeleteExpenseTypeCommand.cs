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
    private readonly IExpenseContractorRepository _expenseContractorRepository;

    public DeleteExpenseTypeCommandHandler(IUserDataResolver userDataResolver,
        IExpenseTypeRepository expenseTypeRepository, IExpenseContractorRepository expenseContractorRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseTypeRepository = expenseTypeRepository;
        _expenseContractorRepository = expenseContractorRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteExpenseTypeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseTypeRepository.GetAsync(new GetExpenseTypeByIdSpec(request.ExpenseTypeId),
                cancellationToken);

        var contractors =
            await _expenseContractorRepository.ListAsync(new GetContractorsByExpenseTypeSpec(request.ExpenseTypeId),
                cancellationToken);
        if (contractors.Count != 0)
        {
            foreach (var expenseContractorEntity in contractors)
            {
                expenseContractorEntity.RemoveExpenseType(request.ExpenseTypeId);
                expenseContractorEntity.SetModified(userId);
            }

            await _expenseContractorRepository.UpdateRangeAsync(contractors, cancellationToken);
        }


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

public sealed class GetContractorsByExpenseTypeSpec : BaseSpecification<ExpenseContractorEntity>
{
    public GetContractorsByExpenseTypeSpec(Guid expenseTypeId)
    {
        EnsureExists();
        Query.Include(t => t.ExpenseTypes);
        Query.Where(t => t.ExpenseTypes.Any(et => et.ExpenseTypeId == expenseTypeId));
    }
}