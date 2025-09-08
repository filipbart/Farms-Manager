using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record UpdateExpenseProductionData
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid ExpenseContractorId { get; init; }
    public string InvoiceNumber { get; init; }
    public decimal InvoiceTotal { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public DateOnly InvoiceDate { get; init; }
}

public record UpdateExpenseProductionCommand(Guid Id, UpdateExpenseProductionData Data) : IRequest<EmptyBaseResponse>;

public class UpdateExpenseProductionCommandHandler : IRequestHandler<UpdateExpenseProductionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseProductionRepository _expenseProductionRepository;

    public UpdateExpenseProductionCommandHandler(IUserDataResolver userDataResolver,
        IExpenseProductionRepository expenseProductionRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseProductionRepository = expenseProductionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateExpenseProductionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseProductionRepository.GetAsync(new GetExpenseProductionByIdSpec(request.Id),
                cancellationToken);

        if (entity.FarmId != request.Data.FarmId)
        {
            entity.SetFarm(request.Data.FarmId);
        }

        if (entity.CycleId != request.Data.CycleId)
        {
            entity.SetCycle(request.Data.CycleId);
        }

        if (entity.ExpenseContractorId != request.Data.ExpenseContractorId)
        {
            entity.SetExpenseContractor(request.Data.ExpenseContractorId);
        }

        entity.Update(
            request.Data.InvoiceNumber,
            request.Data.InvoiceTotal,
            request.Data.SubTotal,
            request.Data.VatAmount,
            request.Data.InvoiceDate
        );

        entity.SetModified(userId);
        await _expenseProductionRepository.UpdateAsync(entity, cancellationToken);
        return BaseResponse.EmptyResponse;
    }
}

public class UpdateExpenseProductionCommandValidator : AbstractValidator<UpdateExpenseProductionCommand>
{
    public UpdateExpenseProductionCommandValidator()
    {
        RuleFor(t => t.Data.FarmId).NotEmpty().WithMessage("Ferma jest wymagana.");
        RuleFor(t => t.Data.CycleId).NotEmpty().WithMessage("Cykl jest wymagany.");
        RuleFor(t => t.Data.ExpenseContractorId).NotEmpty().WithMessage("Kontrahent jest wymagany.");
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.InvoiceTotal).GreaterThan(0);
        RuleFor(t => t.Data.SubTotal).GreaterThan(0);
        RuleFor(t => t.Data.VatAmount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
    }
}