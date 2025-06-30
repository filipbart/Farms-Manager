export interface FarmDictModel {
  id: string;
  name: string;
  henhouses: HenhouseDictModel[];
}

export interface HenhouseDictModel {
  id: string;
  name: string;
}

export interface HatcheryDictModel {
  id: string;
  name: string;
}

export interface CycleDictModel {
  id: string;
  identifier: number;
  year: number;
}
