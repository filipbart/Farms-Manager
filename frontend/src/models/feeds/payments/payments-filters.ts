import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsPaymentsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: FeedsPaymentsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof FeedsPaymentsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<FeedsPaymentsFilterPaginationModel>;
      }
): FeedsPaymentsFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum FeedsPaymentsOrderType {
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPaymentsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
}

export interface FeedsPaymentsFilterPaginationModel
  extends FeedsPaymentsFilter,
    OrderedPaginationParams<FeedsPaymentsOrderType> {}
