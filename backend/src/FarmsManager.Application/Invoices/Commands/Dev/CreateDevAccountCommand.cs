using FarmsManager.Domain.Aggregates.UserAggregate.Entites;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Invoices.Commands.Dev;

public record CreateDevAccountCommand : IRequest
{
    public string Login { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
}

public record CreateDevAccountCommandHandler : IRequestHandler<CreateDevAccountCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateDevAccountCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(CreateDevAccountCommand request, CancellationToken cancellationToken)
    {
        var user = UserEntity.CreateUser(request.Login, request.Password, request.Name);
        await _userRepository.AddAsync(user, cancellationToken);
    }
}