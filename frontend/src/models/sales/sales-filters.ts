import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: SalesFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  slaughterhouseIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: SalesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof SalesFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<SalesFilterPaginationModel> }
): SalesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
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
}

export interface SalesFilterPaginationModel
  extends SalesFilter,
    OrderedPaginationParams<SalesOrderType> {}
