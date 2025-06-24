import type { OrderedPaginationParams } from "../common/pagination-params";
import type { CycleDictModel } from "./insertion-dictionary";

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
