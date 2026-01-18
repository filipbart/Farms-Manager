using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting;

public record DownloadInvoiceAttachmentQuery(Guid InvoiceId, Guid AttachmentId) : IRequest<DownloadInvoiceAttachmentResponse>;

public record DownloadInvoiceAttachmentResponse
{
    public byte[] Content { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }
}

public class DownloadInvoiceAttachmentQueryHandler : IRequestHandler<DownloadInvoiceAttachmentQuery, DownloadInvoiceAttachmentResponse>
{
    private readonly IInvoiceAttachmentService _attachmentService;
    private readonly IUserDataResolver _userDataResolver;

    public DownloadInvoiceAttachmentQueryHandler(
        IInvoiceAttachmentService attachmentService,
        IUserDataResolver userDataResolver)
    {
        _attachmentService = attachmentService;
        _userDataResolver = userDataResolver;
    }

    public async Task<DownloadInvoiceAttachmentResponse> Handle(
        DownloadInvoiceAttachmentQuery request,
        CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var (content, fileName, contentType) = await _attachmentService.DownloadAttachmentAsync(
            request.AttachmentId,
            cancellationToken
        );

        return new DownloadInvoiceAttachmentResponse
        {
            Content = content,
            FileName = fileName,
            ContentType = contentType
        };
    }
}
