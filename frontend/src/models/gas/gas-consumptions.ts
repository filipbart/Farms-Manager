import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface GasConsumptionListModel extends AuditFields {
  id: string;
  farmId: string;
  farmName: string;
  cycleText: string;
  quantityConsumed: number;
  cost: number;
  dateCreatedUtc: string;
}

export interface AddGasConsumptionData {
  farmId: string;
  cycleId: string;
  quantityConsumed: number;
  cost: number;
}

export interface UpdateGasConsumptionData {
  quantityConsumed: number;
}

export interface GasConsumptionCalculateCostParams {
  farmId: string;
  quantity: number;
}

export interface GasConsumptionCalculateCostResponse {
  cost: number;
}
