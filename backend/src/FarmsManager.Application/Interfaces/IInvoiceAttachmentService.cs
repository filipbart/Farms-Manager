using FarmsManager.Application.Common;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

namespace FarmsManager.Application.Interfaces;

public interface IInvoiceAttachmentService : IService
{
    Task<KSeFInvoiceAttachmentEntity> UploadAttachmentAsync(
        Guid invoiceId,
        string fileName,
        byte[] fileContent,
        string contentType,
        Guid uploadedBy,
        CancellationToken cancellationToken = default);

    Task<(byte[] Content, string FileName, string ContentType)> DownloadAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    Task DeleteAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default);

    Task<List<KSeFInvoiceAttachmentEntity>> GetInvoiceAttachmentsAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}
