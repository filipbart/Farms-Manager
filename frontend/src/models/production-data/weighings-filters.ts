import type {
  CycleDictModel,
  DictModel,
  FarmDictModel,
} from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: ProductionDataWeighingsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: ProductionDataWeighingsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionDataWeighingsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionDataWeighingsFilterPaginationModel>;
      }
): ProductionDataWeighingsFilterPaginationModel {
  let newState: ProductionDataWeighingsFilterPaginationModel;

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

export enum ProductionDataWeighingsOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  Henhouse = "Henhouse",
  Hatchery = "Hatchery",
  Weighing1Weight = "Weighing1Weight",
  Weighing2Weight = "Weighing2Weight",
  Weighing3Weight = "Weighing3Weight",
  Weighing4Weight = "Weighing4Weight",
  Weighing5Weight = "Weighing5Weight",
  DateCreatedUtc = "DateCreatedUtc",
}

export const mapProductionDataWeighingsOrderTypeToField = (
  orderType: ProductionDataWeighingsOrderType
): string => {
  switch (orderType) {
    case ProductionDataWeighingsOrderType.Cycle:
      return "cycleText";
    case ProductionDataWeighingsOrderType.Farm:
      return "farmName";
    case ProductionDataWeighingsOrderType.Henhouse:
      return "henhouseName";
    case ProductionDataWeighingsOrderType.Hatchery:
      return "hatcheryName";
    case ProductionDataWeighingsOrderType.Weighing1Weight:
      return "weighing1Weight";
    case ProductionDataWeighingsOrderType.Weighing2Weight:
      return "weighing2Weight";
    case ProductionDataWeighingsOrderType.Weighing3Weight:
      return "weighing3Weight";
    case ProductionDataWeighingsOrderType.Weighing4Weight:
      return "weighing4Weight";
    case ProductionDataWeighingsOrderType.Weighing5Weight:
      return "weighing5Weight";
    case ProductionDataWeighingsOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};

export interface ProductionDataWeighingsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
  showDeleted?: boolean;
}

export interface ProductionDataWeighingsFilterPaginationModel
  extends ProductionDataWeighingsFilter,
    OrderedPaginationParams<ProductionDataWeighingsOrderType> {}

export interface ProductionDataWeighingsDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}
