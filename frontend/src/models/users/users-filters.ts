import type { OrderedPaginationParams } from "../common/pagination-params";

const LOCAL_STORAGE_KEY = "usersFilters";

const saveFiltersToLocalStorage = (filters: UsersFilterPaginationModel) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<UsersFilterPaginationModel> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): UsersFilterPaginationModel => {
  const defaultFilters: UsersFilterPaginationModel = {
    searchPhrase: "",
    page: 0,
    pageSize: 10,
  };

  const savedFilters = loadFiltersFromLocalStorage();

  if (savedFilters) {
    return {
      ...defaultFilters,
      ...savedFilters,

      page: 0,
    };
  }

  return defaultFilters;
};

export const initialFilters = getInitialFilters();

export function filterReducer(
  state: UsersFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof UsersFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<UsersFilterPaginationModel>;
      }
): UsersFilterPaginationModel {
  let newState: UsersFilterPaginationModel;

  switch (action.type) {
    case "set":
      newState = { ...state, [action.key]: action.value };
      break;
    case "setMultiple":
      newState = { ...state, ...action.payload };
      break;
    default:
      return state;
  }

  saveFiltersToLocalStorage(newState);
  return newState;
}

export enum UsersOrderType {
  Login = "Login",
  Name = "Name",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapUserOrderTypeToField = (orderType: UsersOrderType): string => {
  switch (orderType) {
    case UsersOrderType.Login:
      return "login";
    case UsersOrderType.Name:
      return "name";
    case UsersOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface UsersFilter {
  searchPhrase?: string;
}

export interface UsersFilterPaginationModel
  extends UsersFilter,
    OrderedPaginationParams<UsersOrderType> {}
