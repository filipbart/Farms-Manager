using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Users;

public record AddUserData
{
    public string Login { get; init; }
    public string Name { get; init; }
    public string TemporaryPassword { get; init; }
}

public record AddUserCommand(AddUserData Data) : IRequest<EmptyBaseResponse>;

public class AddUserCommandHandler : IRequestHandler<AddUserCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AddUserCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmptyBaseResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var existingUser =
            await _userRepository.FirstOrDefaultAsync(new GetUserByLoginSpec(request.Data.Login),
                cancellationToken);
        if (existingUser is not null)
        {
            throw new Exception($"Użytkownik z loginem '{request.Data.Login}' już istnieje w systemie.");
        }


        var newUser = UserEntity.CreateUser(
            request.Data.Login,
            request.Data.Name,
            userId
        );

        var passwordHash = _passwordHasher.HashPassword(request.Data.TemporaryPassword);
        newUser.ChangePassword(passwordHash);

        await _userRepository.AddAsync(newUser, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Data.Login)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Data.TemporaryPassword)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków.");
    }
}

public sealed class GetUserByLoginSpec : BaseSpecification<UserEntity>,
    ISingleResultSpecification<UserEntity>
{
    public GetUserByLoginSpec(string login)
    {
        EnsureExists();
        Query.Where(u => u.Login == login);
    }
}