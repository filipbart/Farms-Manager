
import type { FilterConfig } from "../../../components/filters/filter-types";

export interface ExpensesContractorsFilterPaginationModel {
  searchPhrase?: string;
  showDeleted?: boolean;
}

export const getExpensesContractorsFiltersConfig = (
  isAdmin: boolean = false
): FilterConfig<keyof ExpensesContractorsFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof ExpensesContractorsFilterPaginationModel>[] = [
  {
    key: "searchPhrase",
    label: "Szukaj",
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
