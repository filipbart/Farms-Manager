export interface SaleEntry {
  henhouseId: string;
  slaughterhouseId: string;
  quantity: string;
  weight: string;
  confiscatedCount: string;
  confiscatedWeight: string;
  deadCount: string;
  deadWeight: string;
  farmerWeight: string;
  isEditing: boolean;
}

export interface SaleEntryErrors {
  henhouseId?: string;
  slaughterhouseId?: string;
  quantity?: string;
  weight?: string;
  confiscatedCount?: string;
  confiscatedWeight?: string;
  deadCount?: string;
  deadWeight?: string;
  farmerWeight?: string;
}
