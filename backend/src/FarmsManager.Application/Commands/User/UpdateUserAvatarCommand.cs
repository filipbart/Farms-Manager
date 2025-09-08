using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FarmsManager.Application.Commands.User;

public record UpdateUserAvatarCommand : IRequest<EmptyBaseResponse>
{
    public IFormFile AvatarFile { get; init; }
}

public class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IS3Service _s3Service;

    public UpdateUserAvatarCommandHandler(IUserDataResolver userDataResolver, IUserRepository userRepository,
        IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), cancellationToken) ??
                   throw DomainException.UserNotFound();

        await using var stream = request.AvatarFile.OpenReadStream();

        using var image = await Image.LoadAsync(stream, cancellationToken);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(256, 256),
            Mode = ResizeMode.Crop
        }));

        await using var memoryStream = new MemoryStream();
        await image.SaveAsWebpAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var filePath = $"avatars/{user.Id}.webp";
        await _s3Service.UploadFileAsync(memoryStream.ToArray(), FileType.Avatar, filePath, cancellationToken);

        user.ChangeAvatarPath(filePath);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var fullUrl = _s3Service.GeneratePreSignedUrl(FileType.Avatar, filePath, "avatar.webp");
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateUserAvatarCommandValidator : AbstractValidator<UpdateUserAvatarCommand>
{
    private const int MaxFileSizeInMb = 5;

    public UpdateUserAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull().WithMessage("Plik jest wymagany.");

        When(x => x.AvatarFile != null, () =>
        {
            RuleFor(x => x.AvatarFile.Length)
                .GreaterThan(0).WithMessage("Plik nie może być pusty.")
                .LessThanOrEqualTo(MaxFileSizeInMb * 1024 * 1024)
                .WithMessage($"Plik nie może być większy niż {MaxFileSizeInMb}MB.");

            RuleFor(x => x.AvatarFile.ContentType)
                .Must(ct => ct.StartsWith("image/"))
                .WithMessage("Przesłany plik musi być obrazem (np. JPG, PNG).");
        });
    }
}