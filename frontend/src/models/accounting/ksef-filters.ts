import type {
  KSeFInvoiceType,
  KSeFInvoiceStatus,
  KSeFPaymentStatus,
  InvoiceSource,
} from "./ksef-invoice";

export enum KSeFInvoicesOrderType {
  InvoiceDate = "InvoiceDate",
  InvoiceNumber = "InvoiceNumber",
  GrossAmount = "GrossAmount",
  SellerName = "SellerName",
  BuyerName = "BuyerName",
  KSeFNumber = "KSeFNumber",
  PaymentDueDate = "PaymentDueDate",
}

export interface KSeFInvoicesFilters {
  page: number;
  pageSize: number;
  orderBy?: KSeFInvoicesOrderType;
  isDescending?: boolean;
  invoiceType?: KSeFInvoiceType;
  // Filtry tekstowe
  buyerName?: string;
  sellerName?: string;
  invoiceNumber?: string;
  // Źródło faktury
  source?: InvoiceSource;
  // Data wystawienia
  invoiceDateFrom?: string;
  invoiceDateTo?: string;
  // Termin płatności
  paymentDueDateFrom?: string;
  paymentDueDateTo?: string;
  // Statusy
  status?: KSeFInvoiceStatus;
  paymentStatus?: KSeFPaymentStatus;
  // Ogólne wyszukiwanie (zachowane dla kompatybilności)
  searchQuery?: string;
}

export const initialKSeFFilters: KSeFInvoicesFilters = {
  page: 0,
  pageSize: 25,
  orderBy: KSeFInvoicesOrderType.InvoiceDate,
  isDescending: true,
  buyerName: "",
  sellerName: "",
  invoiceNumber: "",
  source: undefined,
  invoiceDateFrom: undefined,
  invoiceDateTo: undefined,
  paymentDueDateFrom: undefined,
  paymentDueDateTo: undefined,
  status: undefined,
  paymentStatus: undefined,
};

export type KSeFFiltersAction =
  | { type: "set"; key: keyof KSeFInvoicesFilters; value: any }
  | { type: "setPage"; payload: number }
  | { type: "setPageSize"; payload: number }
  | { type: "setOrderBy"; payload: KSeFInvoicesOrderType }
  | { type: "setIsDescending"; payload: boolean }
  | { type: "setInvoiceType"; payload: KSeFInvoiceType | undefined }
  | { type: "setBuyerName"; payload: string }
  | { type: "setSellerName"; payload: string }
  | { type: "setInvoiceNumber"; payload: string }
  | { type: "setSource"; payload: InvoiceSource | undefined }
  | { type: "setInvoiceDateFrom"; payload: string | undefined }
  | { type: "setInvoiceDateTo"; payload: string | undefined }
  | { type: "setPaymentDueDateFrom"; payload: string | undefined }
  | { type: "setPaymentDueDateTo"; payload: string | undefined }
  | { type: "setStatus"; payload: KSeFInvoiceStatus | undefined }
  | { type: "setPaymentStatus"; payload: KSeFPaymentStatus | undefined }
  | { type: "setSearchQuery"; payload: string | undefined }
  | { type: "setMultiple"; payload: Partial<KSeFInvoicesFilters> }
  | { type: "reset" };

export function ksefFiltersReducer(
  state: KSeFInvoicesFilters,
  action: KSeFFiltersAction
): KSeFInvoicesFilters {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value, page: 0 };
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
    case "setBuyerName":
      return { ...state, buyerName: action.payload, page: 0 };
    case "setSellerName":
      return { ...state, sellerName: action.payload, page: 0 };
    case "setInvoiceNumber":
      return { ...state, invoiceNumber: action.payload, page: 0 };
    case "setSource":
      return { ...state, source: action.payload, page: 0 };
    case "setInvoiceDateFrom":
      return { ...state, invoiceDateFrom: action.payload, page: 0 };
    case "setInvoiceDateTo":
      return { ...state, invoiceDateTo: action.payload, page: 0 };
    case "setPaymentDueDateFrom":
      return { ...state, paymentDueDateFrom: action.payload, page: 0 };
    case "setPaymentDueDateTo":
      return { ...state, paymentDueDateTo: action.payload, page: 0 };
    case "setStatus":
      return { ...state, status: action.payload, page: 0 };
    case "setPaymentStatus":
      return { ...state, paymentStatus: action.payload, page: 0 };
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

export const mapKSeFOrderTypeToField = (
  orderType: KSeFInvoicesOrderType
): string => {
  switch (orderType) {
    case KSeFInvoicesOrderType.InvoiceDate:
      return "invoiceDate";
    case KSeFInvoicesOrderType.InvoiceNumber:
      return "invoiceNumber";
    case KSeFInvoicesOrderType.GrossAmount:
      return "grossAmount";
    case KSeFInvoicesOrderType.SellerName:
      return "sellerName";
    case KSeFInvoicesOrderType.BuyerName:
      return "buyerName";
    case KSeFInvoicesOrderType.KSeFNumber:
      return "kSeFNumber";
    case KSeFInvoicesOrderType.PaymentDueDate:
      return "paymentDueDate";
    default:
      return "";
  }
};
