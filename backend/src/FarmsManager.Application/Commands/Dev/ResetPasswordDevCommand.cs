using FarmsManager.Application.Commands.Users;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.Application.Commands.Dev;

public record ResetPasswordDevCommand(string Login, string Code, string NewPassword) : IRequest<EmptyBaseResponse>;

public class ResetPasswordDevCommandValidator : AbstractValidator<ResetPasswordDevCommand>
{
    public ResetPasswordDevCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}

public class ResetPasswordDevCommandHandler : IRequestHandler<ResetPasswordDevCommand, EmptyBaseResponse>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public ResetPasswordDevCommandHandler(IUserRepository userRepository, IConfiguration configuration,
        IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<EmptyBaseResponse> Handle(ResetPasswordDevCommand request, CancellationToken cancellationToken)
    {
        var secretCode = _configuration["DevSettings:ResetPasswordCode"];
        if (string.IsNullOrEmpty(secretCode) || request.Code != secretCode)
        {
            throw new Exception("Invalid reset code.");
        }

        var user = await _userRepository.SingleOrDefaultAsync(new GetUserByLoginSpec(request.Login), cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.ChangePassword(passwordHash);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new EmptyBaseResponse();
    }
}