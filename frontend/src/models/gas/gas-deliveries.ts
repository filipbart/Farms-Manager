export interface GasDeliveryListModel {
  id: string;
  farmName: string;
  contractorName: string;
  invoiceNumber: string;
  invoiceDate: string;
  invoiceTotal: number;
  unitPrice: number;
  quantity: number;
  comment: string;
  filePath?: string;
  dateCreatedUtc: string;
}

export interface AddGasDeliveryData {
  farmId: string;
  contractorId: string;
  invoiceNumber: string;
  invoiceDate: string;
  unitPrice: number;
  quantity: number;
  comment: string;
  file?: File;
}

export interface UpdateGasDeliveryData {
  invoiceNumber: string;
  invoiceDate: string;
  unitPrice: number;
  quantity: number;
  comment: string;
}

export interface GasInvoiceData {
  farmId?: string;
  invoiceNumber?: string;
  contractorId?: string;
  contractorName?: string;
  invoiceDate?: string;
  unitPrice?: number;
  quantity?: number;
  comment?: string;
}

export interface SaveGasInvoiceData {
  filePath: string;
  draftId: string;
  data: GasInvoiceData;
}

export interface DraftGasInvoice {
  draftId: string;
  filePath: string;
  fileUrl: string;
  extractedFields: GasInvoiceData;
}
