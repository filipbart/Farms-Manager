import type {
  FarmDictModel,
  DictModel,
  CycleDictModel,
} from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export interface ProductionAnalysisDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}

export const initialFilters: ProductionAnalysisFilterPaginationModel = {
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
  state: ProductionAnalysisFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof ProductionAnalysisFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ProductionAnalysisFilterPaginationModel>;
      }
): ProductionAnalysisFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export default interface ProductionAnalysisFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface ProductionAnalysisFilterPaginationModel
  extends ProductionAnalysisFilter,
    OrderedPaginationParams {}
