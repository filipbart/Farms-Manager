using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Dev;

public record CreateDevAccountCommand : IRequest<EmptyBaseResponse>
{
    public string Login { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
}

public record CreateDevAccountCommandHandler : IRequestHandler<CreateDevAccountCommand, EmptyBaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateDevAccountCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmptyBaseResponse> Handle(CreateDevAccountCommand request, CancellationToken cancellationToken)
    {
        var user = UserEntity.CreateUser(request.Login, request.Name);
        if (request.Login.IsEmpty() || request.Password.IsEmpty() || request.Name.IsEmpty())
        {
            return new EmptyBaseResponse();
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        user.ChangePassword(passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        return new EmptyBaseResponse();
    }
}