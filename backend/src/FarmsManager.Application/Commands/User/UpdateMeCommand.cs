using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.User;

public record UpdateMeData
{
    public string Name { get; init; }
    public string Password { get; init; }
}

public record UpdateMeCommand(UpdateMeData Data) : IRequest<EmptyBaseResponse>;

public class UpdateMeCommandHandler : IRequestHandler<UpdateMeCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateMeCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateMeCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);

        user.ChangeName(request.Data.Name);

        if (!string.IsNullOrWhiteSpace(request.Data.Password))
        {
            var passwordHash = _passwordHasher.HashPassword(request.Data.Password);
            user.ChangePassword(passwordHash);
        }

        user.SetModified(userId);

        await _userRepository.UpdateAsync(user, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateMeCommandValidator : AbstractValidator<UpdateMeCommand>
{
    public UpdateMeCommandValidator()
    {
        RuleFor(x => x.Data).NotNull();

        RuleFor(x => x.Data.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Data.Password)
            .MinimumLength(8)
            .WithMessage("Hasło musi mieć co najmniej 8 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Data.Password));
    }
}