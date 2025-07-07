import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsStocksFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: FeedsStocksFilterPaginationModel,
  action:
    | { type: "set"; key: keyof FeedsStocksFilterPaginationModel; value: any }
    | {
        type: "setMultiple";
        payload: Partial<FeedsStocksFilterPaginationModel>;
      }
): FeedsStocksFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum FeedsStocksOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  StockDate = "StockDate",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsStocksFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
}

export interface FeedsStocksFilterPaginationModel
  extends FeedsStocksFilter,
    OrderedPaginationParams<FeedsStocksOrderType> {}
