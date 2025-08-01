import type { FilterConfig } from "../../components/filters/filter-types";
import type {
  HatcheriesPricesDictionary,
  HatcheriesPricesFilterPaginationModel,
} from "../../models/hatcheries/hatcheries-prices-filters";

export const getHatcheriesPricesFiltersConfig = (
  dictionary: HatcheriesPricesDictionary | undefined
): FilterConfig<keyof HatcheriesPricesFilterPaginationModel>[] => [
  {
    key: "hatcheryIds",
    label: "Wylęgarnia",
    type: "multiSelect",
    options:
      dictionary?.hatcheries.map((hatchery) => ({
        value: hatchery.id,
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
