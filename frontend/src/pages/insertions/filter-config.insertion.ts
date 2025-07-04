import type { FilterConfig } from "../../components/filters/filter-types";
import type { CycleDictModel } from "../../models/common/dictionaries";
import type { InsertionDictionary } from "../../models/insertions/insertion-dictionary";
import type { InsertionsFilterPaginationModel } from "../../models/insertions/insertions-filters";

export const getInsertionFiltersConfig = (
  dictionary: InsertionDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  filters: InsertionsFilterPaginationModel
): FilterConfig<keyof InsertionsFilterPaginationModel>[] => [
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
    key: "henhouseIds",
    label: "Kurnik",
    type: "multiSelect",
    options:
      dictionary?.farms
        .filter((farm) => filters.farmIds.includes(farm.id))
        .flatMap((farm) =>
          farm.henhouses.map((henhouse) => ({
            value: henhouse.id,
            label: henhouse.name,
          }))
        ) || [],
    disabled: !dictionary || filters.farmIds.length === 0,
  },
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
