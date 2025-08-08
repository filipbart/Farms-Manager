export interface FallenStockEntry {
  henhouseId: string;
  quantity: number;
}

export interface AddFallenStocksData {
  farmId: string;
  cycleId: string;
  utilizationPlantId: string;
  date: string;
  entries: FallenStockEntry[];
}
