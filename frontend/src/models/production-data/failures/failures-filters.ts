import type { CycleDictModel, FarmDictModel } from "../../common/dictionaries";
import type { OrderedPaginationParams } from "../../common/pagination-params";

export const initialFilters: ProductionDataFailureFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: ProductionDataFailureFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionDataFailureFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionDataFailureFilterPaginationModel>;
      }
): ProductionDataFailureFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum ProductionDataFailureOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  DeadCount = "DeadCount",
  DefectiveCount = "DefectiveCount",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapProductionDataFailureOrderTypeToField = (
  orderType: ProductionDataFailureOrderType
): string => {
  switch (orderType) {
    case ProductionDataFailureOrderType.Cycle:
      return "cycleText";
    case ProductionDataFailureOrderType.Farm:
      return "farmName";
    case ProductionDataFailureOrderType.Henhouse:
      return "henhouseName";
    case ProductionDataFailureOrderType.DeadCount:
      return "deadCount";
    case ProductionDataFailureOrderType.DefectiveCount:
      return "defectiveCount";
    case ProductionDataFailureOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface ProductionDataFailureFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface ProductionDataFailureFilterPaginationModel
  extends ProductionDataFailureFilter,
    OrderedPaginationParams<ProductionDataFailureOrderType> {}

export interface ProductionDataFailureDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}
