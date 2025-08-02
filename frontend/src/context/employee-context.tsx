import { createContext } from "react";
import type { EmployeeDetailsModel } from "../models/employees/employees";

interface EmployeeContextModel {
  employee: EmployeeDetailsModel | undefined;
  refetch: () => void;
  loading: boolean;
}

export const EmployeeContext = createContext<EmployeeContextModel>({
  employee: undefined,
  refetch: () => {},
  loading: false,
} as EmployeeContextModel);
