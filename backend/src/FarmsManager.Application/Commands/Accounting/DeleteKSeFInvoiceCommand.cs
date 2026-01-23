using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record DeleteKSeFInvoiceCommand(Guid InvoiceId) : IRequest<EmptyBaseResponse>;

public class DeleteKSeFInvoiceCommandHandler : IRequestHandler<DeleteKSeFInvoiceCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IKSeFInvoiceRepository _ksefInvoiceRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public DeleteKSeFInvoiceCommandHandler(
        IUserDataResolver userDataResolver,
        IKSeFInvoiceRepository ksefInvoiceRepository,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IExpenseProductionRepository expenseProductionRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _userDataResolver = userDataResolver;
        _ksefInvoiceRepository = ksefInvoiceRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        // Get the invoice to delete
        var invoice = await _ksefInvoiceRepository.GetAsync(
            new KSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        if (invoice == null)
            throw DomainException.RecordNotFound($"Invoice with ID {request.InvoiceId} not found");

        // Only allow deletion of non-KSeF invoices (manually uploaded ones)
        if (invoice.InvoiceSource == KSeFInvoiceSource.KSeF)
            throw DomainException.BadRequest("Nie można usunąć faktury z KSeF. Można usuwać tylko faktury dodane ręcznie.");

        // Delete associated module entity if exists
        if (invoice.AssignedEntityInvoiceId.HasValue)
        {
            await DeleteAssociatedModuleEntity(invoice.ModuleType, invoice.AssignedEntityInvoiceId.Value, cancellationToken);
        }

        // Soft delete the KSeF invoice
        invoice.Delete(userId);
        await _ksefInvoiceRepository.UpdateAsync(invoice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }

    private async Task DeleteAssociatedModuleEntity(ModuleType moduleType, Guid entityId, CancellationToken cancellationToken)
    {
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.FirstOrDefaultAsync(
                    new GetFeedInvoiceByIdSpec(entityId), cancellationToken);
                if (feedInvoice != null)
                    await _feedInvoiceRepository.DeleteAsync(feedInvoice, cancellationToken);
                break;
            
            case ModuleType.Gas:
                var gasDelivery = await _gasDeliveryRepository.FirstOrDefaultAsync(
                    new GetGasDeliveryByIdSpec(entityId), cancellationToken);
                if (gasDelivery != null)
                    await _gasDeliveryRepository.DeleteAsync(gasDelivery, cancellationToken);
                break;
            
            case ModuleType.ProductionExpenses:
                var expenseProduction = await _expenseProductionRepository.FirstOrDefaultAsync(
                    new GetExpenseProductionByIdSpec(entityId), cancellationToken);
                if (expenseProduction != null)
                    await _expenseProductionRepository.DeleteAsync(expenseProduction, cancellationToken);
                break;
            
            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                    new SaleInvoiceByIdSpec(entityId), cancellationToken);
                if (saleInvoice != null)
                    await _saleInvoiceRepository.DeleteAsync(saleInvoice, cancellationToken);
                break;
        }
    }
}
