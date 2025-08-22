import type { CycleDictModel, FarmDictModel } from "../common/dictionaries";

export interface DashboardDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}

export interface DashboardFilters {
  farmId?: string;
  cycle?: string;
  dateSince?: string | null;
  dateTo?: string | null;
  dateCategory: string;
}

export const initialFilters: DashboardFilters = {
  farmId: undefined,
  cycle: undefined,
  dateSince: null,
  dateTo: null,
  dateCategory: "",
};

export function filterReducer(
  state: DashboardFilters,
  action:
    | {
        type: "set";
        key: keyof DashboardFilters;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<DashboardFilters>;
      }
): DashboardFilters {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}
