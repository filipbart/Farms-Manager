import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { SalesDictionary } from "../../../models/sales/sales-dictionary";
import type { SalesInvoicesFilterPaginationModel } from "../../../models/sales/sales-invoices-filters";

export const getSalesInvoicesFiltersConfig = (
  dictionary: SalesDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  _filters: SalesInvoicesFilterPaginationModel,
  isAdmin: boolean = false
): FilterConfig<keyof SalesInvoicesFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof SalesInvoicesFilterPaginationModel>[] = [
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

  if (isAdmin) {
    baseFilters.push({
      key: "showDeleted",
      label: "Pokaż usunięte",
      type: "checkbox",
    });
  }

  return baseFilters;
};
