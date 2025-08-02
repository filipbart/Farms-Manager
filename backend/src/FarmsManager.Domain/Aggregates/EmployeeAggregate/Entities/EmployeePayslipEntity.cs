using FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;

public class EmployeePayslipEntity : Entity
{
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public Guid EmployeeId { get; init; }

    public PayrollPeriod PayrollPeriod { get; protected internal set; }
    public decimal BaseSalary { get; protected internal set; }
    public decimal BankTransferAmount { get; protected internal set; }
    public decimal BonusAmount { get; protected internal set; }
    public decimal OvertimePay { get; protected internal set; }
    public decimal OvertimeHours { get; protected internal set; }
    public decimal Deductions { get; protected internal set; }
    public decimal OtherAllowances { get; protected internal set; }
    public decimal NetPay { get; protected internal set; }
    public string Comment { get; protected internal set; }

    public virtual FarmEntity Farm { get; set; }
    public virtual CycleEntity Cycle { get; set; }
    public virtual EmployeeEntity Employee { get; set; }

    /// <summary>
    /// Metoda fabryczna do tworzenia nowego wpisu wypłaty.
    /// </summary>
    public static EmployeePayslipEntity CreateNew(
        Guid farmId,
        Guid cycleId,
        Guid employeeId,
        PayrollPeriod payrollPeriod,
        decimal baseSalary,
        decimal bankTransferAmount,
        decimal bonusAmount,
        decimal overtimePay,
        decimal overtimeHours,
        decimal deductions,
        decimal otherAllowances,
        decimal netPay,
        string comment,
        Guid? userId = null)
    {
        return new EmployeePayslipEntity
        {
            FarmId = farmId,
            CycleId = cycleId,
            EmployeeId = employeeId,
            PayrollPeriod = payrollPeriod,
            BaseSalary = baseSalary,
            BankTransferAmount = bankTransferAmount,
            BonusAmount = bonusAmount,
            OvertimePay = overtimePay,
            OvertimeHours = overtimeHours,
            Deductions = deductions,
            OtherAllowances = otherAllowances,
            NetPay = netPay,
            Comment = comment,
            CreatedBy = userId
        };
    }

    /// <summary>
    /// Metoda do aktualizacji istniejącego wpisu wypłaty.
    /// </summary>
    public void Update(
        decimal baseSalary,
        decimal bankTransferAmount,
        decimal bonusAmount,
        decimal overtimePay,
        decimal overtimeHours,
        decimal deductions,
        decimal otherAllowances,
        decimal netPay,
        string comment)
    {
        BaseSalary = baseSalary;
        BankTransferAmount = bankTransferAmount;
        BonusAmount = bonusAmount;
        OvertimePay = overtimePay;
        OvertimeHours = overtimeHours;
        Deductions = deductions;
        OtherAllowances = otherAllowances;
        NetPay = netPay;
        Comment = comment;
    }
}