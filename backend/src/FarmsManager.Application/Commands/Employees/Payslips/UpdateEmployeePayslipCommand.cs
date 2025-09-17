using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Employees;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Employees.Payslips;

public record UpdateEmployeePayslipData
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public PayrollPeriod PayrollPeriod { get; init; }
    public decimal BaseSalary { get; init; }
    public decimal BankTransferAmount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal OvertimePay { get; init; }
    public decimal OvertimeHours { get; init; }
    public decimal Deductions { get; init; }
    public decimal OtherAllowances { get; init; }
    public string Comment { get; init; }
}

public record UpdateEmployeePayslipCommand(Guid Id, UpdateEmployeePayslipData Data) : IRequest<EmptyBaseResponse>;

public class UpdateEmployeePayslipCommandValidator : AbstractValidator<UpdateEmployeePayslipCommand>
{
    public UpdateEmployeePayslipCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID wpisu wypłaty jest wymagane.");
        RuleFor(x => x.Data).NotNull();

        RuleFor(x => x.Data.FarmId).NotEmpty().WithMessage("Farma jest wymagana.");
        RuleFor(x => x.Data.CycleId).NotEmpty().WithMessage("Cykl jest wymagany.");
        RuleFor(x => x.Data.PayrollPeriod).IsInEnum().WithMessage("Okres wypłaty jest nieprawidłowy.");

        RuleFor(x => x.Data.BaseSalary).GreaterThanOrEqualTo(0).WithMessage("Pensja podstawowa nie może być ujemna.");
        RuleFor(x => x.Data.BankTransferAmount).GreaterThanOrEqualTo(0)
            .WithMessage("Kwota przelewu nie może być ujemna.");
        RuleFor(x => x.Data.BonusAmount).GreaterThanOrEqualTo(0).WithMessage("Premia nie może być ujemna.");
        RuleFor(x => x.Data.OvertimePay).GreaterThanOrEqualTo(0)
            .WithMessage("Wynagrodzenie za nadgodziny nie może być ujemne.");
        RuleFor(x => x.Data.OvertimeHours).GreaterThanOrEqualTo(0).WithMessage("Liczba nadgodzin nie może być ujemna.");
        RuleFor(x => x.Data.Deductions).GreaterThanOrEqualTo(0).WithMessage("Potrącenia nie mogą być ujemne.");
        RuleFor(x => x.Data.OtherAllowances).GreaterThanOrEqualTo(0).WithMessage("Inne dodatki nie mogą być ujemne.");
    }
}

public class UpdateEmployeePayslipCommandHandler : IRequestHandler<UpdateEmployeePayslipCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEmployeePayslipRepository _employeePayslipRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateEmployeePayslipCommandHandler(
        IUserDataResolver userDataResolver,
        IEmployeePayslipRepository employeePayslipRepository,
        IFarmRepository farmRepository,
        ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _employeePayslipRepository = employeePayslipRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateEmployeePayslipCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var payslip = await _employeePayslipRepository.GetAsync(new GetPayslipByIdSpec(request.Id), cancellationToken);

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        var netPay = request.Data.BaseSalary -
                     request.Data.BankTransferAmount +
                     request.Data.BonusAmount +
                     request.Data.OvertimePay -
                     request.Data.Deductions +
                     request.Data.OtherAllowances;

        payslip.Update(
            farm,
            cycle,
            request.Data.PayrollPeriod,
            request.Data.BaseSalary,
            request.Data.BankTransferAmount,
            request.Data.BonusAmount,
            request.Data.OvertimePay,
            request.Data.OvertimeHours,
            request.Data.Deductions,
            request.Data.OtherAllowances,
            netPay,
            request.Data.Comment
        );

        payslip.SetModified(userId);

        await _employeePayslipRepository.UpdateAsync(payslip, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}