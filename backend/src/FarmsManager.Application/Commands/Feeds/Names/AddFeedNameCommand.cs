using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Names;

public record AddFeedNameCommand : IRequest<EmptyBaseResponse>
{
    public string[] Names { get; set; }
}

public class AddFeedNameCommandHandler : IRequestHandler<AddFeedNameCommand, EmptyBaseResponse>
{
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddFeedNameCommandHandler(IFeedNameRepository feedNameRepository,
        IUserDataResolver userDataResolver)
    {
        _feedNameRepository = feedNameRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddFeedNameCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var items = request.Names.Select(field => FeedNameEntity.CreateNew(field, userId)).ToList();

        await _feedNameRepository.AddRangeAsync(items, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AddFeedNameCommandValidator : AbstractValidator<AddFeedNameCommand>
{
    public AddFeedNameCommandValidator()
    {
        RuleFor(x => x.Names)
            .NotEmpty()
            .WithMessage("Lista nazw nie może byc pusta");
        RuleFor(x => x.Names)
            .Must(BeUniqueNormalized)
            .WithMessage("Lista zawiera zduplikowane nazwy (po uwzględnieniu spacji i wielkości liter)");
    }

    private static bool BeUniqueNormalized(string[] names)
    {
        if (names == null) return true;

        var normalized = names
            .Select(Normalize)
            .ToList();

        return normalized.Distinct().Count() == normalized.Count;
    }

    private static string Normalize(string input)
    {
        return input?.Trim().ToLowerInvariant().Replace(" ", "");
    }
}