import type { Dayjs } from "dayjs";
import type { SaleType } from "../../../../models/sales/sales";
import type {
  SaleEntry,
  SaleEntryErrors,
} from "../../../../models/sales/sales-entry";

export interface OtherExtra {
  name: string;
  value: number | "";
}

export interface SaleFormState {
  saleType?: SaleType;
  farmId: string;
  identifierId: string;
  identifierDisplay: string;
  saleDate: Dayjs | null;
  entries: SaleEntry[];
  basePrice: number | "";
  priceWithExtras: number | "";
  comment: string;
  otherExtras: OtherExtra[];
}

export interface SaleFormErrors {
  saleType?: string;
  farmId?: string;
  identifierId?: string;
  saleDate?: string;
  basePrice?: string;
  priceWithExtras?: string;
  comment?: string;
  entries?: { [index: number]: SaleEntryErrors };
  entriesGeneral?: string;
  otherExtras?: OtherExtraErrors[];
}
export interface OtherExtraErrors {
  name?: string;
  value?: string;
}
