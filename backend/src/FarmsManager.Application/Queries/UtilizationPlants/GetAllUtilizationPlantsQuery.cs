using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.UtilizationPlants;

public record UtilizationPlantRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public string IrzNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public record GetAllUtilizationPlantsQuery : IRequest<BaseResponse<GetAllUtilizationPlantsQueryResponse>>;

public class GetAllUtilizationPlantsQueryResponse : PaginationModel<UtilizationPlantRowDto>;

public class
    GetAllUtilizationPlantsQueryHandler : IRequestHandler<GetAllUtilizationPlantsQuery,
    BaseResponse<GetAllUtilizationPlantsQueryResponse>>
{
    private readonly IUtilizationPlantRepository _utilizationPlantRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetAllUtilizationPlantsQueryHandler(IUtilizationPlantRepository utilizationPlantRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _utilizationPlantRepository = utilizationPlantRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetAllUtilizationPlantsQueryResponse>> Handle(GetAllUtilizationPlantsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var items = await _utilizationPlantRepository.ListAsync<UtilizationPlantRowDto>(
            new GetAllUtilizationPlantsSpec(isAdmin),
            cancellationToken);

        return BaseResponse.CreateResponse(new GetAllUtilizationPlantsQueryResponse
        {
            TotalRows = items.Count,
            Items = items.ClearAdminData(isAdmin)
        });
    }
}

public sealed class GetAllUtilizationPlantsSpec : BaseSpecification<UtilizationPlantEntity>
{
    public GetAllUtilizationPlantsSpec(bool isAdmin, bool? showDeleted = null)
    {
        EnsureExists(showDeleted, isAdmin);
        Query.OrderBy(t => t.Name);
    }
}

public class UtilizationPlantProfile : Profile
{
    public UtilizationPlantProfile()
    {
        CreateMap<UtilizationPlantEntity, UtilizationPlantRowDto>();
    }
}