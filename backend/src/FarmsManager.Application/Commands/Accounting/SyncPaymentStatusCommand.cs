using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record SyncPaymentStatusCommand(
    Guid InvoiceId,
    string Direction,
    string NewPaymentStatus
) : IRequest<EmptyBaseResponse>;

public class SyncPaymentStatusCommandHandler : IRequestHandler<SyncPaymentStatusCommand, EmptyBaseResponse>
{
    private readonly IPaymentStatusSynchronizationService _syncService;

    public SyncPaymentStatusCommandHandler(IPaymentStatusSynchronizationService syncService)
    {
        _syncService = syncService;
    }

    public async Task<EmptyBaseResponse> Handle(
        SyncPaymentStatusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Direction.Equals("ToAccounting", StringComparison.OrdinalIgnoreCase))
        {
            await _syncService.SyncPaymentStatusToAccountingAsync(request.InvoiceId, cancellationToken);
        }
        else if (request.Direction.Equals("FromAccounting", StringComparison.OrdinalIgnoreCase))
        {
            if (!Enum.TryParse<KSeFPaymentStatus>(request.NewPaymentStatus, out var paymentStatus))
            {
                throw new ArgumentException($"Invalid payment status: {request.NewPaymentStatus}");
            }
            
            await _syncService.SyncPaymentStatusFromAccountingAsync(
                request.InvoiceId, 
                paymentStatus, 
                cancellationToken);
        }
        else
        {
            throw new ArgumentException($"Invalid sync direction: {request.Direction}. Use 'ToAccounting' or 'FromAccounting'.");
        }

        return new EmptyBaseResponse();
    }
}
