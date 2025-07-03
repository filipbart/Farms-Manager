import type {
  SaleEntry,
  SaleEntryErrors,
} from "../../../../../models/sales/sales-entry";
import type { OtherExtraErrors } from "../sale-form-types";

export const validateEntry = (entry: SaleEntry): SaleEntryErrors => {
  const e: SaleEntryErrors = {};

  if (!entry.henhouseId) e.henhouseId = "Kurnik jest wymagany";

  const quantityNum = Number(entry.quantity);
  if (entry.quantity === "" || isNaN(quantityNum) || quantityNum <= 0) {
    e.quantity = "Ilość musi być większa niż 0";
  }

  const weightNum = Number(entry.weight);
  if (entry.weight === "" || isNaN(weightNum) || weightNum <= 0) {
    e.weight = "Waga jest wymagana i musi być większa niż 0";
  }

  const confiscatedCountNum = Number(entry.confiscatedCount);
  if (
    entry.confiscatedCount === "" ||
    isNaN(confiscatedCountNum) ||
    confiscatedCountNum < 0
  ) {
    e.confiscatedCount = "Wymagana liczba sztuk konfiskaty";
  }

  const confiscatedWeightNum = Number(entry.confiscatedWeight);
  if (
    entry.confiscatedWeight === "" ||
    isNaN(confiscatedWeightNum) ||
    confiscatedWeightNum < 0
  ) {
    e.confiscatedWeight = "Wymagana waga konfiskaty";
  }

  const deadCountNum = Number(entry.deadCount);
  if (entry.deadCount === "" || isNaN(deadCountNum) || deadCountNum < 0) {
    e.deadCount = "Wymagana liczba martwych sztuk";
  }

  const deadWeightNum = Number(entry.deadWeight);
  if (entry.deadWeight === "" || isNaN(deadWeightNum) || deadWeightNum < 0) {
    e.deadWeight = "Wymagana waga martwych sztuk";
  }

  const farmerWeightNum = Number(entry.farmerWeight);
  if (
    entry.farmerWeight === "" ||
    isNaN(farmerWeightNum) ||
    farmerWeightNum < 0
  ) {
    e.farmerWeight = "Wymagana waga hodowcy";
  }

  const basePriceNum = Number(entry.basePrice);
  if (entry.basePrice === "" || isNaN(basePriceNum) || basePriceNum <= 0) {
    e.basePrice = "Cena bazowa musi być większa niż 0";
  }

  const priceWithExtrasNum = Number(entry.priceWithExtras);
  if (
    entry.priceWithExtras === "" ||
    isNaN(priceWithExtrasNum) ||
    priceWithExtrasNum <= 0
  ) {
    e.priceWithExtras = "Cena z dodatkami musi być większa niż 0";
  }

  // Walidacja otherExtras w ramach entry
  if (entry.otherExtras?.length > 0) {
    const otherExtrasErrors: OtherExtraErrors[] = [];

    entry.otherExtras.forEach((extra, index) => {
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
      e.otherExtras = otherExtrasErrors;
    }
  }

  return e;
};
