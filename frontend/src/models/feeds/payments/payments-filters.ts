import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: FeedsPaymentsFilterPaginationModel = {
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

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export default interface FeedsPaymentsFilter {}

export interface FeedsPaymentsFilterPaginationModel
  extends FeedsPaymentsFilter,
    OrderedPaginationParams<FeedsPaymentsOrderType> {}
