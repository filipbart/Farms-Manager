using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Settings;

public class SaveIrzplusCredentialsCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}

public class SaveIrzplusCredentialsCommandHandler : IRequestHandler<SaveIrzplusCredentialsCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IFarmRepository _farmRepository;

    public SaveIrzplusCredentialsCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IEncryptionService encryptionService, IFarmRepository farmRepository)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _farmRepository = farmRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SaveIrzplusCredentialsCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);

        var encryptedPassword = _encryptionService.Encrypt(request.Password);
        user.AddIrzplusCredentials(new IrzplusCredentials
        {
            FarmId = farm.Id,
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
        RuleFor(t => t.FarmId).NotEmpty();
        RuleFor(t => t.Login).NotEmpty();
        RuleFor(t => t.Password).NotEmpty();
    }
}