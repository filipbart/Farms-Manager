import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: SalesFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  slaughterhouseIds: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: SalesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof SalesFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<SalesFilterPaginationModel> }
): SalesFilterPaginationModel {
  let newState: SalesFilterPaginationModel;

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

export enum SalesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  SaleDate = "SaleDate",
  //   WeightSlaughter = "WeightSlaughter",
  //   QuantitySlaughter = "QuantitySlaughter",
  DateCreatedUtc = "DateCreatedUtc",
}

interface SalesFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  slaughterhouseIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface SalesFilterPaginationModel
  extends SalesFilter,
    OrderedPaginationParams<SalesOrderType> {}
