using Ardalis.Specification;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries;

public record HatcheryRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public string ProducerNumber { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public record GetAllHatcheriesQuery : IRequest<BaseResponse<GetAllHatcheriesQueryResponse>>;

public class GetAllHatcheriesQueryResponse : PaginationModel<HatcheryRowDto>;

public class
    GetAllHatcheriesQueryHandler : IRequestHandler<GetAllHatcheriesQuery, BaseResponse<GetAllHatcheriesQueryResponse>>
{
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetAllHatcheriesQueryHandler(IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetAllHatcheriesQueryResponse>> Handle(GetAllHatcheriesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var items = await _hatcheryRepository.ListAsync<HatcheryRowDto>(new GetAllHatcheriesSpec(isAdmin), cancellationToken);
        return BaseResponse.CreateResponse(new GetAllHatcheriesQueryResponse
        {
            TotalRows = items.Count,
            Items = items.ClearAdminData(isAdmin)
        });
    }
}

public sealed class GetAllHatcheriesSpec : BaseSpecification<HatcheryEntity>
{
    public GetAllHatcheriesSpec(bool isAdmin, bool? showDeleted = null)
    {
        EnsureExists(showDeleted, isAdmin);
        Query.OrderBy(t => t.Name);
    }
}