export interface ProductionDataTransferFeedListModel {
  id: string;
  fromCycleText: string;
  fromFarmName: string;
  fromHenhouseName: string;
  toCycleText: string;
  toFarmName: string;
  toHenhouseName: string;
  feedName: string;
  remainingTonnage: number;
  remainingValue: number;
  dateCreatedUtc: Date;
}
