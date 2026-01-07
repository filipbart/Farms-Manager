import type { CycleDictModel, FarmDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: ProductionDataFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: ProductionDataFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionDataFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionDataFilterPaginationModel>;
      }
): ProductionDataFilterPaginationModel {
  let newState: ProductionDataFilterPaginationModel;

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

export enum ProductionDataOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  DeadCount = "DeadCount",
  DefectiveCount = "DefectiveCount",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapProductionDataOrderTypeToField = (
  orderType: ProductionDataOrderType
): string => {
  switch (orderType) {
    case ProductionDataOrderType.Cycle:
      return "cycleText";
    case ProductionDataOrderType.Farm:
      return "farmName";
    case ProductionDataOrderType.Henhouse:
      return "henhouseName";
    case ProductionDataOrderType.DeadCount:
      return "deadCount";
    case ProductionDataOrderType.DefectiveCount:
      return "defectiveCount";
    case ProductionDataOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface ProductionDataFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface ProductionDataFilterPaginationModel
  extends ProductionDataFilter,
    OrderedPaginationParams<ProductionDataOrderType> {}

export interface ProductionDataDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}
