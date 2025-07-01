import type {
  FarmDictModel,
  CycleDictModel,
  DictModel,
} from "../common/dictionaries";

export interface InsertionDictionary {
  farms: FarmDictModel[];
  hatcheries: DictModel[];
  cycles: CycleDictModel[];
}
