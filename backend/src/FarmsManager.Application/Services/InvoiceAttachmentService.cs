using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Accounting;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;

namespace FarmsManager.Application.Services;

public class InvoiceAttachmentService : IInvoiceAttachmentService
{
    private readonly IKSeFInvoiceAttachmentRepository _attachmentRepository;
    private readonly IS3Service _s3Service;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public InvoiceAttachmentService(
        IKSeFInvoiceAttachmentRepository attachmentRepository,
        IS3Service s3Service)
    {
        _attachmentRepository = attachmentRepository;
        _s3Service = s3Service;
    }

    public async Task<KSeFInvoiceAttachmentEntity> UploadAttachmentAsync(
        Guid invoiceId,
        string fileName,
        byte[] fileContent,
        string contentType,
        Guid uploadedBy,
        CancellationToken cancellationToken = default)
    {
        if (fileContent.Length > MaxFileSizeBytes)
            throw DomainException.FileSizeLimitExceeded();

        var sanitizedFileName = SanitizeFileName(fileName);
        var filePath = $"{invoiceId}/{Guid.NewGuid()}/{sanitizedFileName}";

        await _s3Service.UploadFileAsync(
            fileContent,
            FileType.AccountingInvoiceAttachment,
            filePath,
            cancellationToken
        );

        var attachment = KSeFInvoiceAttachmentEntity.CreateNew(
            invoiceId: invoiceId,
            fileName: sanitizedFileName,
            filePath: filePath,
            fileSize: fileContent.Length,
            contentType: contentType,
            uploadedBy: uploadedBy
        );

        await _attachmentRepository.AddAsync(attachment, cancellationToken);
        await _attachmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return attachment;
    }

    public async Task<(byte[] Content, string FileName, string ContentType)> DownloadAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.FirstOrDefaultAsync(
            new KSeFInvoiceAttachmentByIdSpec(attachmentId),
            cancellationToken);

        if (attachment == null)
            throw DomainException.FileNotFound();

        var file = await _s3Service.GetFileAsync(
            FileType.AccountingInvoiceAttachment,
            attachment.FilePath
        );

        return (file.Data, attachment.FileName, attachment.ContentType);
    }

    public async Task DeleteAttachmentAsync(
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        var attachment = await _attachmentRepository.FirstOrDefaultAsync(
            new KSeFInvoiceAttachmentByIdSpec(attachmentId),
            cancellationToken);

        if (attachment == null)
            throw DomainException.FileNotFound();

        await _s3Service.DeleteFileAsync(
            FileType.AccountingInvoiceAttachment,
            attachment.FilePath
        );

        await _attachmentRepository.DeleteAsync(attachment, cancellationToken);
        await _attachmentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<KSeFInvoiceAttachmentEntity>> GetInvoiceAttachmentsAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await _attachmentRepository.ListAsync(
            new KSeFInvoiceAttachmentByInvoiceIdSpec(invoiceId),
            cancellationToken);
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Where(c => !invalidChars.Contains(c)));
    }
}
