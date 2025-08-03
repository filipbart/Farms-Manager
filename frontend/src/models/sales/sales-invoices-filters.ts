import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: SalesInvoicesFilterPaginationModel = {
  farmIds: [],
  slaughterhouseIds: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: SalesInvoicesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof SalesInvoicesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<SalesInvoicesFilterPaginationModel>;
      }
): SalesInvoicesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum SalesInvoicesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Slaughterhouse = "Slaughterhouse",
  InvoiceNumber = "InvoiceNumber",
  InvoiceDate = "InvoiceDate",
  DueDate = "DueDate",
  InvoiceTotal = "InvoiceTotal",
  SubTotal = "SubTotal",
  VatAmount = "VatAmount",
}

export const mapSalesInvoiceOrderTypeToField = (
  orderType: SalesInvoicesOrderType
): string => {
  switch (orderType) {
    case SalesInvoicesOrderType.Cycle:
      return "cycleText";
    case SalesInvoicesOrderType.Farm:
      return "farmName";
    case SalesInvoicesOrderType.Slaughterhouse:
      return "slaughterhouseName";
    case SalesInvoicesOrderType.InvoiceNumber:
      return "invoiceNumber";
    case SalesInvoicesOrderType.InvoiceDate:
      return "invoiceDate";
    case SalesInvoicesOrderType.DueDate:
      return "dueDate";
    case SalesInvoicesOrderType.InvoiceTotal:
      return "invoiceTotal";
    case SalesInvoicesOrderType.SubTotal:
      return "subTotal";
    case SalesInvoicesOrderType.VatAmount:
      return "vatAmount";
    default:
      return "";
  }
};

export interface SalesInvoicesFilter {
  farmIds: string[];
  slaughterhouseIds: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
}

export interface SalesInvoicesFilterPaginationModel
  extends SalesInvoicesFilter,
    OrderedPaginationParams<SalesInvoicesOrderType> {}
