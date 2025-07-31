export interface GasConsumptionListModel {
  id: string;
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
