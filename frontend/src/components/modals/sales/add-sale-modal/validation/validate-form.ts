import type { SaleEntryErrors } from "../../../../../models/sales/sales-entry";
import type {
  OtherExtraErrors,
  SaleFormErrors,
  SaleFormState,
} from "../sale-form-types";
import { validateEntry } from "./validate-entry";

export const validateForm = (form: SaleFormState): SaleFormErrors => {
  const newErrors: SaleFormErrors = {};

  if (!form.saleType) newErrors.saleType = "Typ sprzedaży jest wymagany";
  if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
  if (!form.identifierId) newErrors.identifierId = "Brak aktywnego cyklu";
  if (!form.saleDate) newErrors.saleDate = "Data jest wymagana";

  if (
    form.basePrice === "" ||
    isNaN(Number(form.basePrice)) ||
    Number(form.basePrice) <= 0
  ) {
    newErrors.basePrice = "Cena bazowa musi być większa niż 0";
  }

  if (
    form.priceWithExtras === "" ||
    isNaN(Number(form.priceWithExtras)) ||
    Number(form.priceWithExtras) <= 0
  ) {
    newErrors.priceWithExtras = "Cena z dodatkami musi być większa niż 0";
  }

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

  if (form.otherExtras.length > 0) {
    const otherExtrasErrors: OtherExtraErrors[] = [];

    form.otherExtras.forEach((extra, index) => {
      const extraErrors: OtherExtraErrors = {};
      if (!extra.name || extra.name.trim() === "") {
        extraErrors.name = "Nazwa dodatku jest wymagana";
      }
      if (
        extra.value === "" ||
        isNaN(Number(extra.value)) ||
        Number(extra.value) < 0
      ) {
        extraErrors.value = "Wartość dodatku musi być większa lub równa 0";
      }
      if (Object.keys(extraErrors).length > 0) {
        otherExtrasErrors[index] = extraErrors;
      }
    });

    if (otherExtrasErrors.length > 0) {
      newErrors.otherExtras = otherExtrasErrors;
    }
  }

  return newErrors;
};
