using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;


namespace FarmsManager.Application.Commands.ProductionData.TransferFeed; 

public record UpdateProductionDataTransferFeedCommandDto
{
    public int RemainingTonnage { get; init; }
    public decimal RemainingValue { get; init; }
}

public record UpdateProductionDataTransferFeedCommand(Guid Id, UpdateProductionDataTransferFeedCommandDto Data)
    : IRequest<EmptyBaseResponse>;

public class
    UpdateProductionDataTransferFeedCommandHandler : IRequestHandler<UpdateProductionDataTransferFeedCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataTransferFeedRepository _repository; 

    public UpdateProductionDataTransferFeedCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataTransferFeedRepository repository) 
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateProductionDataTransferFeedCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var transferFeed =
            await _repository.GetAsync(new ProductionDataTransferFeedByIdSpec(request.Id), cancellationToken); 

        transferFeed.UpdateData(request.Data.RemainingTonnage, request.Data.RemainingValue); 
        transferFeed.SetModified(userId);
        await _repository.UpdateAsync(transferFeed, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public class
    UpdateProductionDataTransferFeedCommandValidator : AbstractValidator<UpdateProductionDataTransferFeedCommand>
{
    public UpdateProductionDataTransferFeedCommandValidator()
    {
        RuleFor(t => t.Data.RemainingTonnage).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.RemainingValue).GreaterThanOrEqualTo(0);
    }
}