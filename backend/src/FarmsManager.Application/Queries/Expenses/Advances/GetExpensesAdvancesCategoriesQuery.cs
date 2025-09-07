using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Advances;

public record ExpenseAdvanceCategoryRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public ExpenseAdvanceCategoryType Type { get; init; }
    public string TypeDesc => Type.GetDescription();
}

public record GetExpensesAdvancesCategoriesQuery : IRequest<BaseResponse<List<ExpenseAdvanceCategoryRow>>>;

public class
    GetExpensesAdvancesCategoriesQueryHandler : IRequestHandler<GetExpensesAdvancesCategoriesQuery,
    BaseResponse<List<ExpenseAdvanceCategoryRow>>>
{
    private readonly IExpenseAdvanceCategoryRepository _expenseAdvanceCategoryRepository;

    public GetExpensesAdvancesCategoriesQueryHandler(IExpenseAdvanceCategoryRepository expenseAdvanceCategoryRepository)
    {
        _expenseAdvanceCategoryRepository = expenseAdvanceCategoryRepository;
    }

    public async Task<BaseResponse<List<ExpenseAdvanceCategoryRow>>> Handle(GetExpensesAdvancesCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _expenseAdvanceCategoryRepository.ListAsync<ExpenseAdvanceCategoryRow>(
            new GetAllAdvancesCategoriesSpec(),
            cancellationToken);

        return BaseResponse.CreateResponse(items);
    }
}

public class AdvancesCategoriesProfile : Profile
{
    public AdvancesCategoriesProfile()
    {
        CreateMap<ExpenseAdvanceCategoryEntity, ExpenseAdvanceCategoryRow>();
    }
}

public sealed class GetAllAdvancesCategoriesSpec : BaseSpecification<ExpenseAdvanceCategoryEntity>
{
    public GetAllAdvancesCategoriesSpec()
    {
        EnsureExists();
        Query.OrderBy(t => t.Name);
    }
}