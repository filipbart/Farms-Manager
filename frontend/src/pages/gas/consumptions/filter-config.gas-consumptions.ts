import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { GasConsumptionsFilterPaginationModel } from "../../../models/gas/gas-consumptions-filters";
import type { GasConsumptionsDictionary } from "../../../models/gas/gas-dictionaries";

export const getGasConsumptionsFiltersConfig = (
  dictionary: GasConsumptionsDictionary | undefined,
  uniqueCycles: CycleDictModel[]
): FilterConfig<keyof GasConsumptionsFilterPaginationModel>[] => [
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
];
