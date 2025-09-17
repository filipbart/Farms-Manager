import type { PaginateModel } from "../../common/interfaces/paginate";

export interface EmployeePayslipListModel {
  id: string;
  farmId: string;
  farmName: string;
  cycleId: string;
  cycleText: string;
  payrollPeriod: PayrollPeriod;
  payrollPeriodDesc: string;
  employeeFullName: string;

  baseSalary: number; // pensja podstawowa
  bankTransferAmount: number; // kwota przelana na konto
  bonusAmount: number; // premia
  overtimePay: number; // kwota za nadgodziny
  overtimeHours: number; // liczba nadgodzin
  deductions: number; // potrącenia
  otherAllowances: number; // inne dodatki
  netPay: number; // kwota do wypłaty (netto)

  comment?: string;
  dateCreatedUtc: string;
}

export interface EmployeePayslipAggregation {
  baseSalary: number;
  bankTransferAmount: number;
  bonusAmount: number;
  overtimePay: number;
  overtimeHours: number;
  deductions: number;
  otherAllowances: number;
  netPay: number;
}

export interface GetEmployeePayslipsResponse {
  list: PaginateModel<EmployeePayslipListModel>;
  aggregation: EmployeePayslipAggregation;
}

export enum PayrollPeriod {
  January = "January",
  February = "February",
  March = "March",
  April = "April",
  May = "May",
  June = "June",
  July = "July",
  August = "August",
  September = "September",
  October = "October",
  November = "November",
  December = "December",
}

export interface AddEmployeePayslipData {
  farmId: string;
  cycleId: string;
  payrollPeriod: PayrollPeriod;
  entries: AddEmployeePayslipEntry[];
}

export interface AddEmployeePayslipEntry {
  employeeId: string;

  baseSalary: number;
  bankTransferAmount: number;
  bonusAmount: number;
  overtimePay: number;
  overtimeHours: number;
  deductions: number;
  otherAllowances: number;

  comment?: string;
}

export interface UpdateEmployeePayslip {
  farmId: string;
  cycleId: string;
  payrollPeriod: PayrollPeriod;
  baseSalary: number;
  bankTransferAmount: number;
  bonusAmount: number;
  overtimePay: number;
  overtimeHours: number;
  deductions: number;
  otherAllowances: number;

  comment?: string;
}
