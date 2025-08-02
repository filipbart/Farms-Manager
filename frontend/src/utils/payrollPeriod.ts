import { PayrollPeriod } from "../models/employees/employees-payslips";

export const getCurrentPayrollPeriod = (): PayrollPeriod => {
  const monthIndex = new Date().getMonth();

  const periods: PayrollPeriod[] = [
    PayrollPeriod.January,
    PayrollPeriod.February,
    PayrollPeriod.March,
    PayrollPeriod.April,
    PayrollPeriod.May,
    PayrollPeriod.June,
    PayrollPeriod.July,
    PayrollPeriod.August,
    PayrollPeriod.September,
    PayrollPeriod.October,
    PayrollPeriod.November,
    PayrollPeriod.December,
  ];

  return periods[monthIndex];
};
