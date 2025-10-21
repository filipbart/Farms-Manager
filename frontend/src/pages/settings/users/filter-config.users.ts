import type { FilterConfig } from "../../../components/filters/filter-types";
import type { UsersFilterPaginationModel } from "../../../models/users/users-filters";

export const getUsersFiltersConfig = (
  isAdmin: boolean = false
): FilterConfig<keyof UsersFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof UsersFilterPaginationModel>[] = [
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
