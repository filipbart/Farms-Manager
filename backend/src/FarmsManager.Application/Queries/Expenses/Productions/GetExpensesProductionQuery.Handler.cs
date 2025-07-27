using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public class GetExpensesProductionQueryHandler : IRequestHandler<GetExpensesProductionQuery,
    BaseResponse<GetExpensesProductionQueryResponse>>
{
    private readonly IExpenseProductionRepository _expenseProductionRepository;

    public GetExpensesProductionQueryHandler(IExpenseProductionRepository expenseProductionRepository)
    {
        _expenseProductionRepository = expenseProductionRepository;
    }

    public async Task<BaseResponse<GetExpensesProductionQueryResponse>> Handle(GetExpensesProductionQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _expenseProductionRepository.ListAsync<ExpenseProductionRow>(
            new GetAllExpenseProductionsSpec(request.Filters, true), cancellationToken);
        var count = await _expenseProductionRepository.CountAsync(
            new GetAllExpenseProductionsSpec(request.Filters, false), cancellationToken);
        return BaseResponse.CreateResponse(new GetExpensesProductionQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class ExpenseProductionProfile : Profile
{
    public ExpenseProductionProfile()
    {
        CreateMap<ExpenseProductionEntity, ExpenseProductionRow>()
            .ForMember(m => m.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(m => m.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(m => m.ContractorName, opt => opt.MapFrom(t => t.ExpenseContractor.Name))
            .ForMember(m => m.ExpenseTypeName, opt => opt.MapFrom(t => t.ExpenseContractor.ExpenseType.Name));
    }
}