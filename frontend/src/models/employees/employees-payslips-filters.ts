import type { GridAggregationModel } from "@mui/x-data-grid-premium";
import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";
import type { PayrollPeriod } from "./employees-payslips";

const getInitialFilters = (): EmployeePayslipsFilterPaginationModel => {
  const currentYear = new Date().getFullYear();
  return {
    farmIds: [],
    cycles: [],
    searchPhrase: "",
    payrollPeriod: undefined,
    year: currentYear,
    showDeleted: false,
    page: 0,
    pageSize: 10,
  };
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
  year?: number | string;
  showDeleted?: boolean;
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
