import type {
  SaleEntry,
  SaleEntryErrors,
} from "../../../../../models/sales/sales-entry";

export const validateEntry = (entry: SaleEntry): SaleEntryErrors => {
  const e: SaleEntryErrors = {};

  if (!entry.slaughterhouseId) e.slaughterhouseId = "Ubojnia jest wymagana";
  if (!entry.henhouseId) e.henhouseId = "Kurnik jest wymagany";

  const quantityNum = Number(entry.quantity);
  if (entry.quantity === "" || isNaN(quantityNum) || quantityNum <= 0)
    e.quantity = "Ilość musi być większa niż 0";

  const weightNum = Number(entry.weight);
  if (entry.weight === "" || isNaN(weightNum) || weightNum <= 0)
    e.weight = "Waga jest wymagana i musi być większa niż 0";

  const confiscatedCountNum = Number(entry.confiscatedCount);
  if (
    entry.confiscatedCount === "" ||
    isNaN(confiscatedCountNum) ||
    confiscatedCountNum < 0
  )
    e.confiscatedCount = "Wymagana liczba sztuk konfiskaty";

  const confiscatedWeightNum = Number(entry.confiscatedWeight);
  if (
    entry.confiscatedWeight === "" ||
    isNaN(confiscatedWeightNum) ||
    confiscatedWeightNum < 0
  )
    e.confiscatedWeight = "Wymagana waga konfiskaty";

  const deadCountNum = Number(entry.deadCount);
  if (entry.deadCount === "" || isNaN(deadCountNum) || deadCountNum < 0)
    e.deadCount = "Wymagana liczba martwych sztuk";

  const deadWeightNum = Number(entry.deadWeight);
  if (entry.deadWeight === "" || isNaN(deadWeightNum) || deadWeightNum < 0)
    e.deadWeight = "Wymagana waga martwych sztuk";

  const farmerWeightNum = Number(entry.farmerWeight);
  if (
    entry.farmerWeight === "" ||
    isNaN(farmerWeightNum) ||
    farmerWeightNum < 0
  )
    e.farmerWeight = "Wymagana waga hodowcy";

  return e;
};
