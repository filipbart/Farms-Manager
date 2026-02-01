using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
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

        // Get all non-deleted invoices
        var invoices = await _dbContext.Set<KSeFInvoiceEntity>()
            .Where(i => i.DateDeletedUtc == null)
            .ToListAsync(cancellationToken);

        // Hard delete associated module entities for each invoice
        foreach (var invoice in invoices)
        {
            if (invoice.AssignedEntityInvoiceId.HasValue && invoice.ModuleType.HasValue)
            {
                await HardDeleteAssociatedModuleEntity(
                    invoice.ModuleType.Value,
                    invoice.AssignedEntityInvoiceId.Value,
                    invoice.InvoiceNumber,
                    cancellationToken);
            }
        }

        var count = invoices.Count;

        // Hard delete all invoices
        _dbContext.Set<KSeFInvoiceEntity>().RemoveRange(invoices);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return BaseResponse.CreateResponse(new DeleteAllKSeFInvoicesCommandResponse
        {
            DeletedCount = count
        });
    }

    private async Task HardDeleteAssociatedModuleEntity(
        ModuleType moduleType,
        Guid entityId,
        string invoiceNumber,
        CancellationToken cancellationToken)
    {
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _dbContext.Set<FeedInvoiceEntity>()
                    .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);
                if (feedInvoice != null)
                {
                    _dbContext.Set<FeedInvoiceEntity>().Remove(feedInvoice);
                }
                break;

            case ModuleType.Gas:
                var gasDelivery = await _dbContext.Set<GasDeliveryEntity>()
                    .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);
                if (gasDelivery != null)
                {
                    _dbContext.Set<GasDeliveryEntity>().Remove(gasDelivery);
                }

                // Usuń również powiązany wpis w Kosztach Produkcyjnych (tworzony razem z dostawą gazu)
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    var gasExpenseProduction = await _dbContext.Set<ExpenseProductionEntity>()
                        .FirstOrDefaultAsync(e => e.InvoiceNumber == invoiceNumber, cancellationToken);
                    if (gasExpenseProduction != null)
                    {
                        _dbContext.Set<ExpenseProductionEntity>().Remove(gasExpenseProduction);
                    }
                }
                break;

            case ModuleType.ProductionExpenses:
                var expenseProduction = await _dbContext.Set<ExpenseProductionEntity>()
                    .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);
                if (expenseProduction != null)
                {
                    _dbContext.Set<ExpenseProductionEntity>().Remove(expenseProduction);
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _dbContext.Set<SaleInvoiceEntity>()
                    .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);
                if (saleInvoice != null)
                {
                    _dbContext.Set<SaleInvoiceEntity>().Remove(saleInvoice);
                }
                break;
        }
    }
}
