using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Shared.Helpers;
using MediatR;

namespace FarmsManager.Application.Queries.Feeds;

public record GetFeedDeliveryFileQuery(Guid FeedDeliveryId) : IRequest<FileModel>;

public class GetFeedDeliveryFileQueryHandler : IRequestHandler<GetFeedDeliveryFileQuery, FileModel>
{
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IS3Service _s3Service;

    public GetFeedDeliveryFileQueryHandler(IFeedInvoiceRepository feedInvoiceRepository, IS3Service s3Service)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
        _s3Service = s3Service;
    }

    public async Task<FileModel> Handle(GetFeedDeliveryFileQuery request, CancellationToken cancellationToken)
    {
        var feedInvoice =
            await _feedInvoiceRepository.GetAsync(new GetFeedInvoiceByIdSpec(request.FeedDeliveryId),
                cancellationToken);

        var contentType = FileHelper.GetFileContentType(feedInvoice.FilePath);
        var extension = Path.GetExtension(feedInvoice.FilePath);
        var fileName = $"Faktura_{feedInvoice.InvoiceNumber}{extension}";

        var file = await _s3Service.GetFileAsync(FileType.FeedDeliveryInvoice, feedInvoice.FilePath);
        file.FileName = fileName;
        file.ContentType = contentType;

        return file;
    }
}