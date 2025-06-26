using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Dtos;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.User;

public class DetailsQuery : IRequest<BaseResponse<UserDetailsDto>>;

public class DetailsQueryHandler : IRequestHandler<DetailsQuery, BaseResponse<UserDetailsDto>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;

    public DetailsQueryHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IEncryptionService encryptionService)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public async Task<BaseResponse<UserDetailsDto>> Handle(DetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        string password = null;
        if (user.IrzplusCredentials is not null)
        {
            password = _encryptionService.Decrypt(user.IrzplusCredentials.EncryptedPassword);
        }

        return BaseResponse.CreateResponse(new UserDetailsDto
        {
            Login = user.Login,
            Name = user.Name,
            IrzplusCredentials = new IrzplusCredentialsDto
            {
                Login = user.IrzplusCredentials?.Login,
                Password = password,
            }
        });
    }
}