using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Accounting;

public record UploadInvoiceAttachmentCommand(Guid InvoiceId, IFormFile File) : IRequest<BaseResponse<UploadInvoiceAttachmentResponse>>;

public record UploadInvoiceAttachmentResponse
{
    public Guid Id { get; init; }
    public string FileName { get; init; }
    public long FileSize { get; init; }
    public string ContentType { get; init; }
    public DateTime UploadedAt { get; init; }
}

public class UploadInvoiceAttachmentCommandHandler : IRequestHandler<UploadInvoiceAttachmentCommand, BaseResponse<UploadInvoiceAttachmentResponse>>
{
    private readonly IInvoiceAttachmentService _attachmentService;
    private readonly IUserDataResolver _userDataResolver;

    public UploadInvoiceAttachmentCommandHandler(
        IInvoiceAttachmentService attachmentService,
        IUserDataResolver userDataResolver)
    {
        _attachmentService = attachmentService;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<UploadInvoiceAttachmentResponse>> Handle(
        UploadInvoiceAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream, cancellationToken);
        var fileContent = memoryStream.ToArray();

        var attachment = await _attachmentService.UploadAttachmentAsync(
            request.InvoiceId,
            request.File.FileName,
            fileContent,
            request.File.ContentType,
            userId,
            cancellationToken
        );

        return BaseResponse.CreateResponse(new UploadInvoiceAttachmentResponse
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            FileSize = attachment.FileSize,
            ContentType = attachment.ContentType,
            UploadedAt = attachment.DateCreatedUtc
        });
    }
}
