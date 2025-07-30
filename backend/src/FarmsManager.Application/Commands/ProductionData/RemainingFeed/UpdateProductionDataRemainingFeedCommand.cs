using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;


namespace FarmsManager.Application.Commands.ProductionData.RemainingFeed;

public record UpdateProductionDataRemainingFeedCommandDto
{
    public decimal RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
}

public record UpdateProductionDataRemainingFeedCommand(Guid Id, UpdateProductionDataRemainingFeedCommandDto Data)
    : IRequest<EmptyBaseResponse>;

public class
    UpdateProductionDataRemainingFeedCommandHandler : IRequestHandler<UpdateProductionDataRemainingFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataRemainingFeedRepository _repository;

    public UpdateProductionDataRemainingFeedCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataRemainingFeedRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateProductionDataRemainingFeedCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var remainingFeed =
            await _repository.GetAsync(new ProductionDataRemainingFeedByIdSpec(request.Id), cancellationToken);

        remainingFeed.UpdateData(request.Data.RemainingTonnage, request.Data.RemainingValue);
        remainingFeed.SetModified(userId);
        await _repository.UpdateAsync(remainingFeed, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public class
    UpdateProductionDataRemainingFeedCommandValidator : AbstractValidator<UpdateProductionDataRemainingFeedCommand>
{
    public UpdateProductionDataRemainingFeedCommandValidator()
    {
        RuleFor(t => t.Data.RemainingTonnage).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.RemainingValue).GreaterThanOrEqualTo(0);
    }
}