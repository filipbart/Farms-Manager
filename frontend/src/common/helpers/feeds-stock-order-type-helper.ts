import { FeedsStocksOrderType } from "../../models/feeds/stocks/stock-filters";

export const mapFeedsStocksOrderTypeToField = (
  orderType: FeedsStocksOrderType
): string => {
  switch (orderType) {
    case FeedsStocksOrderType.Cycle:
      return "cycleText";
    case FeedsStocksOrderType.Farm:
      return "farmName";
    case FeedsStocksOrderType.StockDate:
      return "stockDate";
    case FeedsStocksOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
