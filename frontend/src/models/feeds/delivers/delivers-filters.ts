import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsDeliversFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: FeedsDeliversFilterPaginationModel,
  action:
    | { type: "set"; key: keyof FeedsDeliversFilterPaginationModel; value: any }
    | {
        type: "setMultiple";
        payload: Partial<FeedsDeliversFilterPaginationModel>;
      }
): FeedsDeliversFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum FeedsDeliversOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  PriceDate = "PriceDate",
  Price = "Price",
  Name = "Name",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsDeliversFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  dateSince: string;
  dateTo: string;
}

export interface FeedsDeliversFilterPaginationModel
  extends FeedsDeliversFilter,
    OrderedPaginationParams<FeedsDeliversOrderType> {}
