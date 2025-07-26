using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Expenses.Productions;

public class GetExpensesProductionQueryHandler : IRequestHandler<GetExpensesProductionQuery,
    BaseResponse<GetExpensesProductionQueryResponse>>
{
    public Task<BaseResponse<GetExpensesProductionQueryResponse>> Handle(GetExpensesProductionQuery request, CancellationToken cancellationToken)
    {
        
    }
}