import type { HouseRowModel } from "../../../../../models/farms/house-row-model";
import type {
  SaleEntry,
  SaleEntryErrors,
} from "../../../../../models/sales/sales-entry";

export interface SaleEntriesTableProps {
  entries: SaleEntry[];
  henhouses: HouseRowModel[];
  errors: { [index: number]: SaleEntryErrors } | undefined;
  dispatch: React.Dispatch<any>;
  setEntryErrors: (index: number, errors: SaleEntryErrors) => void;
  farmId?: string;
}
