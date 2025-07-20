using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record UpdateFeedDeliveryCommandDto
{
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
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public UpdateFeedDeliveryCommandHandler(IUserDataResolver userDataResolver, IFeedNameRepository feedNameRepository,
        IFeedInvoiceRepository feedInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _feedNameRepository = feedNameRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFeedDeliveryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedDelivery =
            await _feedInvoiceRepository.GetAsync(new GetFeedInvoiceByIdSpec(request.Id), cancellationToken);
        var feedName =
            await _feedNameRepository.GetAsync(new GetFeedNameByNameSpec(request.Data.ItemName), cancellationToken);

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

        await _feedInvoiceRepository.UpdateAsync(feedDelivery, cancellationToken);
        return new EmptyBaseResponse();
    }
}

public class UpdateFeedDeliveryCommandValidator : AbstractValidator<UpdateFeedDeliveryCommand>
{
    public UpdateFeedDeliveryCommandValidator()
    {
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