import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsPricesFilterPaginationModel = {
  farmIds: [],
  feedNames: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
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
  let newState: FeedsPricesFilterPaginationModel;

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

export enum FeedsPricesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  PriceDate = "PriceDate",
  Price = "Price",
  Name = "Name",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPricesFilter {
  farmIds: string[];
  feedNames: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface FeedsPricesFilterPaginationModel
  extends FeedsPricesFilter,
    OrderedPaginationParams<FeedsPricesOrderType> {}
