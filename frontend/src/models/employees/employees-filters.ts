import type { FarmDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";
import type { EmployeeStatus } from "./employees";

const LOCAL_STORAGE_KEY = "employeesFilters";

const saveFiltersToLocalStorage = (filters: EmployeesFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<EmployeesFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): EmployeesFilterPaginationModel => {
  const defaultFilters: EmployeesFilterPaginationModel = {
    farmIds: [],
    dateSince: "",
    dateTo: "",
    showDeleted: false,
    page: 0,
    pageSize: 10,
  };

  const savedFilters = loadFiltersFromLocalStorage();

  if (savedFilters) {
    return {
      ...defaultFilters,
      ...savedFilters,

      page: 0,
    };
  }

  return defaultFilters;
};

export const initialFilters = getInitialFilters();

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
  let newState: EmployeesFilterPaginationModel;

  switch (action.type) {
    case "set":
      newState = { ...state, [action.key]: action.value };
      break;
    case "setMultiple":
      newState = { ...state, ...action.payload };
      break;
    default:
      return state;
  }

  saveFiltersToLocalStorage(newState);
  return newState;
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
  showDeleted?: boolean;
}

export interface EmployeesFilterPaginationModel
  extends EmployeesFilter,
    OrderedPaginationParams<EmployeesOrderType> {}

export interface EmployeesDictionary {
  farms: FarmDictModel[];
}
