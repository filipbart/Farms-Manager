using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Settings;

public class SaveIrzplusCredentialsCommand : IRequest<EmptyBaseResponse>
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public class SaveIrzplusCredentialsCommandHandler : IRequestHandler<SaveIrzplusCredentialsCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;

    public SaveIrzplusCredentialsCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository, IEncryptionService encryptionService)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public async Task<EmptyBaseResponse> Handle(SaveIrzplusCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        var encryptedPassword = _encryptionService.Encrypt(request.Password);
        user.ChangeIrzplusCredentials(new IrzplusCredentials
        {
            Login = request.Login,
            EncryptedPassword = encryptedPassword
        });
        await _userRepository.UpdateAsync(user, cancellationToken);       

        return BaseResponse.EmptyResponse;
    }
}

public class SaveIrzplusCredentialsCommandValidator : AbstractValidator<SaveIrzplusCredentialsCommand>
{
    public SaveIrzplusCredentialsCommandValidator()
    {
        RuleFor(t => t.Login).NotEmpty();
        RuleFor(t => t.Password).NotEmpty();
    }
}