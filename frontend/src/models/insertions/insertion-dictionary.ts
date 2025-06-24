interface FarmDictModel {
  id: string;
  name: string;
  henhouses: HenhouseDictModel[];
}

interface HenhouseDictModel {
  id: string;
  name: string;
}

interface HatcheryDictModel {
  id: string;
  name: string;
}

export interface CycleDictModel {
  id: string;
  identifier: number;
  year: number;
}

export interface InsertionDictionary {
  farms: FarmDictModel[];
  hatcheries: HatcheryDictModel[];
  cycles: CycleDictModel[];
}
