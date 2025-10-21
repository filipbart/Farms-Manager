import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { ExpensesProductionsDictionary } from "../../../models/expenses/production/expenses-productions-dictionary";
import type { ExpensesProductionsFilterPaginationModel } from "../../../models/expenses/production/expenses-productions-filters";

export const getExpensesProductionFiltersConfig = (
  dictionary: ExpensesProductionsDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  _filters: ExpensesProductionsFilterPaginationModel,
  isAdmin: boolean = false
): FilterConfig<keyof ExpensesProductionsFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof ExpensesProductionsFilterPaginationModel>[] = [
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
    key: "contractorIds",
    label: "Kontrahent",
    type: "multiSelectSearch",
    options:
      dictionary?.contractors.map((contractor) => ({
        value: contractor.id,
        label: contractor.name,
      })) || [],
    disabled: !dictionary,
  },
  {
    key: "expensesTypesIds",
    label: "Typ wydatku",
    type: "multiSelect",
    options:
      dictionary?.expenseTypes.map((type) => ({
        value: type.id,
        label: type.name,
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
    key: "invoiceNumber",
    label: "Numer faktury",
    type: "text",
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
