using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Commands.Expenses.Contractors;

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
    public string Comment { get; init; }
}

public record UpdateExpenseProductionCommand(Guid Id, UpdateExpenseProductionData Data) : IRequest<EmptyBaseResponse>;

public class UpdateExpenseProductionCommandHandler : IRequestHandler<UpdateExpenseProductionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseProductionRepository _expenseProductionRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IExpenseContractorRepository _expenseContractorRepository;

    public UpdateExpenseProductionCommandHandler(IUserDataResolver userDataResolver,
        IExpenseProductionRepository expenseProductionRepository, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IExpenseContractorRepository expenseContractorRepository)
    {
        _userDataResolver = userDataResolver;
        _expenseProductionRepository = expenseProductionRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _expenseContractorRepository = expenseContractorRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateExpenseProductionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity =
            await _expenseProductionRepository.GetAsync(new GetExpenseProductionByIdSpec(request.Id),
                cancellationToken);

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);
        var expenseContractor =
            await _expenseContractorRepository.GetAsync(
                new GetExpenseContractorByIdSpec(request.Data.ExpenseContractorId), cancellationToken);

        if (entity.FarmId != farm.Id)
        {
            entity.SetFarm(farm.Id);
        }

        if (entity.CycleId != cycle.Id)
        {
            entity.SetCycle(cycle.Id);
        }

        if (entity.ExpenseContractorId != expenseContractor.Id)
        {
            entity.SetExpenseContractor(expenseContractor.Id);
        }

        entity.Update(
            request.Data.InvoiceNumber,
            request.Data.InvoiceTotal,
            request.Data.SubTotal,
            request.Data.VatAmount,
            request.Data.InvoiceDate,
            request.Data.Comment
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
        RuleFor(t => t.Data.InvoiceTotal).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.SubTotal).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.VatAmount).GreaterThanOrEqualTo(0);
        RuleFor(t => t.Data.InvoiceDate).NotEmpty();
        RuleFor(t => t.Data.Comment).MaximumLength(500);
    }
}
