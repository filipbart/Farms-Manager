import type { FilterConfig } from "../../components/filters/filter-types";
import { EmployeeStatus } from "../../models/employees/employees";
import type {
  EmployeesDictionary,
  EmployeesFilterPaginationModel,
} from "../../models/employees/employees-filters";

export const getEmployeesFiltersConfig = (
  dictionary: EmployeesDictionary | undefined,
  isAdmin: boolean = false
): FilterConfig<keyof EmployeesFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof EmployeesFilterPaginationModel>[] = [
    {
      key: "searchPhrase",
      label: "Szukaj pracownika...",
      type: "text",
    },
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
      key: "status",
      label: "Status pracownika",
      type: "select",
      options: [
        { value: EmployeeStatus.Active, label: "Aktywny" },
        { value: EmployeeStatus.Inactive, label: "Nieaktywny" },
      ],
    },
  ];

  // Dodaj checkbox "Pokaż usunięte" tylko dla admina
  if (isAdmin) {
    baseFilters.push({
      key: "showDeleted",
      label: "Pokaż usunięte",
      type: "checkbox",
    });
  }

  return baseFilters;
};
