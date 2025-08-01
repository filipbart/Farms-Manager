export interface HatcheryPriceListModel {
  id: string;
  hatcheryName: string;
  price: number;
  date: string;
  comment: string;
  dateCreatedUtc: string;
}

export interface AddHatcheryPriceFormData {
  hatcheryId: string;
  price: number;
  date: string;
  comment?: string;
}

export interface EditHatcherPriceFormData {
  price: number;
  date: string;
  comment?: string;
}
