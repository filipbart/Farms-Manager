using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
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
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetEmployeesDictionaryQueryHandler(IFarmRepository farmRepository, IUserRepository userRepository, IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetEmployeesDictionaryQueryResponse>> Handle(
        GetEmployeesDictionaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();
        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds), cancellationToken);

        return BaseResponse.CreateResponse(new GetEmployeesDictionaryQueryResponse
        {
            Farms = farms
        });
    }
}