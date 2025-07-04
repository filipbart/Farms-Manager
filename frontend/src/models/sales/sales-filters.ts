import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export enum SalesOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  SaleDate = "SaleDate",
  //   WeightSlaughter = "WeightSlaughter",
  //   QuantitySlaughter = "QuantitySlaughter",
  DateCreatedUtc = "DateCreatedUtc",
}

export default interface SalesFilter {
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
