import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: HatcheriesPricesFilterPaginationModel = {
  hatcheryNames: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: HatcheriesPricesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof HatcheriesPricesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<HatcheriesPricesFilterPaginationModel>;
      }
): HatcheriesPricesFilterPaginationModel {
  let newState: HatcheriesPricesFilterPaginationModel;

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

export enum HatcheriesPricesOrderType {
  HatcheryName = "HatcheryName",
  Price = "Price",
  Date = "Date",
}

export const mapHatcheriesPricesOrderTypeToField = (
  orderType: HatcheriesPricesOrderType
): string => {
  switch (orderType) {
    case HatcheriesPricesOrderType.HatcheryName:
      return "hatcheryName";
    case HatcheriesPricesOrderType.Price:
      return "price";
    case HatcheriesPricesOrderType.Date:
      return "date";
    default:
      return "";
  }
};

export default interface HatcheriesPricesFilter {
  hatcheryNames: string[];
  dateSince: string;
  dateTo: string;
  priceFrom?: number;
  priceTo?: number;
  showDeleted?: boolean;
}

export interface HatcheriesPricesFilterPaginationModel
  extends HatcheriesPricesFilter,
    OrderedPaginationParams<HatcheriesPricesOrderType> {}
