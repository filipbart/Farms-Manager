import type {
  CycleDictModel,
  DictModel,
  FarmDictModel,
} from "../../common/dictionaries";

export interface ExpensesProductionsDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
  expenseTypes: DictModel[];
  contractors: DictModel[];
}
