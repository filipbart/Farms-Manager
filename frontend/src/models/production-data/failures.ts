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
  henhouseId: string;
  cycleId: string;
  deadCount: number;
  defectiveCount: number;
}

export interface UpdateProductionDataFailureData {
  deadCount: number;
  defectiveCount: number;
}
