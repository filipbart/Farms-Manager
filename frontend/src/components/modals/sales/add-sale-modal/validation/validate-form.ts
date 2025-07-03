import type { SaleEntryErrors } from "../../../../../models/sales/sales-entry";
import type { SaleFormErrors, SaleFormState } from "../sale-form-types";
import { validateEntry } from "./validate-entry";

export const validateForm = (form: SaleFormState): SaleFormErrors => {
  const newErrors: SaleFormErrors = {};

  if (!form.saleType) newErrors.saleType = "Typ sprzedaży jest wymagany";
  if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
  if (!form.slaughterhouseId)
    newErrors.slaughterhouseId = "Ubojnia jest wymagana";
  if (!form.identifierId) newErrors.identifierId = "Brak aktywnego cyklu";
  if (!form.saleDate) newErrors.saleDate = "Data jest wymagana";

  if (form.entries.length === 0) {
    newErrors.entriesGeneral = "Musisz dodać przynajmniej jedną pozycję";
  } else {
    const entryErrors: { [index: number]: SaleEntryErrors } = {};
    form.entries.forEach((entry, index) => {
      const e = validateEntry(entry);
      if (Object.keys(e).length > 0) {
        entryErrors[index] = e;
      }
    });

    if (Object.keys(entryErrors).length > 0) {
      newErrors.entries = entryErrors;
    }
  }

  return newErrors;
};
