import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface ProductionDataFlockLossListModel extends AuditFields {
  id: string;
  dateCreatedUtc: string;

  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  henhouseName: string;
  hatcheryName: string;
  insertionQuantity: number;

  flockLoss1Day?: number;
  flockLoss1Quantity?: number;
  flockLoss1Percentage?: number;

  flockLoss2Day?: number;
  flockLoss2Quantity?: number;
  flockLoss2Percentage?: number;

  flockLoss3Day?: number;
  flockLoss3Quantity?: number;
  flockLoss3Percentage?: number;

  flockLoss4Day?: number;
  flockLoss4Quantity?: number;
  flockLoss4Percentage?: number;
}

export interface UpdateFlockLossData {
  measureNumber: number;
  day: number;
  quantity: number;
}

export interface AddFlockLossMeasureData {
  farmId: string;
  cycleId: string;
  measureNumber: number;
  day: number;
  entries: FlockLossMeasureEntry[];
}

export interface FlockLossMeasureEntry {
  henhouseId: string;
  quantity: number;
}
