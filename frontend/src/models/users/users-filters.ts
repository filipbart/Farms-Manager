import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: UsersFilterPaginationModel = {
  searchPhrase: "",
  showDeleted: false,
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
  showDeleted?: boolean;
}

export interface UsersFilterPaginationModel
  extends UsersFilter,
    OrderedPaginationParams<UsersOrderType> {}
