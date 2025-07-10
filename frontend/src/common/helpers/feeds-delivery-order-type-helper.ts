import { FeedsDeliversOrderType } from "../../models/feeds/delivers/delivers-filters";

export const mapFeedsDeliversOrderTypeToField = (
  orderType: FeedsDeliversOrderType
): string => {
  switch (orderType) {
    case FeedsDeliversOrderType.Cycle:
      return "cycleText";
    case FeedsDeliversOrderType.Farm:
      return "farmName";
    case FeedsDeliversOrderType.PriceDate:
      return "priceDate";
    case FeedsDeliversOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
