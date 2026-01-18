using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

public class KSeFInvoiceAttachmentEntity : Entity
{
    protected KSeFInvoiceAttachmentEntity()
    {
    }

    public static KSeFInvoiceAttachmentEntity CreateNew(
        Guid invoiceId,
        string fileName,
        string filePath,
        long fileSize,
        string contentType,
        Guid uploadedBy)
    {
        return new KSeFInvoiceAttachmentEntity
        {
            InvoiceId = invoiceId,
            FileName = fileName,
            FilePath = filePath,
            FileSize = fileSize,
            ContentType = contentType,
            UploadedBy = uploadedBy,
            CreatedBy = uploadedBy
        };
    }

    /// <summary>
    /// Identyfikator faktury, do której należy załącznik
    /// </summary>
    public Guid InvoiceId { get; init; }

    /// <summary>
    /// Faktura, do której należy załącznik
    /// </summary>
    public virtual KSeFInvoiceEntity Invoice { get; init; }

    /// <summary>
    /// Oryginalna nazwa pliku
    /// </summary>
    public string FileName { get; init; }

    /// <summary>
    /// Ścieżka do pliku w storage (S3)
    /// </summary>
    public string FilePath { get; init; }

    /// <summary>
    /// Rozmiar pliku w bajtach
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// Typ MIME pliku
    /// </summary>
    public string ContentType { get; init; }

    /// <summary>
    /// Identyfikator użytkownika, który przesłał plik
    /// </summary>
    public Guid UploadedBy { get; init; }

    /// <summary>
    /// Użytkownik, który przesłał plik
    /// </summary>
    public virtual UserEntity Uploader { get; init; }
}
