import type { HouseRowModel } from "../farms/house-row-model";

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
  sendToIrz: boolean;
}

export interface GetAvailableHenhousesFallenStocks {
  farmId: string;
  cycleId: string;
  date: string;
}

export interface GetAvailableHenhousesFallenStocksResponse {
  henhouses: HouseRowModel[];
}

export interface GetFallenStockEditData {
  farmName: string;
  cycleDisplay: string;
  utilizationPlantName: string;
  date: string;
  entries: {
    henhouseId: string;
    henhouseName: string;
    quantity: number;
  }[];
}

export type FallenStockEditableEntry = {
  henhouseId: string;
  henhouseName: string;
  quantity: string;
};
