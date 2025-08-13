import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDeliveriesFilterPaginationModel } from "../../../models/feeds/deliveries/deliveries-filters";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";

export const getFeedsDeliveriesFiltersConfig = (
  dictionary: FeedsDictionary | undefined,
  uniqueCycles: CycleDictModel[]
): FilterConfig<keyof FeedsDeliveriesFilterPaginationModel>[] => [
  {
    key: "farmIds",
    label: "Ferma",
    type: "multiSelect",
    options:
      dictionary?.farms.map((farm) => ({
        value: farm.id,
        label: farm.name,
      })) || [],
    disabled: !dictionary,
  },
  {
    key: "cycles",
    label: "Identyfikator (cykl)",
    type: "multiSelect",
    options: uniqueCycles.map((cycle) => ({
      value: `${cycle.identifier}-${cycle.year}`,
      label: `${cycle.identifier}/${cycle.year}`,
    })),
    disabled: !dictionary,
  },
  {
    key: "incorrectPrices",
    label: "Niepoprawna cena",
    type: "select",
    options: [
      { value: "true", label: "Tak" },
      { value: "false", label: "Nie" },
    ],
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
];
