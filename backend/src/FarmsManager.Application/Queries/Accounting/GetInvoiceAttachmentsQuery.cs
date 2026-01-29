using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting;

public record GetInvoiceAttachmentsQuery(Guid InvoiceId) : IRequest<BaseResponse<List<InvoiceAttachmentItemDto>>>;

public record InvoiceAttachmentItemDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; }
    public long FileSize { get; init; }
    public string ContentType { get; init; }
    public DateTime UploadedAt { get; init; }
    public string UploadedByName { get; init; }
}

public class GetInvoiceAttachmentsQueryHandler : IRequestHandler<GetInvoiceAttachmentsQuery, BaseResponse<List<InvoiceAttachmentItemDto>>>
{
    private readonly IInvoiceAttachmentService _attachmentService;
    private readonly IUserDataResolver _userDataResolver;

    public GetInvoiceAttachmentsQueryHandler(
        IInvoiceAttachmentService attachmentService,
        IUserDataResolver userDataResolver)
    {
        _attachmentService = attachmentService;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<List<InvoiceAttachmentItemDto>>> Handle(
        GetInvoiceAttachmentsQuery request,
        CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var attachments = await _attachmentService.GetInvoiceAttachmentsAsync(
            request.InvoiceId,
            cancellationToken
        );

        var result = attachments.Select(a => new InvoiceAttachmentItemDto
        {
            Id = a.Id,
            FileName = a.FileName,
            FileSize = a.FileSize,
            ContentType = a.ContentType,
            UploadedAt = a.DateCreatedUtc,
            UploadedByName = a.Uploader?.Name ?? "Nieznany"
        }).ToList();

        return BaseResponse.CreateResponse(result);
    }
}
