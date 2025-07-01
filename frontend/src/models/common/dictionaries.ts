export interface FarmDictModel extends DictModel {
  henhouses: DictModel[];
}

export interface DictModel {
  id: string;
  name: string;
}

export interface CycleDictModel {
  id: string;
  identifier: number;
  year: number;
}
