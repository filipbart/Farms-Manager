// Typy faktur KSeF
export enum KSeFInvoiceType {
  Sales = "Sales",
  Purchase = "Purchase",
}

// Status faktury KSeF
export enum KSeFInvoiceStatus {
  New = "New",
  Rejected = "Rejected",
  Accepted = "Accepted",
  SentToOffice = "SentToOffice",
}

// Status płatności
export enum KSeFPaymentStatus {
  Unpaid = "Unpaid",
  PartiallyPaid = "PartiallyPaid",
  Suspended = "Suspended",
  PaidCash = "PaidCash",
  PaidTransfer = "PaidTransfer",
}

// Typ płatności
export enum KSeFInvoicePaymentType {
  Cash = "Cash",
  BankTransfer = "BankTransfer",
}

// Źródło faktury
export enum InvoiceSource {
  KSeF = "KSeF",
  Manual = "Manual",
}

// Typ modułu
export enum ModuleType {
  None = "None",
  Feeds = "Feeds",
  ProductionExpenses = "ProductionExpenses",
  Gas = "Gas",
  Sales = "Sales",
  Farmstead = "Farmstead",
  Other = "Other",
}

// Typ odliczenia VAT
export enum VatDeductionType {
  Full = "Full",
  Half = "Half",
  None = "None",
}

// Model faktury KSeF w liście
export interface KSeFInvoiceListModel {
  id: string;
  kSeFNumber: string;
  nip: string;
  buyerName: string;
  buyerNip: string;
  sellerName: string;
  sellerNip: string;
  invoiceType: KSeFInvoiceType;
  cycleIdentifier: string | null;
  cycleYear: number | null;
  source: InvoiceSource;
  location: string | null;
  invoiceNumber: string;
  invoiceDate: string;
  moduleType: ModuleType | null;
  vatDeductionType: VatDeductionType | null;
  status: KSeFInvoiceStatus;
  paymentStatus: KSeFPaymentStatus;
  paymentType: KSeFInvoicePaymentType;
  comment: string | null;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  hasXml: boolean;
  hasPdf: boolean;
}

// Model szczegółów faktury
export interface KSeFInvoiceDetails extends KSeFInvoiceListModel {
  invoiceXml: string | null;
  comment: string | null;
  vatDeductionType: VatDeductionType;
  farmId: string | null;
  cycleId: string | null;
  assignedUserId: string | null;
  assignedUserName: string | null;
  relatedInvoiceNumber: string | null;
  createdAt: string;
  createdBy: string | null;
}

// Mapowanie statusów na polskie nazwy
export const KSeFInvoiceStatusLabels: Record<KSeFInvoiceStatus, string> = {
  [KSeFInvoiceStatus.New]: "Nowa",
  [KSeFInvoiceStatus.Rejected]: "Odrzucona",
  [KSeFInvoiceStatus.Accepted]: "Zaakceptowana",
  [KSeFInvoiceStatus.SentToOffice]: "Przekazana do biura",
};

export const KSeFPaymentStatusLabels: Record<KSeFPaymentStatus, string> = {
  [KSeFPaymentStatus.Unpaid]: "Nieopłacona",
  [KSeFPaymentStatus.PartiallyPaid]: "Częściowo opłacona",
  [KSeFPaymentStatus.Suspended]: "Wstrzymana",
  [KSeFPaymentStatus.PaidCash]: "Opłacona gotówką",
  [KSeFPaymentStatus.PaidTransfer]: "Opłacona przelewem",
};

export const KSeFInvoicePaymentTypeLabels: Record<
  KSeFInvoicePaymentType,
  string
> = {
  [KSeFInvoicePaymentType.Cash]: "Gotówka",
  [KSeFInvoicePaymentType.BankTransfer]: "Przelew",
};

export const InvoiceSourceLabels: Record<InvoiceSource, string> = {
  [InvoiceSource.KSeF]: "KSeF",
  [InvoiceSource.Manual]: "Poza KSeF",
};

export const ModuleTypeLabels: Record<ModuleType, string> = {
  [ModuleType.None]: "Brak",
  [ModuleType.Feeds]: "Pasze",
  [ModuleType.ProductionExpenses]: "Koszty produkcyjne",
  [ModuleType.Gas]: "Gaz",
  [ModuleType.Sales]: "Sprzedaże",
  [ModuleType.Farmstead]: "Gospodarstwo rolne",
  [ModuleType.Other]: "Inne",
};

export const KSeFInvoiceTypeLabels: Record<KSeFInvoiceType, string> = {
  [KSeFInvoiceType.Sales]: "Sprzedaż",
  [KSeFInvoiceType.Purchase]: "Zakup",
};

export const VatDeductionTypeLabels: Record<VatDeductionType, string> = {
  [VatDeductionType.Full]: "100% (1/1)",
  [VatDeductionType.Half]: "50% (1/2)",
  [VatDeductionType.None]: "Brak",
};

// Parsed XML data interfaces
export interface KSeFParsedXmlData {
  header?: {
    formCode?: string;
    formVariant?: number;
    createdDate?: string;
    systemInfo?: string;
  };
  seller?: KSeFPartyData;
  buyer?: KSeFPartyData;
  thirdParty?: KSeFPartyData & { role?: string };
  invoiceData?: {
    invoiceType?: string;
    issueDate?: string;
    issuePlace?: string;
    invoiceNumber?: string;
    saleDate?: string;
    currency?: string;
    grossTotal?: number;
    vatBreakdown?: KSeFVatBreakdown[];
  };
  lineItems?: KSeFLineItem[];
  payment?: KSeFPaymentData;
  footer?: string;
}

export interface KSeFPartyData {
  nip?: string;
  name?: string;
  vatEuNumber?: string;
  idNumber?: string;
  address?: {
    countryCode?: string;
    addressLine1?: string;
    addressLine2?: string;
    gln?: string;
  };
  contact?: {
    email?: string;
    phone?: string;
  };
}

export interface KSeFVatBreakdown {
  rate: string;
  netAmount: number;
  vatAmount?: number;
}

export interface KSeFLineItem {
  lineNumber: number;
  name?: string;
  unit?: string;
  quantity?: number;
  unitPriceNet?: number;
  unitPriceGross?: number;
  netAmount?: number;
  grossAmount?: number;
  vatRate?: number;
  pkwiu?: string;
  cn?: string;
  gtu?: string;
}

export interface KSeFPaymentData {
  isPaid?: boolean;
  paymentDate?: string;
  dueDate?: string;
  paymentMethod?: string;
  paymentMethodOther?: string;
  paymentDescription?: string;
  bankAccounts?: KSeFBankAccount[];
}

export interface KSeFBankAccount {
  accountNumber?: string;
  bankName?: string;
  description?: string;
}
