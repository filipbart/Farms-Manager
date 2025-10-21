import type { FilterConfig } from "../../../../components/filters/filter-types";
import type { ExpensesAdvancesFilterPaginationModel } from "../../../../models/expenses/advances/expenses-advances-filters";

const monthOptions = [
  { value: "1", label: "Styczeń" },
  { value: "2", label: "Luty" },
  { value: "3", label: "Marzec" },
  { value: "4", label: "Kwiecień" },
  { value: "5", label: "Maj" },
  { value: "6", label: "Czerwiec" },
  { value: "7", label: "Lipiec" },
  { value: "8", label: "Sierpień" },
  { value: "9", label: "Wrzesień" },
  { value: "10", label: "Październik" },
  { value: "11", label: "Listopad" },
  { value: "12", label: "Grudzień" },
];

const generateYearOptions = () => {
  const currentYear = new Date().getFullYear();
  const years = [];
  for (let i = 0; i < 10; i++) {
    const year = currentYear - i;
    years.push({ value: year.toString(), label: year.toString() });
  }
  return years;
};

export const getExpensesAdvancesFiltersConfig = (
  isAdmin: boolean = false
): FilterConfig<keyof ExpensesAdvancesFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof ExpensesAdvancesFilterPaginationModel>[] = [
    {
      key: "years",
      label: "Lata",
      type: "multiSelect",
      options: generateYearOptions(),
    },
    {
      key: "months",
      label: "Miesiące",
      type: "multiSelect",
      options: monthOptions,
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
