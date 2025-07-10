import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsPricesFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: FeedsPricesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof FeedsPricesFilterPaginationModel; value: any }
    | {
        type: "setMultiple";
        payload: Partial<FeedsPricesFilterPaginationModel>;
      }
): FeedsPricesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum FeedsPricesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  PriceDate = "PriceDate",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPricesFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
}

export interface FeedsPricesFilterPaginationModel
  extends FeedsPricesFilter,
    OrderedPaginationParams<FeedsPricesOrderType> {}
