import type { KSeFInvoiceType } from "./ksef-invoice";

export enum KSeFInvoicesOrderType {
  InvoiceDate = "InvoiceDate",
  InvoiceNumber = "InvoiceNumber",
  GrossAmount = "GrossAmount",
  SellerName = "SellerName",
  BuyerName = "BuyerName",
  KSeFNumber = "KSeFNumber",
}

export interface KSeFInvoicesFilters {
  page: number;
  pageSize: number;
  orderBy?: KSeFInvoicesOrderType;
  isDescending?: boolean;
  invoiceType?: KSeFInvoiceType;
  dateFrom?: string;
  dateTo?: string;
  searchQuery?: string;
}

export const initialKSeFFilters: KSeFInvoicesFilters = {
  page: 0,
  pageSize: 25,
  orderBy: KSeFInvoicesOrderType.InvoiceDate,
  isDescending: true,
};

export type KSeFFiltersAction =
  | { type: "setPage"; payload: number }
  | { type: "setPageSize"; payload: number }
  | { type: "setOrderBy"; payload: KSeFInvoicesOrderType }
  | { type: "setIsDescending"; payload: boolean }
  | { type: "setInvoiceType"; payload: KSeFInvoiceType | undefined }
  | { type: "setDateFrom"; payload: string | undefined }
  | { type: "setDateTo"; payload: string | undefined }
  | { type: "setSearchQuery"; payload: string | undefined }
  | { type: "setMultiple"; payload: Partial<KSeFInvoicesFilters> }
  | { type: "reset" };

export function ksefFiltersReducer(
  state: KSeFInvoicesFilters,
  action: KSeFFiltersAction
): KSeFInvoicesFilters {
  switch (action.type) {
    case "setPage":
      return { ...state, page: action.payload };
    case "setPageSize":
      return { ...state, pageSize: action.payload };
    case "setOrderBy":
      return { ...state, orderBy: action.payload };
    case "setIsDescending":
      return { ...state, isDescending: action.payload };
    case "setInvoiceType":
      return { ...state, invoiceType: action.payload, page: 0 };
    case "setDateFrom":
      return { ...state, dateFrom: action.payload, page: 0 };
    case "setDateTo":
      return { ...state, dateTo: action.payload, page: 0 };
    case "setSearchQuery":
      return { ...state, searchQuery: action.payload, page: 0 };
    case "setMultiple":
      return { ...state, ...action.payload };
    case "reset":
      return initialKSeFFilters;
    default:
      return state;
  }
}

export const mapKSeFOrderTypeToField: Record<KSeFInvoicesOrderType, string> = {
  [KSeFInvoicesOrderType.InvoiceDate]: "invoiceDate",
  [KSeFInvoicesOrderType.InvoiceNumber]: "invoiceNumber",
  [KSeFInvoicesOrderType.GrossAmount]: "grossAmount",
  [KSeFInvoicesOrderType.SellerName]: "sellerName",
  [KSeFInvoicesOrderType.BuyerName]: "buyerName",
  [KSeFInvoicesOrderType.KSeFNumber]: "kSeFNumber",
};
