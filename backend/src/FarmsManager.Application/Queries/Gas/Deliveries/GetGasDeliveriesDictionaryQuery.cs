using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Gas.Deliveries;

public record GetGasDeliveriesDictionaryQueryResponse
{
    public List<FarmDictModel> Farms { get; set; } = [];
    public List<DictModel> Contractors { get; set; } = [];
}

public record GetGasDeliveriesDictionaryQuery : IRequest<BaseResponse<GetGasDeliveriesDictionaryQueryResponse>>;

public class GetGasDeliveriesDictionaryQueryHandler : IRequestHandler<GetGasDeliveriesDictionaryQuery,
    BaseResponse<GetGasDeliveriesDictionaryQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetGasDeliveriesDictionaryQueryHandler(IFarmRepository farmRepository,
        IGasContractorRepository gasContractorRepository, IUserRepository userRepository,
        IUserDataResolver userDataResolver)
    {
        _farmRepository = farmRepository;
        _gasContractorRepository = gasContractorRepository;
        _userRepository = userRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetGasDeliveriesDictionaryQueryResponse>> Handle(
        GetGasDeliveriesDictionaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var farms = await _farmRepository.ListAsync<FarmDictModel>(new GetAllFarmsSpec(accessibleFarmIds, user.IsAdmin),
            cancellationToken);
        var contractors =
            await _gasContractorRepository.ListAsync<DictModel>(new GetAllGasContractorsSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetGasDeliveriesDictionaryQueryResponse
        {
            Farms = farms,
            Contractors = contractors,
        });
    }
}

public class GasDeliveriesDictionaryProfile : Profile
{
    public GasDeliveriesDictionaryProfile()
    {
        CreateMap<GasContractorEntity, DictModel>();
    }
}