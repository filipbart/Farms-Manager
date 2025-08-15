using AutoMapper;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Users;


public class UserRowDto
{
    public Guid Id { get; init; }
    public string Login { get; init; }
    public string Name { get; init; }
    public DateTime DateCreatedUtc { get; init; }
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

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetUsersQueryResponse>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new GetAllUsersSpec(request.Filters, true);
        var countSpec = new GetAllUsersSpec(request.Filters, false);

        var data = await _userRepository.ListAsync(spec, cancellationToken);
        var count = await _userRepository.CountAsync(countSpec, cancellationToken);
        
        var items = _mapper.Map<List<UserRowDto>>(data);

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