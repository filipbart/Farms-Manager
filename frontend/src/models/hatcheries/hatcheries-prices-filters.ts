import type { DictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

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

export const initialFilters: HatcheriesPricesFilterPaginationModel = {
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
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
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum HatcheriesPricesOrderType {
  HatcheryName = "HatcheryName",
  Price = "Price",
  Date = "Date",
}

export default interface HatcheriesPricesFilter {
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
  priceFrom?: number;
  priceTo?: number;
}

export interface HatcheriesPricesFilterPaginationModel
  extends HatcheriesPricesFilter,
    OrderedPaginationParams<HatcheriesPricesOrderType> {}

export interface HatcheriesPricesDictionary {
  hatcheries: DictModel[];
}
