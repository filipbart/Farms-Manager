import type { FarmDictModel, CycleDictModel } from "../common/dictionaries";

export const initialFilters: FallenStockFilterModel = {
  farmId: undefined,
  cycle: undefined,
  showDeleted: false,
};

export function filterReducer(
  state: FallenStockFilterModel,
  action:
    | { type: "set"; key: keyof FallenStockFilterModel; value: any }
    | { type: "setMultiple"; payload: Partial<FallenStockFilterModel> }
): FallenStockFilterModel {
  let newState: FallenStockFilterModel;

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

export interface FallenStocksDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}

export interface FallenStockFilterModel {
  farmId?: string;
  cycle?: string;
  showDeleted?: boolean;
}
