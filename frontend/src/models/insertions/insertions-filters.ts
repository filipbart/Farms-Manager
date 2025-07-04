import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

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
