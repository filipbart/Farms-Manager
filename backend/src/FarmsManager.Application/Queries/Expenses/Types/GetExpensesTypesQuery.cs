using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Types;

public record GetExpensesTypesQuery : IRequest<BaseResponse<GetExpensesTypesQueryResponse>>;

public record GetExpensesTypesQueryResponse
{
    public List<ExpenseTypeRow> Types { get; init; }
}

public record ExpenseTypeRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}

public class
    GetExpensesTypesQueryHandler : IRequestHandler<GetExpensesTypesQuery,
    BaseResponse<GetExpensesTypesQueryResponse>>
{
    private readonly IExpenseTypeRepository _expenseTypeRepository;

    public GetExpensesTypesQueryHandler(IExpenseTypeRepository expenseTypeRepository)
    {
        _expenseTypeRepository = expenseTypeRepository;
    }

    public async Task<BaseResponse<GetExpensesTypesQueryResponse>> Handle(GetExpensesTypesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _expenseTypeRepository.ListAsync<ExpenseTypeRow>(new GetAllExpensesTypesSpec(),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetExpensesTypesQueryResponse
        {
            Types = items
        });
    }
}

public sealed class GetAllExpensesTypesSpec : BaseSpecification<ExpenseTypeEntity>
{
    public GetAllExpensesTypesSpec()
    {
        EnsureExists();
    }
}

public class ExpensesTypesProfile : Profile
{
    public ExpensesTypesProfile()
    {
        CreateMap<ExpenseTypeEntity, ExpenseTypeRow>();
    }
}