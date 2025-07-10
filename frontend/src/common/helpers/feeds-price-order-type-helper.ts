import { FeedsPricesOrderType } from "../../models/feeds/prices/price-filters";

export const mapFeedsPricesOrderTypeToField = (
  orderType: FeedsPricesOrderType
): string => {
  switch (orderType) {
    case FeedsPricesOrderType.Cycle:
      return "cycleText";
    case FeedsPricesOrderType.Farm:
      return "farmName";
    case FeedsPricesOrderType.PriceDate:
      return "priceDate";
    case FeedsPricesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
