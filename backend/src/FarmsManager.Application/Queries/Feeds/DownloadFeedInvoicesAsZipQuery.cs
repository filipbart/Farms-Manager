using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using MediatR;
using System.IO.Compression;

namespace FarmsManager.Application.Queries.Feeds;

public record DownloadFeedInvoicesAsZipQuery(
    List<Guid> DeliveryIds,
    List<string> CorrectionFilePaths) : IRequest<byte[]>;

public class DownloadFeedInvoicesAsZipQueryHandler : IRequestHandler<DownloadFeedInvoicesAsZipQuery, byte[]>
{
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IS3Service _s3Service;

    public DownloadFeedInvoicesAsZipQueryHandler(
        IFeedInvoiceRepository feedInvoiceRepository,
        IS3Service s3Service)
    {
        _feedInvoiceRepository = feedInvoiceRepository;
        _s3Service = s3Service;
    }

    public async Task<byte[]> Handle(DownloadFeedInvoicesAsZipQuery request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // Pobierz faktury zwykłych dostaw
            if (request.DeliveryIds is { Count: > 0 })
            {
                foreach (var deliveryId in request.DeliveryIds)
                {
                    var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(deliveryId, cancellationToken);
                    
                    if (feedInvoice is null || string.IsNullOrEmpty(feedInvoice.FilePath))
                        continue;

                    var file = await _s3Service.GetFileAsync(FileType.FeedDeliveryInvoice, feedInvoice.FilePath);
                    
                    if (file is not { HasData: true })
                        continue;

                    var extension = Path.GetExtension(feedInvoice.FilePath);
                    var fileName = !string.IsNullOrEmpty(feedInvoice.InvoiceNumber)
                        ? $"faktura_dostawa_{feedInvoice.InvoiceNumber}{extension}"
                        : $"faktura_dostawa_{deliveryId}{extension}";

                    var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    await using var zipEntryStream = zipEntry.Open();
                    await zipEntryStream.WriteAsync(file.Data.AsMemory(0, file.Data.Length), cancellationToken);
                }
            }

            // Pobierz faktury korekt
            if (request.CorrectionFilePaths is { Count: > 0 })
            {
                foreach (var filePath in request.CorrectionFilePaths)
                {
                    if (string.IsNullOrEmpty(filePath))
                        continue;

                    var decodedPath = Uri.UnescapeDataString(filePath);
                    var file = await _s3Service.GetFileAsync(FileType.FeedDeliveryCorrection, decodedPath);
                    
                    if (file is not { HasData: true })
                        continue;

                    var originalFileName = Path.GetFileName(decodedPath);
                    var fileName = $"faktura_korekta_{originalFileName}";

                    var zipEntry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    await using var zipEntryStream = zipEntry.Open();
                    await zipEntryStream.WriteAsync(file.Data.AsMemory(0, file.Data.Length), cancellationToken);
                }
            }
        }

        return memoryStream.ToArray();
    }
}
