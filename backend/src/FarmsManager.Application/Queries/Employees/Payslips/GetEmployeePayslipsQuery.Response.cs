using FarmsManager.Application.Common;
using FarmsManager.Application.Extensions;
using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;

namespace FarmsManager.Application.Queries.Employees.Payslips;

public class EmployeePayslipRowDto
{
    public Guid Id { get; init; }
    public Guid FarmId { get; init; }
    public string FarmName { get; init; }
    public Guid CycleId { get; init; }
    public string CycleText { get; init; }
    public PayrollPeriod PayrollPeriod { get; init; }
    public string PayrollPeriodDesc => PayrollPeriod.GetDescription();
    public string EmployeeFullName { get; init; }

    public decimal BaseSalary { get; init; }
    public decimal BankTransferAmount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal OvertimePay { get; init; }
    public decimal OvertimeHours { get; init; }
    public decimal Deductions { get; init; }
    public decimal OtherAllowances { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal NetPay { get; init; }

    public string Comment { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class EmployeePayslipAggregationDto
{
    public decimal BaseSalary { get; init; }
    public decimal BankTransferAmount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal OvertimePay { get; init; }
    public decimal OvertimeHours { get; init; }
    public decimal Deductions { get; init; }
    public decimal OtherAllowances { get; init; }
    public decimal NetPay { get; init; }
}

public class GetEmployeePayslipsQueryResponse
{
    public PaginationModel<EmployeePayslipRowDto> List { get; init; }
    public EmployeePayslipAggregationDto Aggregation { get; init; }
}