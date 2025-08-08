export interface FallenStockPickupRow {
  id: string;
  farmName: string;
  cycleText: string;
  date: string;
  quantity: number;
  dateCreatedUtc: string;
}

export interface GetFallenStockPickupsResponse {
  items: FallenStockPickupRow[];
}

export interface AddFallenStockPickupsEntries {
  date: string;
  quantity: number;
}

export interface AddFallenStockPickups {
  farmId: string;
  cycleId: string;
  entries: AddFallenStockPickupsEntries[];
}
