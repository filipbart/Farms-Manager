using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Employees;

public record GetEmployeesDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
}

public record GetEmployeesDictionaryQuery : IRequest<BaseResponse<GetEmployeesDictionaryQueryResponse>>;

public class GetEmployeesDictionaryQueryHandler : IRequestHandler<GetEmployeesDictionaryQuery,
    BaseResponse<GetEmployeesDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;

    public GetEmployeesDictionaryQueryHandler(IFarmRepository farmRepository)
    {
        _farmRepository = farmRepository;
    }

    public async Task<BaseResponse<GetEmployeesDictionaryQueryResponse>> Handle(
        GetEmployeesDictionaryQuery request, CancellationToken cancellationToken)
    {
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetEmployeesDictionaryQueryResponse
        {
            Farms = farms
        });
    }
}