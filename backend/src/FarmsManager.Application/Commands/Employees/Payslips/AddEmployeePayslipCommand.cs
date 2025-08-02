using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees.Payslips;

public record AddEmployeePayslipEntry
{
    public Guid EmployeeId { get; init; }
    public decimal BaseSalary { get; init; }
    public decimal BankTransferAmount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal OvertimePay { get; init; }
    public decimal OvertimeHours { get; init; }
    public decimal Deductions { get; init; }
    public decimal OtherAllowances { get; init; }
    public string Comment { get; init; }
}

public record AddEmployeePayslipCommand : IRequest<EmptyBaseResponse>
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public PayrollPeriod PayrollPeriod { get; init; }
    public List<AddEmployeePayslipEntry> Entries { get; init; } = new();
}

public class AddEmployeePayslipCommandValidator : AbstractValidator<AddEmployeePayslipCommand>
{
    public AddEmployeePayslipCommandValidator()
    {
        RuleFor(x => x.FarmId).NotEmpty().WithMessage("Ferma jest wymagana.");
        RuleFor(x => x.CycleId).NotEmpty().WithMessage("Cykl jest wymagany.");
        RuleFor(x => x.PayrollPeriod).IsInEnum().WithMessage("Okres rozliczeniowy jest wymagany.");
        RuleFor(x => x.Entries).NotEmpty().WithMessage("Należy dodać przynajmniej jednego pracownika.");

        RuleForEach(x => x.Entries).SetValidator(new AddEmployeePayslipEntryValidator());
    }
}

public class AddEmployeePayslipEntryValidator : AbstractValidator<AddEmployeePayslipEntry>
{
    public AddEmployeePayslipEntryValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty().WithMessage("Pracownik jest wymagany.");
        RuleFor(x => x.BaseSalary).GreaterThanOrEqualTo(0).WithMessage("Pensja podstawowa nie może być ujemna.");
        RuleFor(x => x.BankTransferAmount).GreaterThanOrEqualTo(0).WithMessage("Kwota przelewu nie może być ujemna.");
        RuleFor(x => x.BonusAmount).GreaterThanOrEqualTo(0).WithMessage("Premia nie może być ujemna.");
        RuleFor(x => x.OvertimePay).GreaterThanOrEqualTo(0)
            .WithMessage("Wynagrodzenie za nadgodziny nie może być ujemne.");
        RuleFor(x => x.OvertimeHours).GreaterThanOrEqualTo(0).WithMessage("Liczba nadgodzin nie może być ujemna.");
        RuleFor(x => x.Deductions).GreaterThanOrEqualTo(0).WithMessage("Potrącenia nie mogą być ujemne.");
        RuleFor(x => x.OtherAllowances).GreaterThanOrEqualTo(0).WithMessage("Inne dodatki nie mogą być ujemne.");
    }
}

public class AddEmployeePayslipCommandHandler : IRequestHandler<AddEmployeePayslipCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;

    public AddEmployeePayslipCommandHandler(
        IUserDataResolver userDataResolver,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository,
        IEmployeeRepository employeeRepository,
        IEmployeePayslipRepository employeePayslipRepository)
    {
        _userDataResolver = userDataResolver;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _employeeRepository = employeeRepository;
        _employeePayslipRepository = employeePayslipRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddEmployeePayslipCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), cancellationToken);

        var payslipsToAdd = new List<EmployeePayslipEntity>();

        foreach (var entry in request.Entries)
        {
            var employee =
                await _employeeRepository.GetAsync(new EmployeeByIdSpec(entry.EmployeeId), cancellationToken);
            var existingPayslip = await _employeePayslipRepository.AnyAsync(
                new GetPayslipByEmployeeAndPeriodSpec(farm.Id, entry.EmployeeId, cycle.Id, request.PayrollPeriod),
                cancellationToken);

            if (existingPayslip)
            {
                throw new Exception(
                    $"Wpis dla pracownika '{employee?.FullName}' w tym okresie rozliczeniowym już istnieje.");
            }

            var netPay = entry.BaseSalary -
                         entry.BankTransferAmount +
                         entry.BonusAmount +
                         entry.OvertimePay -
                         entry.Deductions +
                         entry.OtherAllowances;
            var newPayslip = EmployeePayslipEntity.CreateNew(
                farm.Id,
                cycle.Id,
                employee.Id,
                request.PayrollPeriod,
                entry.BaseSalary,
                entry.BankTransferAmount,
                entry.BonusAmount,
                entry.OvertimePay,
                entry.OvertimeHours,
                entry.Deductions,
                entry.OtherAllowances,
                netPay,
                entry.Comment,
                userId
            );
            payslipsToAdd.Add(newPayslip);
        }

        await _employeePayslipRepository.AddRangeAsync(payslipsToAdd, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class GetPayslipByEmployeeAndPeriodSpec : BaseSpecification<EmployeePayslipEntity>,
    ISingleResultSpecification<EmployeePayslipEntity>
{
    public GetPayslipByEmployeeAndPeriodSpec(Guid farmId, Guid employeeId, Guid cycleId, PayrollPeriod payrollPeriod)
    {
        EnsureExists();
        Query.Where(p => p.FarmId == farmId);
        Query.Where(p => p.EmployeeId == employeeId);
        Query.Where(p => p.CycleId == cycleId);
        Query.Where(p => p.PayrollPeriod == payrollPeriod);
    }
}