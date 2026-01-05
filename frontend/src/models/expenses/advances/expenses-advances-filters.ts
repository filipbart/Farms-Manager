import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: ExpensesAdvancesFilterPaginationModel = {
  dateSince: "",
  dateTo: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: ExpensesAdvancesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ExpensesAdvancesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ExpensesAdvancesFilterPaginationModel>;
      }
): ExpensesAdvancesFilterPaginationModel {
  let newState: ExpensesAdvancesFilterPaginationModel;
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

export const mapExpensesAdvancesOrderTypeToField = (
  orderType: ExpensesAdvancesOrderType
): string => {
  switch (orderType) {
    case ExpensesAdvancesOrderType.Date:
      return "date";
    case ExpensesAdvancesOrderType.Type:
      return "type";
    case ExpensesAdvancesOrderType.Name:
      return "name";
    case ExpensesAdvancesOrderType.Amount:
      return "amount";
    case ExpensesAdvancesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export enum ExpensesAdvancesOrderType {
  Date = "Date",
  Type = "Type",
  Name = "Name",
  Amount = "Amount",
  DateCreatedUtc = "DateCreatedUtc",
}

export interface ExpensesAdvancesFilter {
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface ExpensesAdvancesFilterPaginationModel
  extends ExpensesAdvancesFilter,
    OrderedPaginationParams<ExpensesAdvancesOrderType> {
  years?: number[];
  months?: number[];
}
