using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Auth;

public class AuthenticateCommand : IRequest<BaseResponse<AuthenticateCommandResponse>>
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public record AuthenticateCommandResponse(string AccessToken, DateTime ExpiryAtUtc, bool MustChangePassword = false);

public record
    AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, BaseResponse<AuthenticateCommandResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserSessionService _userSessionService;

    public AuthenticateCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher,
        IUserSessionService userSessionService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userSessionService = userSessionService;
    }

    public async Task<BaseResponse<AuthenticateCommandResponse>> Handle(AuthenticateCommand request,
        CancellationToken ct)
    {
        request.Login = request.Login.Replace(" ", string.Empty).Trim();
        request.Password = request.Password.Replace(" ", string.Empty).Trim();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByLoginSpec(request.Login), ct) ??
                   throw DomainException.InvalidCredentials();

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw DomainException.InvalidCredentials();
        }

        var tokenResponse = await _userSessionService.GenerateToken(user, ct);
        return BaseResponse.CreateResponse(new AuthenticateCommandResponse(tokenResponse.Token,
            DateTime.UtcNow + tokenResponse.ValidFor, user.MustChangePassword));
    }
}