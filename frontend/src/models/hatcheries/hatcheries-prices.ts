export interface HatcheryPriceListModel {
  id: string;
  hatcheryName: string;
  price: number;
  date: string;
  comment: string;
  dateCreatedUtc: string;
}

export interface AddHatcheryPriceFormData {
  hatcheryName: string;
  price: number;
  date: string;
  comment?: string;
}

export interface EditHatcherPriceFormData {
  price: number;
  date: string;
  comment?: string;
}

export interface HatcheriesNames {
  hatcheries: HatcheryName[];
}

export interface HatcheryName {
  id: string;
  name: string;
}
