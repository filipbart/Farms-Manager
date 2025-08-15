import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: UsersFilterPaginationModel = {
  searchPhrase: "",
  page: 0,
  pageSize: 10,
};

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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
