export interface ProductionDataRemainingFeedListModel {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  henhouseName: string;
  feedName: string;
  remainingTonnage: number;
  remainingValue: number;
  dateCreatedUtc: Date;
}

export interface AddRemainingFeedData {
  farmId: string;
  henhouseId: string;
  cycleId: string;
  feedName: string;
  remainingTonnage: number;
  remainingValue: number;
}

export interface UpdateRemainingFeedData {
  cycleId: string;
  remainingTonnage: number;
  remainingValue: number;
}
