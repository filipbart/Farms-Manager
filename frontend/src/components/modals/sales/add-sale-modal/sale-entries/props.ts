import type { HouseRowModel } from "../../../../../models/farms/house-row-model";
import type { SaleFormErrors } from "../../../../../models/sales/sale-form-states";
import type {
  SaleEntry,
  SaleEntryErrors,
} from "../../../../../models/sales/sales-entry";
import type { SaleFieldsExtraRow } from "../../../../../services/sales-settings-service";

export interface SaleEntriesSectionProps {
  saleFieldsExtra: SaleFieldsExtraRow[];
  setErrors: React.Dispatch<React.SetStateAction<SaleFormErrors>>;
  dispatch: React.Dispatch<any>;
  entries: SaleEntry[];
  henhouses: HouseRowModel[];
  errors: { [index: number]: SaleEntryErrors } | undefined;
  setEntryErrors: (index: number, errors: SaleEntryErrors) => void;
  farmId?: string;
  entriesTableReady: number[];
}
