import type {
  FarmDictModel,
  DictModel,
  CycleDictModel,
} from "../common/dictionaries";

export interface SalesDictionary {
  farms: FarmDictModel[];
  slaughterhouses: DictModel[];
  cycles: CycleDictModel[];
}
