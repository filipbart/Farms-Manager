import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";

export interface FallenStocksDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}

export interface FallenStockFilterModel {
  farmId?: string;
  cycleId?: string;
}

export const initialFilters: FallenStockFilterModel = {
  farmId: undefined,
  cycleId: undefined,
};

export function filterReducer(
  state: FallenStockFilterModel,
  action:
    | { type: "set"; key: keyof FallenStockFilterModel; value: any }
    | { type: "setMultiple"; payload: Partial<FallenStockFilterModel> }
): FallenStockFilterModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}
