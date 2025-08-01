export interface FeedPriceListModel {
  id: string;
  farmName: string;
  cycleText: string;
  priceDate: string;
  name: string;
  price: number;
  dateCreatedUtc: string;
}

export interface AddFeedPriceFormData {
  farmId: string;
  identifierId: string;
  identifierDisplay?: string;
  priceDate: string;
  nameId: string;
  price: number;
}

export interface UpdateFeedPriceFormData {
  priceDate: string;
  nameId: string;
  price: number;
}
