import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface ProductionDataFailureListModel extends AuditFields {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
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
  cycleId: string;
  deadCount: number;
  defectiveCount: number;
}
