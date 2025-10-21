using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Slaughterhouses;

public record SlaughterhouseRowDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
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

public record GetAllSlaughterhousesQuery(bool? ShowDeleted = null) : IRequest<BaseResponse<GetAllSlaughterhousesQueryResponse>>;

public class GetAllSlaughterhousesQueryResponse : PaginationModel<SlaughterhouseRowDto>;

public class
    GetAllSlaughterhousesQueryHandler : IRequestHandler<GetAllSlaughterhousesQuery,
    BaseResponse<GetAllSlaughterhousesQueryResponse>>
{
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetAllSlaughterhousesQueryHandler(ISlaughterhouseRepository slaughterhouseRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _slaughterhouseRepository = slaughterhouseRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }


    public async Task<BaseResponse<GetAllSlaughterhousesQueryResponse>> Handle(GetAllSlaughterhousesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var items = await _slaughterhouseRepository.ListAsync<SlaughterhouseRowDto>(new GetAllSlaughterhousesSpec(isAdmin, request.ShowDeleted),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetAllSlaughterhousesQueryResponse
        {
            TotalRows = items.Count,
            Items = items.ClearAdminData(isAdmin)
        });
    }
}

public sealed class GetAllSlaughterhousesSpec : BaseSpecification<SlaughterhouseEntity>
{
    public GetAllSlaughterhousesSpec(bool isAdmin, bool? showDeleted = null)
    {
        EnsureExists(showDeleted, isAdmin);
        Query.OrderBy(t => t.Name);
    }
}

public class SlaughterhouseProfile : Profile
{
    public SlaughterhouseProfile()
    {
        CreateMap<SlaughterhouseEntity, SlaughterhouseRowDto>();
    }
}