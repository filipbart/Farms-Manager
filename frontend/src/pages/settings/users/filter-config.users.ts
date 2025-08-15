import type { FilterConfig } from "../../../components/filters/filter-types";
import type { UsersFilterPaginationModel } from "../../../models/users/users-filters";

export const getUsersFiltersConfig = (): FilterConfig<
  keyof UsersFilterPaginationModel
>[] => [
  {
    key: "searchPhrase",
    label: "Szukaj użytkownika...",
    type: "text",
  },
];
