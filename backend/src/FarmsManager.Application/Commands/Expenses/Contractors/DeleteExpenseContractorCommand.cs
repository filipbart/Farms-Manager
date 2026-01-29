using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Contractors;

public record DeleteExpenseContractorCommand(Guid ExpenseContractorId) : IRequest<EmptyBaseResponse>;

public class DeleteExpenseContractorCommandHandler : IRequestHandler<DeleteExpenseContractorCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseContractorRepository _expenseContractorRepository;


    public DeleteExpenseContractorCommandHandler(IUserDataResolver userDataResolver,
        IExpenseContractorRepository expenseContractorRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseContractorRepository = expenseContractorRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteExpenseContractorCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseContractorRepository.GetAsync(new GetExpenseContractorByIdSpec(request.ExpenseContractorId),
                cancellationToken);

        entity.Delete(userId);
        await _expenseContractorRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public sealed class GetExpenseContractorByIdSpec : BaseSpecification<ExpenseContractorEntity>,
    ISingleResultSpecification<ExpenseContractorEntity>
{
    public GetExpenseContractorByIdSpec(Guid id, bool includeExpenseTypes = false)
    {
        EnsureExists();
        Query.Where(e => e.Id == id);
        
        if (includeExpenseTypes)
        {
            Query.Include(e => e.ExpenseTypes).ThenInclude(et => et.ExpenseType);
        }
    }
}