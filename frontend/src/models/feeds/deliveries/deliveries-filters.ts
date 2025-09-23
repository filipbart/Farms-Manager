import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsDeliveriesFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: FeedsDeliveriesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof FeedsDeliveriesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<FeedsDeliveriesFilterPaginationModel>;
      }
): FeedsDeliveriesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum FeedsDeliveriesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  HenhouseName = "HenhouseName",
  ItemName = "ItemName",
  VendorName = "VendorName",
  InvoiceNumber = "InvoiceNumber",
  Quantity = "Quantity",
  UnitPrice = "UnitPrice",
  InvoiceDate = "InvoiceDate",
  DueDate = "DueDate",
  InvoiceTotal = "InvoiceTotal",
  SubTotal = "SubTotal",
  VatAmount = "VatAmount",
  PaymentDateUtc = "PaymentDateUtc",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsDeliveriesFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  incorrectPrices?: boolean;
  dateSince: string;
  dateTo: string;
}

export interface FeedsDeliveriesFilterPaginationModel
  extends FeedsDeliveriesFilter,
    OrderedPaginationParams<FeedsDeliveriesOrderType> {}
