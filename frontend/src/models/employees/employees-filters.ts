import type { FarmDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";
import type { EmployeeStatus } from "./employees";

export const initialFilters: EmployeesFilterPaginationModel = {
  farmIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: EmployeesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof EmployeesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<EmployeesFilterPaginationModel>;
      }
): EmployeesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum EmployeesOrderType {
  Farm = "Farm",
  FullName = "FullName",
  Position = "Position",
  ContractType = "ContractType",
  Salary = "Salary",
  StartDate = "StartDate",
  EndDate = "EndDate",
  Status = "Status",
}

export const mapEmployeeOrderTypeToField = (
  orderType: EmployeesOrderType
): string => {
  switch (orderType) {
    case EmployeesOrderType.FullName:
      return "fullName";
    case EmployeesOrderType.Position:
      return "position";
    case EmployeesOrderType.ContractType:
      return "contractType";
    case EmployeesOrderType.Salary:
      return "salary";
    case EmployeesOrderType.StartDate:
      return "startDate";
    case EmployeesOrderType.EndDate:
      return "endDate";
    case EmployeesOrderType.Status:
      return "status";
    default:
      return "";
  }
};

export interface EmployeesFilter {
  farmIds: string[];
  searchPhrase?: string;
  status?: EmployeeStatus;
  addToAdvances?: boolean;
  dateSince: string;
  dateTo: string;
}

export interface EmployeesFilterPaginationModel
  extends EmployeesFilter,
    OrderedPaginationParams<EmployeesOrderType> {}

export interface EmployeesDictionary {
  farms: FarmDictModel[];
}
