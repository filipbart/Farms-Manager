using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.ProductionData.RemainingFeed;

public class GetProductionDataRemainingFeedQueryHandler : IRequestHandler<GetProductionDataRemainingFeedQuery,
    BaseResponse<GetProductionDataRemainingFeedQueryResponse>>
{
    private readonly IProductionDataRemainingFeedRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetProductionDataRemainingFeedQueryHandler(IProductionDataRemainingFeedRepository repository, IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetProductionDataRemainingFeedQueryResponse>> Handle(
        GetProductionDataRemainingFeedQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;
        
        var data = await _repository.ListAsync<ProductionDataRemainingFeedRowDto>(
            new GetAllProductionDataRemainingFeedSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);
        var count = await _repository.CountAsync(
            new GetAllProductionDataRemainingFeedSpec(request.Filters, false,accessibleFarmIds, isAdmin),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetProductionDataRemainingFeedQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class ProductionDataRemainingFeedProfile : Profile
{
    public ProductionDataRemainingFeedProfile()
    {
        CreateMap<ProductionDataRemainingFeedEntity, ProductionDataRemainingFeedRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.FeedName, opt => opt.MapFrom(t => t.FeedName))
            .ForMember(t => t.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(t => t.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(t => t.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}