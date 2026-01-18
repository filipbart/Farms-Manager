using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record DeleteInvoiceAttachmentCommand(Guid InvoiceId, Guid AttachmentId) : IRequest<EmptyBaseResponse>;

public class DeleteInvoiceAttachmentCommandHandler : IRequestHandler<DeleteInvoiceAttachmentCommand, EmptyBaseResponse>
{
    private readonly IInvoiceAttachmentService _attachmentService;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteInvoiceAttachmentCommandHandler(
        IInvoiceAttachmentService attachmentService,
        IUserDataResolver userDataResolver)
    {
        _attachmentService = attachmentService;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(
        DeleteInvoiceAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        await _attachmentService.DeleteAttachmentAsync(request.AttachmentId, cancellationToken);

        return new EmptyBaseResponse();
    }
}
