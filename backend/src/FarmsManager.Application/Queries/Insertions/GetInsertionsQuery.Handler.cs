using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Insertions;

public class GetInsertionsQueryHandler : IRequestHandler<GetInsertionsQuery, BaseResponse<GetInsertionsQueryResponse>>
{
    private readonly IInsertionRepository _insertionRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetInsertionsQueryHandler(IInsertionRepository insertionRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _insertionRepository = insertionRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetInsertionsQueryResponse>> Handle(GetInsertionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;
        var isAdmin = user.IsAdmin;

        var data = await _insertionRepository.ListAsync<InsertionRowDto>(
            new GetAllInsertionsSpec(request.Filters, true, accessibleFarmIds, isAdmin), cancellationToken);
        var count = await _insertionRepository.CountAsync(
            new GetAllInsertionsSpec(request.Filters, false, accessibleFarmIds, isAdmin),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetInsertionsQueryResponse
        {
            TotalRows = count,
            Items = data.ClearAdminData(isAdmin)
        });
    }
}

public class InsertionProfile : Profile
{
    public InsertionProfile()
    {
        CreateMap<InsertionEntity, InsertionRowDto>()
            .ForMember(t => t.CycleText, opt => opt.MapFrom(t => t.Cycle.Identifier + "/" + t.Cycle.Year))
            .ForMember(t => t.FarmName, opt => opt.MapFrom(t => t.Farm.Name))
            .ForMember(t => t.HenhouseName, opt => opt.MapFrom(t => t.Henhouse.Name))
            .ForMember(t => t.HatcheryName, opt => opt.MapFrom(t => t.Hatchery.Name));
    }
}