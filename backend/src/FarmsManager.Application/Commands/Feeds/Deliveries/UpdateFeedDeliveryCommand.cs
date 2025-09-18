using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record UpdateFeedDeliveryCommandDto
{
    public Guid CycleId { get; init; }
    public string InvoiceNumber { get; init; }
    public string BankAccountNumber { get; init; }
    public string ItemName { get; init; }
    public string VendorName { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public string Comment { get; init; }
}

public record UpdateFeedDeliveryCommand(Guid Id, UpdateFeedDeliveryCommandDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateFeedDeliveryCommandHandler : IRequestHandler<UpdateFeedDeliveryCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedNameRepository _feedNameRepository;
    private readonly IFeedPriceRepository _feedPriceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateFeedDeliveryCommandHandler(IUserDataResolver userDataResolver, IFeedNameRepository feedNameRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IFeedPriceRepository feedPriceRepository,
        ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _feedNameRepository = feedNameRepository;
        _feedPriceRepository = feedPriceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFeedDeliveryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedDelivery =
            await _feedInvoiceRepository.GetAsync(new GetFeedInvoiceByIdSpec(request.Id), cancellationToken);
        var feedName =
            await _feedNameRepository.GetAsync(new GetFeedNameByNameSpec(request.Data.ItemName), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        if (feedDelivery.CycleId != cycle.Id)
        {
            feedDelivery.SetCycle(cycle.Id);
        }

        feedDelivery.Update(
            request.Data.InvoiceNumber,
            request.Data.BankAccountNumber,
            feedName.Name,
            request.Data.VendorName,
            request.Data.Quantity,
            request.Data.UnitPrice,
            request.Data.InvoiceDate,
            request.Data.DueDate,
            request.Data.InvoiceTotal,
            request.Data.SubTotal,
            request.Data.VatAmount,
            request.Data.Comment
        );
        feedDelivery.SetModified(userId);

        await CheckFeedInvoiceUnitPrice(feedDelivery, cancellationToken);

        await _feedInvoiceRepository.UpdateAsync(feedDelivery, cancellationToken);
        return new EmptyBaseResponse();
    }

    private async Task CheckFeedInvoiceUnitPrice(FeedInvoiceEntity feedInvoice, CancellationToken ct)
    {
        var feedPrices =
            await _feedPriceRepository.ListAsync(
                new GetFeedPriceForFeedInvoiceSpec(feedInvoice.FarmId, feedInvoice.CycleId, feedInvoice.ItemName,
                    feedInvoice.InvoiceDate),
                ct);

        feedInvoice.CheckUnitPrice(feedPrices);
    }
}

public class UpdateFeedDeliveryCommandValidator : AbstractValidator<UpdateFeedDeliveryCommand>
{
    public UpdateFeedDeliveryCommandValidator()
    {
        RuleFor(t => t.Data.CycleId).NotEmpty();
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.BankAccountNumber).NotEmpty();
        RuleFor(t => t.Data.ItemName).NotEmpty();
        RuleFor(t => t.Data.VendorName).NotEmpty();
        RuleFor(t => t.Data.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.DueDate).NotEmpty();
        RuleFor(t => t.Data.InvoiceTotal).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.SubTotal).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.VatAmount).GreaterThanOrEqualTo(0);
    }
}