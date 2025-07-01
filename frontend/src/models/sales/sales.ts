export default interface SalesListModel {
  id: string;
  cycleText: string;
  farmName: string;
  henhouseName: string;
  type: SaleType;
  typeDesc: string;
  saleDate: Date;
  //todo uzupełnić

  dateCreatedUtc: Date;
  internalGroupId: string;
  dateIrzSentUtc?: Date;
  isSentToIrz: boolean;
  documentNumber?: string;
}

export enum SaleType {
  PartSale = "PartSale",
  TotalSale = "TotalSale",
}

export const SaleTypeLabels: Record<SaleType, string> = {
  [SaleType.PartSale]: "Ubiórka",
  [SaleType.TotalSale]: "Całkowita",
};
