import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type {
  SalesInvoicesDictionary,
  SalesInvoicesFilterPaginationModel,
} from "../../../models/sales/sales-invoices-filters";

export const getSalesInvoicesFiltersConfig = (
  dictionary: SalesInvoicesDictionary | undefined,
  uniqueCycles: CycleDictModel[]
): FilterConfig<keyof SalesInvoicesFilterPaginationModel>[] => [
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
    key: "slaughterhouseIds",
    label: "Nabywca",
    type: "multiSelect",
    options:
      dictionary?.slaughterhouses.map((s) => ({
        value: s.id,
        label: s.name,
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
