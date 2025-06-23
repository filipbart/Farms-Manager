export interface InsertionEntry {
  henhouseId: string;
  hatcheryId: string;
  quantity: string;
  bodyWeight: string;
  isEditing?: boolean;
}

export interface InsertionEntryErrors {
  henhouseId?: string;
  hatcheryId?: string;
  quantity?: string;
  bodyWeight?: string;
}
