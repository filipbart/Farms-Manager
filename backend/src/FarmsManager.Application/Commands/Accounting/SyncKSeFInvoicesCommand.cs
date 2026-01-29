using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record SyncKSeFInvoicesCommand : IRequest<EmptyBaseResponse>;

public class SyncKSeFInvoicesCommandHandler : IRequestHandler<SyncKSeFInvoicesCommand, EmptyBaseResponse>
{
    private readonly IKSeFSynchronizationJob _ksefSyncJob;

    public SyncKSeFInvoicesCommandHandler(IKSeFSynchronizationJob ksefSyncJob)
    {
        _ksefSyncJob = ksefSyncJob;
    }

    public async Task<EmptyBaseResponse> Handle(SyncKSeFInvoicesCommand request, CancellationToken cancellationToken)
    {
        await _ksefSyncJob.ExecuteSynchronizationAsync(isManual: true, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}
