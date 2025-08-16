using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public class GetExpensesProductionQueryHandler : IRequestHandler<GetExpensesProductionQuery,
    BaseResponse<GetExpensesProductionQueryResponse>>
{
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetExpensesProductionQueryHandler(IExpenseProductionRepository expenseProductionRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _expenseProductionRepository = expenseProductionRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetExpensesProductionQueryResponse>> Handle(GetExpensesProductionQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var data = await _expenseProductionRepository.ListAsync<ExpenseProductionRow>(
            new GetAllExpenseProductionsSpec(request.Filters, true, accessibleFarmIds), cancellationToken);
        var count = await _expenseProductionRepository.CountAsync(
            new GetAllExpenseProductionsSpec(request.Filters, false, accessibleFarmIds), cancellationToken);
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