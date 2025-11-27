using FarmsManager.Application.Interfaces;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record SendTestKSeFInvoiceCommand(string FileContent = null) : IRequest<string>;

public class SendTestKSeFInvoiceCommandHandler : IRequestHandler<SendTestKSeFInvoiceCommand, string>
{
    private readonly IKSeFService _ksefService;

    public SendTestKSeFInvoiceCommandHandler(IKSeFService ksefService)
    {
        _ksefService = ksefService;
    }

    public async Task<string> Handle(SendTestKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        return await _ksefService.SendTestInvoiceAsync(request.FileContent, cancellationToken);
    }
}