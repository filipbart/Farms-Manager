import type { FilterConfig } from "../../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../../models/feeds/feeds-dictionary";
import type { FeedsPricesFilterPaginationModel } from "../../../../models/feeds/prices/price-filters";

export const getFeedsPricesFiltersConfig = (
  dictionary: FeedsDictionary | undefined,
  uniqueCycles: CycleDictModel[]
): FilterConfig<keyof FeedsPricesFilterPaginationModel>[] => [
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
