import type { CycleDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsPaymentsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  status: undefined,
  showDeleted: false,
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
  let newState: FeedsPaymentsFilterPaginationModel;

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

export enum FeedsPaymentsOrderType {
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface FeedsPaymentsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  status?: string;
  showDeleted?: boolean;
}

export interface FeedsPaymentsFilterPaginationModel
  extends FeedsPaymentsFilter,
    OrderedPaginationParams<FeedsPaymentsOrderType> {}
