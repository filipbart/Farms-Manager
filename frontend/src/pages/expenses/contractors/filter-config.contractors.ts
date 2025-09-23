
import type { FilterConfig } from "../../../components/filters/filter-types";

export interface ExpensesContractorsFilterPaginationModel {
  searchPhrase?: string;
}

export const getExpensesContractorsFiltersConfig = (): FilterConfig<
  keyof ExpensesContractorsFilterPaginationModel
>[] => [
  {
    key: "searchPhrase",
    label: "Szukaj kontrahenta...",
    type: "text",
  },
];
