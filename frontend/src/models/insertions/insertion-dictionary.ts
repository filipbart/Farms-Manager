import type {
  FarmDictModel,
  HatcheryDictModel,
  CycleDictModel,
} from "../common/dictionaries";

export interface InsertionDictionary {
  farms: FarmDictModel[];
  hatcheries: HatcheryDictModel[];
  cycles: CycleDictModel[];
}
