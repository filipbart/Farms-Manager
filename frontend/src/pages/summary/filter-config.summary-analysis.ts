import type { FilterConfig } from "../../components/filters/filter-types";
import type { CycleDictModel } from "../../models/common/dictionaries";
import type {
  AnalysisDictionary,
  AnalysisFilterPaginationModel,
} from "../../models/summary/analysis-filters";

export const getSummaryAnalysisFiltersConfig = (
  dictionary: AnalysisDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  filters: AnalysisFilterPaginationModel
): FilterConfig<keyof AnalysisFilterPaginationModel>[] => [
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
