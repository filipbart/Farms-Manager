export interface FarmPayslipRowModel {
  id: string;
  name: string;
  cycle: CyclePayslipModel;
  employees: EmployeeFarmPayslipModel[];
}

export interface CyclePayslipModel {
  id: string;
  identifier: number;
  year: number;
}

export interface EmployeeFarmPayslipModel {
  id: string;
  fullName: string;
  salary: number;
}
