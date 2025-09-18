export interface SaleOtherExtras {
  name: string;
  value: number;
}

export interface SaleListModel {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  henhouseName: string;
  slaughterhouseName: string;
  type: SaleType;
  typeDesc: string;
  saleDate: string;
  weight: number;
  quantity: number;
  confiscatedWeight: number;
  confiscatedCount: number;
  deadWeight: number;
  deadCount: number;
  farmerWeight: number;
  basePrice: number;
  priceWithExtras: number;
  otherExtras: SaleOtherExtras[];
  comment: string;
  dateCreatedUtc: string;
  internalGroupId: string;
  dateIrzSentUtc: string | null;
  isSentToIrz: boolean;
  documentNumber: string;
  directoryPath: string;
}

export enum SaleType {
  PartSale = "PartSale",
  TotalSale = "TotalSale",
}

export const SaleTypeLabels: Record<SaleType, string> = {
  [SaleType.PartSale]: "Ubiórka",
  [SaleType.TotalSale]: "Całkowita",
};
