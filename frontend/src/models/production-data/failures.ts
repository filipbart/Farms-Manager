export interface ProductionDataFailureListModel {
  id: string;
  cycleText: string;
  farmName: string;
  henhouseName: string;
  deadCount: number;
  defectiveCount: number;
  dateCreatedUtc: Date;
}

export interface AddProductionDataFailureData {
  farmId: string;
  cycleId: string;
  failureEntries: ProductionDataFailureEntry[];
}

export interface ProductionDataFailureEntry {
  henhouseId: string;
  deadCount: number;
  defectiveCount: number;
}

export interface UpdateProductionDataFailureData {
  deadCount: number;
  defectiveCount: number;
}
