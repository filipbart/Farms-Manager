using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Accounting;

public record RejectKSeFInvoiceCommand(Guid InvoiceId) : IRequest<EmptyBaseResponse>;

public class RejectKSeFInvoiceCommandHandler : IRequestHandler<RejectKSeFInvoiceCommand, EmptyBaseResponse>
{
    private readonly IKSeFInvoiceRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInvoiceAuditService _auditService;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public RejectKSeFInvoiceCommandHandler(
        IKSeFInvoiceRepository repository,
        IUserDataResolver userDataResolver,
        IInvoiceAuditService auditService,
        IFeedInvoiceRepository feedInvoiceRepository,
        IGasDeliveryRepository gasDeliveryRepository,
        IExpenseProductionRepository expenseProductionRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _auditService = auditService;
        _feedInvoiceRepository = feedInvoiceRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
        _expenseProductionRepository = expenseProductionRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(RejectKSeFInvoiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var userName = _userDataResolver.GetLoginAsync();
        var invoice = await _repository.GetAsync(new KSeFInvoiceByIdSpec(request.InvoiceId), cancellationToken);

        var previousStatus = invoice.Status;

        // Delete module entity if it exists
        if (invoice.AssignedEntityInvoiceId.HasValue)
        {
            await DeleteModuleEntityAsync(invoice.ModuleType, invoice.AssignedEntityInvoiceId.Value, cancellationToken);
            invoice.ClearAssignedEntityInvoiceId();
        }

        // Change status to Rejected
        invoice.Update(
            status: KSeFInvoiceStatus.Rejected,
            paymentStatus: null,
            paymentDate: null,
            dueDate: null,
            moduleType: null,
            vatDeductionType: null,
            comment: null,
            farmId: null,
            cycleId: null,
            assignedUserId: null,
            relatedInvoiceNumber: null
        );

        invoice.SetModified(userId);
        await _repository.UpdateAsync(invoice, cancellationToken);

        // Log audit
        await _auditService.LogStatusChangeAsync(
            invoice.Id,
            KSeFInvoiceAuditAction.Rejected,
            previousStatus,
            KSeFInvoiceStatus.Rejected,
            userId,
            userName,
            cancellationToken: cancellationToken);

        return BaseResponse.EmptyResponse;
    }

    private async Task DeleteModuleEntityAsync(ModuleType moduleType, Guid entityId, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        switch (moduleType)
        {
            case ModuleType.Feeds:
                var feedInvoice = await _feedInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (feedInvoice != null)
                {
                    // Soft delete without removing file (file is in AccountingInvoice folder)
                    feedInvoice.Delete(userId);
                    await _feedInvoiceRepository.UpdateAsync(feedInvoice, cancellationToken);
                }
                break;

            case ModuleType.Gas:
                var gasDelivery = await _gasDeliveryRepository.GetByIdAsync(entityId, cancellationToken);
                if (gasDelivery != null)
                {
                    // Soft delete without removing file (file is in AccountingInvoice folder)
                    gasDelivery.Delete(userId);
                    await _gasDeliveryRepository.UpdateAsync(gasDelivery, cancellationToken);
                }
                break;

            case ModuleType.ProductionExpenses:
                var expense = await _expenseProductionRepository.GetByIdAsync(entityId, cancellationToken);
                if (expense != null)
                {
                    // Soft delete without removing file (file is in AccountingInvoice folder)
                    expense.Delete(userId);
                    await _expenseProductionRepository.UpdateAsync(expense, cancellationToken);
                }
                break;

            case ModuleType.Sales:
                var saleInvoice = await _saleInvoiceRepository.GetByIdAsync(entityId, cancellationToken);
                if (saleInvoice != null)
                {
                    // Soft delete without removing file (file is in AccountingInvoice folder)
                    saleInvoice.Delete(userId);
                    await _saleInvoiceRepository.UpdateAsync(saleInvoice, cancellationToken);
                }
                break;
        }
    }
}
