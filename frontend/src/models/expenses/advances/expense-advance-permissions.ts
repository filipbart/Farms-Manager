export enum ExpenseAdvancePermissionType {
  View = "View",
  Edit = "Edit",
}

export interface ExpenseAdvanceEntity {
  id: string;
  employeeId: string;
  employeeName: string;
  description?: string;
}

export interface ExpenseAdvancePermission {
  id: string;
  userId: string;
  expenseAdvanceId: string;
  employeeName: string;
  permissionType: ExpenseAdvancePermissionType;
  createdAt: string;
  updatedAt?: string;
}

export interface AssignExpenseAdvancePermissionRequest {
  userId: string;
  expenseAdvanceId: string;
  permissionTypes: ExpenseAdvancePermissionType[];
}

export interface UpdateExpenseAdvancePermissionRequest {
  permissionId: string;
  permissionTypes: ExpenseAdvancePermissionType[];
}

export interface RemoveExpenseAdvancePermissionRequest {
  permissionId: string;
}

export interface UserExpenseAdvancePermissions {
  userId: string;
  permissions: ExpenseAdvancePermission[];
}
