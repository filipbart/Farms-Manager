export interface CorrectionData {
  invoiceNumber: string;
  farmId: string;
  cycleId: string;
  identifierDisplay: string;
  subTotal: number;
  vatAmount: number;
  invoiceTotal: number;
  invoiceDate: string;
  file?: File;
}
