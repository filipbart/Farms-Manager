import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: ExpensesAdvancesFilterPaginationModel = {
  dateSince: "",
  dateTo: "",
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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
}

export interface ExpensesAdvancesFilterPaginationModel
  extends ExpensesAdvancesFilter,
    OrderedPaginationParams<ExpensesAdvancesOrderType> {}
