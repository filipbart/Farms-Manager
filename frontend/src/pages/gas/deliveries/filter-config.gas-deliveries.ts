import type { FilterConfig } from "../../../components/filters/filter-types";
import type { GasDeliveriesDictionary } from "../../../models/gas/gas-deliveries-dictionary";
import type { GasDeliveriesFilterPaginationModel } from "../../../models/gas/gas-deliveries-filters";

export const getGasDeliveriesFiltersConfig = (
  dictionary: GasDeliveriesDictionary | undefined
): FilterConfig<keyof GasDeliveriesFilterPaginationModel>[] => [
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
    key: "contractorIds",
    label: "Kontrahent",
    type: "multiSelect",
    options:
      dictionary?.contractors.map((contractor) => ({
        value: contractor.id,
        label: contractor.name,
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
];
