using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.TransferFeed;

public class GetProductionDataTransferFeedQueryHandler : IRequestHandler<GetProductionDataTransferFeedQuery,
    BaseResponse<GetProductionDataTransferFeedQueryResponse>>
{
    private readonly IProductionDataTransferFeedRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetProductionDataTransferFeedQueryHandler(IProductionDataTransferFeedRepository repository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetProductionDataTransferFeedQueryResponse>> Handle(
        GetProductionDataTransferFeedQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var data = await _repository.ListAsync<ProductionDataTransferFeedRowDto>(
            new GetAllProductionDataTransferFeedSpec(request.Filters, true, accessibleFarmIds), cancellationToken);
        var count = await _repository.CountAsync(
            new GetAllProductionDataTransferFeedSpec(request.Filters, false, accessibleFarmIds),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetProductionDataTransferFeedQueryResponse
        {
            TotalRows = count,
            Items = data
        });
    }
}

public class ProductionDataTransferFeedProfile : Profile
{
    public ProductionDataTransferFeedProfile()
    {
        CreateMap<ProductionDataTransferFeedEntity, ProductionDataTransferFeedRowDto>()
            .ForMember(t => t.FromCycleText, opt => opt.MapFrom(t => t.FromCycle.Identifier + "/" + t.FromCycle.Year))
            .ForMember(t => t.FromFarmName, opt => opt.MapFrom(t => t.FromFarm.Name))
            .ForMember(t => t.FromHenhouseName, opt => opt.MapFrom(t => t.FromHenhouse.Name))
            .ForMember(t => t.ToCycleText, opt => opt.MapFrom(t => t.ToCycle.Identifier + "/" + t.ToCycle.Year))
            .ForMember(t => t.ToFarmName, opt => opt.MapFrom(t => t.ToFarm.Name))
            .ForMember(t => t.ToHenhouseName, opt => opt.MapFrom(t => t.ToHenhouse.Name))
            .ForMember(t => t.FeedName, opt => opt.MapFrom(t => t.FeedName));
    }
}