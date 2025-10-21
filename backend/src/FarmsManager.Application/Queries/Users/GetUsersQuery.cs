using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Users;


public class UserRowDto
{
    public Guid Id { get; init; }
    public string Login { get; init; }
    public string Name { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public class GetUsersQueryResponse : PaginationModel<UserRowDto>;
public enum UsersOrderBy
{
    Login,
    Name,
    DateCreatedUtc
}

public record GetUsersQueryFilters : OrderedPaginationParams<UsersOrderBy>
{
    public string SearchPhrase { get; init; }
}

public record GetUsersQuery(GetUsersQueryFilters Filters)
    : IRequest<BaseResponse<GetUsersQueryResponse>>;


public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, BaseResponse<GetUsersQueryResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IUserDataResolver _userDataResolver;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper, IUserDataResolver userDataResolver)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<GetUsersQueryResponse>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var spec = new GetAllUsersSpec(request.Filters, true, isAdmin);
        var countSpec = new GetAllUsersSpec(request.Filters, false, isAdmin);

        var data = await _userRepository.ListAsync(spec, cancellationToken);
        var count = await _userRepository.CountAsync(countSpec, cancellationToken);
        
        var items = _mapper.Map<List<UserRowDto>>(data).ClearAdminData(isAdmin);

        return BaseResponse.CreateResponse(new GetUsersQueryResponse
        {
            TotalRows = count,
            Items = items
        });
    }
}

public class UserProfile : Profile
{
    public UserProfile()
    {
        
        CreateMap<UserEntity, UserRowDto>();
    }
}