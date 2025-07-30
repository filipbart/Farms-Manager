export interface ProductionDataWeighingListModel {
  id: string;
  dateCreatedUtc: string;

  cycleText: string;
  farmName: string;
  henhouseName: string;
  hatcheryName: string;
  weighing1Day?: number;
  weighing1Weight?: number;
  weighing1Deviation?: number;
  weighing2Day?: number;
  weighing2Weight?: number;
  weighing2Deviation?: number;
  weighing3Day?: number;
  weighing3Weight?: number;
  weighing3Deviation?: number;
  weighing4Day?: number;
  weighing4Weight?: number;
  weighing4Deviation?: number;
  weighing5Day?: number;
  weighing5Weight?: number;
  weighing5Deviation?: number;
}

export interface AddWeighingData {
  farmId: string;
  henhouseId: string;
  cycleId: string;
  hatcheryId: string;
  day: number;
  weight: number;
}

export interface UpdateWeighingData {
  weighingNumber: number;
  day: number;
  weight: number;
}

export interface WeightStandardRowModel {
  id: string;
  day: number;
  weight: number;
}
