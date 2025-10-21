import type { FilterConfig } from "../../components/filters/filter-types";
import type { HatcheriesNames } from "../../models/hatcheries/hatcheries-prices";
import type { HatcheriesPricesFilterPaginationModel } from "../../models/hatcheries/hatcheries-prices-filters";

export const getHatcheriesPricesFiltersConfig = (
  dictionary: HatcheriesNames | undefined,
  isAdmin: boolean = false
): FilterConfig<keyof HatcheriesPricesFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof HatcheriesPricesFilterPaginationModel>[] = [
    {
      key: "hatcheryNames",
      label: "Wylęgarnia",
      type: "multiSelect",
      options:
        dictionary?.hatcheries.map((hatchery) => ({
          value: hatchery.name,
          label: hatchery.name,
        })) || [],
      disabled: !dictionary,
    },
    {
      key: "dateSince",
      label: "Data od",
      type: "date",
    },
    {
      key: "dateTo",
      label: "Data do",
      type: "date",
    },
    {
      key: "priceFrom",
      label: "Cena od [zł]",
      type: "number",
    },
    {
      key: "priceTo",
      label: "Cena do [zł]",
      type: "number",
    },
  ];

  if (isAdmin) {
    baseFilters.push({
      key: "showDeleted",
      label: "Pokaż usunięte",
      type: "checkbox",
    });
  }

  return baseFilters;
};
