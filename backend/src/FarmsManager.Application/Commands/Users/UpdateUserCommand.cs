using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Users;

public class UpdateUserData
{
    public string Name { get; init; }
    public string Password { get; init; }
    public bool IsAdmin { get; init; }
}

public record UpdateUserCommand(Guid Id, UpdateUserData Data) : IRequest<EmptyBaseResponse>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var currentUser = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);

        var user = await _userRepository.GetAsync(new UserByIdSpec(request.Id), cancellationToken);

        user.ChangeName(request.Data.Name);

        if (!string.IsNullOrWhiteSpace(request.Data.Password))
        {
            var passwordHash = _passwordHasher.HashPassword(request.Data.Password);
            user.ChangePassword(passwordHash);
        }

        if (!currentUser.IsAdmin && user.IsAdmin != request.Data.IsAdmin)
        {
            throw DomainException.Forbidden();
        }

        user.ChangeIsAdmin(request.Data.IsAdmin);
        user.SetModified(userId);

        await _userRepository.UpdateAsync(user, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Data.Password)
            .MinimumLength(8)
            .WithMessage("Hasło musi mieć co najmniej 8 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Data.Password));
    }
}