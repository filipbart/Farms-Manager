using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record DeleteFeedDeliveryCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteFeedDeliveryCommandHandler : IRequestHandler<DeleteFeedDeliveryCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IS3Service _s3Service;

    public DeleteFeedDeliveryCommandHandler(IUserDataResolver userDataResolver,
        IFeedInvoiceRepository feedInvoiceRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _feedInvoiceRepository = feedInvoiceRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteFeedDeliveryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var feedDelivery =
            await _feedInvoiceRepository.GetAsync(new GetFeedInvoiceByIdSpec(request.Id), cancellationToken);

        feedDelivery.Delete(userId);
        await _s3Service.DeleteFileAsync(FileType.FeedDeliveryInvoice, feedDelivery.FilePath);
        await _feedInvoiceRepository.UpdateAsync(feedDelivery, cancellationToken);

        return new EmptyBaseResponse();
    }
}