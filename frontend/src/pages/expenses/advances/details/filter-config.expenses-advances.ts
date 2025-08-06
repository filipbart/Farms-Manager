import type { FilterConfig } from "../../../../components/filters/filter-types";
import type { ExpensesAdvancesFilterPaginationModel } from "../../../../models/expenses/advances/expenses-advances-filters";

export const getExpensesAdvancesFiltersConfig = (): FilterConfig<
  keyof ExpensesAdvancesFilterPaginationModel
>[] => [
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
