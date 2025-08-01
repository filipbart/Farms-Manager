import type { Dayjs } from "dayjs";
import type { SaleType } from "./sales";
import type { SaleEntry, SaleEntryErrors } from "./sales-entry";

export interface OtherExtra {
  name: string;
  value: number | "";
}

export interface SaleFormState {
  saleType?: SaleType;
  farmId: string;
  slaughterhouseId: string;
  identifierId: string;
  identifierDisplay: string;
  saleDate: Dayjs | null;
  entries: SaleEntry[];
  entriesTableReady: number[];
  files: File[];
}

export interface SaleFormErrors {
  saleType?: string;
  farmId?: string;
  slaughterhouseId?: string;
  identifierId?: string;
  saleDate?: string;
  entries?: { [index: number]: SaleEntryErrors };
  entriesGeneral?: string;
}
export interface OtherExtraErrors {
  name?: string;
  value?: string;
}
