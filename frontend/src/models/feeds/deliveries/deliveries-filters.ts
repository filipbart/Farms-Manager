import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsDeliveriesFilterPaginationModel = {
  farmIds: [],
  henhouseIds: [],
  feedNames: [],
  invoiceNumber: "",
  cycles: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
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
  let newState: FeedsDeliveriesFilterPaginationModel;

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
  henhouseIds: string[];
  feedNames: string[];
  invoiceNumber: string;
  cycles: CycleDictModel[];
  incorrectPrices?: boolean;
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface FeedsDeliveriesFilterPaginationModel
  extends FeedsDeliveriesFilter,
    OrderedPaginationParams<FeedsDeliveriesOrderType> {}
