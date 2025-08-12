import type {
  FarmDictModel,
  DictModel,
  CycleDictModel,
} from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export interface AnalysisDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}

export const initialFilters: AnalysisFilterPaginationModel = {
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
  state: AnalysisFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof AnalysisFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<AnalysisFilterPaginationModel>;
      }
): AnalysisFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export default interface AnalysisFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  henhouseIds: string[];
  hatcheryIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface AnalysisFilterPaginationModel
  extends AnalysisFilter,
    OrderedPaginationParams {}
