import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: InsertionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: InsertionsFilterPaginationModel,
  action:
    | { type: "set"; key: keyof InsertionsFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<InsertionsFilterPaginationModel> }
): InsertionsFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum InsertionOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  InsertionDate = "InsertionDate",
  Quantity = "Quantity",
  Hatchery = "Hatchery",
  BodyWeight = "BodyWeight",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface InsertionsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface InsertionsFilterPaginationModel
  extends InsertionsFilter,
    OrderedPaginationParams<InsertionOrderType> {}
