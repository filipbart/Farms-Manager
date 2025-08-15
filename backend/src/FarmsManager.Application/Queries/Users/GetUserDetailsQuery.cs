using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Users;

public record UserDetailsQueryResponse
{
    public Guid Id { get; init; }
    public string Login { get; init; }
    public string Name { get; init; }
}

public record GetUserDetailsQuery(Guid UserId) : IRequest<BaseResponse<UserDetailsQueryResponse>>;

public class
    GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery,
    BaseResponse<UserDetailsQueryResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<UserDetailsQueryResponse>> Handle(GetUserDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository.GetAsync<UserDetailsQueryResponse>(
                new UserByIdSpec(request.UserId), cancellationToken);

        return BaseResponse.CreateResponse(user);
    }
}

public class UserDetailsProfile : Profile
{
    public UserDetailsProfile()
    {
        CreateMap<UserEntity, UserDetailsQueryResponse>();
    }
}