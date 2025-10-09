import type { GridAggregationModel } from "@mui/x-data-grid-premium";
import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";
import type { PayrollPeriod } from "./employees-payslips";

const LOCAL_STORAGE_KEY = "employeesPayslipsFilters";

const saveFiltersToLocalStorage = (
  filters: EmployeePayslipsFilterPaginationModel
) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<EmployeePayslipsFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): EmployeePayslipsFilterPaginationModel => {
  const defaultFilters: EmployeePayslipsFilterPaginationModel = {
    farmIds: [],
    cycles: [],
    searchPhrase: "",
    payrollPeriod: undefined,
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
  state: EmployeePayslipsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof EmployeePayslipsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<EmployeePayslipsFilterPaginationModel>;
      }
): EmployeePayslipsFilterPaginationModel {
  let newState: EmployeePayslipsFilterPaginationModel;

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

export enum EmployeePayslipsOrderType {
  FarmName = "FarmName",
  Cycle = "Cycle",
  EmployeeFullName = "EmployeeFullName",
  PayrollPeriod = "PayrollPeriod",
  NetPay = "NetPay",
  BaseSalary = "BaseSalary",
  BonusAmount = "BonusAmount",
}

export const mapEmployeePayslipOrderTypeToField = (
  orderType: EmployeePayslipsOrderType
): string => {
  switch (orderType) {
    case EmployeePayslipsOrderType.FarmName:
      return "farmName";
    case EmployeePayslipsOrderType.Cycle:
      return "cycleText";
    case EmployeePayslipsOrderType.EmployeeFullName:
      return "employeeFullName";
    case EmployeePayslipsOrderType.PayrollPeriod:
      return "payrollPeriodDesc";
    case EmployeePayslipsOrderType.NetPay:
      return "netPay";
    case EmployeePayslipsOrderType.BaseSalary:
      return "baseSalary";
    case EmployeePayslipsOrderType.BonusAmount:
      return "bonusAmount";
    default:
      return "";
  }
};

export interface EmployeePayslipsFilter {
  farmIds: string[];
  cycles: string[];
  searchPhrase?: string;
  payrollPeriod?: PayrollPeriod;
}

export interface EmployeePayslipsFilterPaginationModel
  extends EmployeePayslipsFilter,
    OrderedPaginationParams<EmployeePayslipsOrderType> {
  aggregationModel?: GridAggregationModel;
}

export interface EmployeePayslipsDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}
