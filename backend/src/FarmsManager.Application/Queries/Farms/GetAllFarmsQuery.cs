using Ardalis.Specification;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Domain.Models.FarmAggregate;
using MediatR;

namespace FarmsManager.Application.Queries.Farms;

public record GetAllFarmsQuery : IRequest<BaseResponse<GetAllFarmsQueryResponse>>;

public class GetAllFarmsQueryResponse : PaginationModel<FarmRowDto>;

public class GetAllFarmsQueryHandler : IRequestHandler<GetAllFarmsQuery, BaseResponse<GetAllFarmsQueryResponse>>
{
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetAllFarmsQueryHandler(IFarmRepository farmRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetAllFarmsQueryResponse>> Handle(GetAllFarmsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.IsAdmin ? null : user.Farms?.Select(t => t.FarmId).ToList();

        var items = await _farmRepository.ListAsync<FarmRowDto>(new GetAllFarmsSpec(accessibleFarmIds),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetAllFarmsQueryResponse
        {
            TotalRows = items.Count,
            Items = items
        });
    }
}

public sealed class GetAllFarmsSpec : BaseSpecification<FarmEntity>
{
    public GetAllFarmsSpec(List<Guid> accessibleFarmIds)
    {
        EnsureExists();
        if (accessibleFarmIds is not null && accessibleFarmIds.Any())
        {
            Query.Where(t => accessibleFarmIds.Contains(t.Id));
        }

        Query.Include(t => t.Henhouses);
    }
}