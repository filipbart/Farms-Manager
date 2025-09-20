import type { HouseRowModel } from "../farms/house-row-model";

export enum FallenStockType {
  FallCollision = "FallCollision",
  EndCycle = "EndCycle",
}

export interface FallenStockEntry {
  henhouseId: string;
  quantity: number;
}

export interface AddFallenStocksData {
  farmId: string;
  cycleId: string;
  type: FallenStockType;
  utilizationPlantId: string | null;
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
  utilizationPlantName?: string;
  date: string;
  typeDesc: string;
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

export interface UpdateFallenStocksData {
  cycleId: string;
  entries: FallenStockEditableEntry[];
}

export interface IrzSummaryData {
  currentStockSize: number;
  reportedFallenStock: number;
  collectedFallenStock: number;
}
