export interface ProductionDataTransferFeedListModel {
  id: string;
  fromCycleText: string;
  fromFarmName: string;
  fromHenhouseName: string;
  toCycleText: string;
  toFarmName: string;
  toHenhouseName: string;
  feedName: string;
  tonnage: number;
  value: number;
  dateCreatedUtc: Date;
}

export interface AddTransferFeedData {
  fromFarmId: string;
  fromHenhouseId: string;
  fromCycleId: string;
  toFarmId: string;
  toHenhouseId: string;
  toCycleId: string;
  feedName: string;
  tonnage: number;
  value: number;
}

export interface UpdateTransferFeedData {
  tonnage: number;
  value: number;
}
