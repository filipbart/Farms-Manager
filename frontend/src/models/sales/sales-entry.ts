import type { OtherExtra, OtherExtraErrors } from "./sale-form-states";

export interface SaleEntry {
  henhouseId: string;
  henhouseName: string;
  quantity: string;
  weight: string;
  confiscatedCount: string;
  confiscatedWeight: string;
  deadCount: string;
  deadWeight: string;
  farmerWeight: string;
  basePrice: number | "";
  priceWithExtras: number | "";
  comment: string;
  otherExtras: OtherExtra[];
}

export interface SaleEntryErrors {
  henhouseId?: string;
  quantity?: string;
  weight?: string;
  confiscatedCount?: string;
  confiscatedWeight?: string;
  deadCount?: string;
  deadWeight?: string;
  farmerWeight?: string;
  basePrice?: string;
  priceWithExtras?: string;
  comment?: string;
  otherExtras?: OtherExtraErrors[];
}
