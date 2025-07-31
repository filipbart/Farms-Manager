import type {
  FarmDictModel,
  DictModel,
  CycleDictModel,
} from "../common/dictionaries";

export interface GasDeliveriesDictionary {
  farms: FarmDictModel[];
  contractors: DictModel[];
}

export interface GasConsumptionsDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}
