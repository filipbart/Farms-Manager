using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record UpdateFeedCorrectionDto
{
    public Guid Id { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public decimal InvoiceTotal { get; init; }
    public DateOnly InvoiceDate { get; init; }
}

public record UpdateFeedCorrectionCommand(UpdateFeedCorrectionDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateFeedCorrectionCommandHandler : IRequestHandler<UpdateFeedCorrectionCommand, EmptyBaseResponse>
{
    private const string Comment = "Korekta została ujęta: {0}";

    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;

    public UpdateFeedCorrectionCommandHandler(IUserDataResolver userDataResolver,
        IFeedInvoiceRepository feedInvoiceRepository, IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _userDataResolver = userDataResolver;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateFeedCorrectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var correction = await _feedInvoiceCorrectionRepository.GetAsync(new GetFeedCorrectionByIdSpec(request.Data.Id),
            cancellationToken);

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedInvoicesByCorrectionIdSpec(correction.Id),
                cancellationToken);

        var comment = string.Format(Comment, request.Data.InvoiceNumber);

        correction.Update(request.Data.InvoiceNumber, request.Data.SubTotal, request.Data.VatAmount,
            request.Data.InvoiceTotal, request.Data.InvoiceDate);
        correction.SetModified(userId);

        foreach (var feedInvoiceEntity in feedInvoices)
        {
            feedInvoiceEntity.SetComment(comment);
            feedInvoiceEntity.SetModified(userId);
        }

        await _feedInvoiceRepository.UpdateRangeAsync(feedInvoices, cancellationToken);
        await _feedInvoiceCorrectionRepository.UpdateAsync(correction, cancellationToken);

        return new EmptyBaseResponse();
    }
}