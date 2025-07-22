export interface CorrectionData {
  invoiceNumber: string;
  farmId: string;
  subTotal: number;
  vatAmount: number;
  invoiceTotal: number;
  file?: File;
}

export interface FeedCorrectionListModel {
  id: string;
  farmName: string;
  invoiceNumber: string;
  dateCreatedUtc: string;
}
