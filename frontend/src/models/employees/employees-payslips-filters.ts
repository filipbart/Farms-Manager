import type { GridAggregationModel } from "@mui/x-data-grid-premium";
import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";
import type { PayrollPeriod } from "./employees-payslips";

export const initialFilters: EmployeePayslipsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  searchPhrase: "",
  payrollPeriod: undefined,
  page: 0,
  pageSize: 10,
};

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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
