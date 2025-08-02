export interface EmployeeListModel {
  id: string;
  farmName: string;
  fullName: string;
  position: string;
  contractType: string;
  salary: number;
  startDate: string;
  endDate: string;
  status: EmployeeStatus;
  statusDesc: string;
  comment: string;
  files: EmployeeFileModel[];
}

export enum EmployeeStatus {
  Active = "Active",
  Inactive = "Inactive",
}

export interface EmployeeFileModel {
  fileName: string;
  filePath: string;
}

export interface AddEmployeeData {
  farmId: string;
  fullName: string;
  position: string;
  contractType: string;
  salary: number;
  startDate: string;
  endDate?: string;
  comment?: string;
}

export interface EmployeeReminderModel {
  id: string;
  title: string;
  dueDate: string;
}

export interface EmployeeDetailsModel {
  id: string;
  farmId: string;
  farmName: string;
  fullName: string;
  position: string;
  contractType: string;
  salary: number;
  startDate: string;
  endDate: string;
  status: EmployeeStatus;
  statusDesc: string;
  comment: string;
  files: EmployeeFileModel[];
  reminders: EmployeeReminderModel[];
}
