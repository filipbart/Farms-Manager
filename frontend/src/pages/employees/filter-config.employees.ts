import type { FilterConfig } from "../../components/filters/filter-types";
import type {
  EmployeesDictionary,
  EmployeesFilterPaginationModel,
} from "../../models/employees/employees-filters";

export const getEmployeesFiltersConfig = (
  dictionary: EmployeesDictionary | undefined
): FilterConfig<keyof EmployeesFilterPaginationModel>[] => [
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
];
