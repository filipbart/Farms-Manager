import type { AuditFields } from "../../../common/interfaces/audit-fields";

export interface FeedPriceListModel extends AuditFields {
  id: string;
  farmId: string;
  farmName: string;
  cycleId: string;
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
  entries: AddFeedPriceEntry[];
}

export interface AddFeedPriceEntry {
  nameId: string;
  price?: number;
}

export interface UpdateFeedPriceFormData {
  priceDate: string;
  nameId: string;
  price: number;
  cycleId: string;
}
