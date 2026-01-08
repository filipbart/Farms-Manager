using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Commands.Accounting;

public record DeleteAllKSeFInvoicesCommandResponse
{
    public int DeletedCount { get; init; }
}

public record DeleteAllKSeFInvoicesCommand : IRequest<BaseResponse<DeleteAllKSeFInvoicesCommandResponse>>;

public class DeleteAllKSeFInvoicesCommandHandler : IRequestHandler<DeleteAllKSeFInvoicesCommand,
    BaseResponse<DeleteAllKSeFInvoicesCommandResponse>>
{
    private readonly DbContext _dbContext;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteAllKSeFInvoicesCommandHandler(
        DbContext dbContext,
        IUserDataResolver userDataResolver)
    {
        _dbContext = dbContext;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<DeleteAllKSeFInvoicesCommandResponse>> Handle(
        DeleteAllKSeFInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        // Hard delete all invoices (for testing purposes only)
        var invoices = await _dbContext.Set<KSeFInvoiceEntity>()
            .Where(i => i.DateDeletedUtc == null)
            .ToListAsync(cancellationToken);

        var count = invoices.Count;

        _dbContext.Set<KSeFInvoiceEntity>().RemoveRange(invoices);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(new DeleteAllKSeFInvoicesCommandResponse
        {
            DeletedCount = count
        });
    }
}
