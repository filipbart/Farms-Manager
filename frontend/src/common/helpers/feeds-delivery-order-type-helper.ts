import { FeedsDeliveriesOrderType } from "../../models/feeds/deliveries/deliveries-filters";

export const mapFeedsDeliveriesOrderTypeToField = (
  orderType: FeedsDeliveriesOrderType
): string => {
  switch (orderType) {
    case FeedsDeliveriesOrderType.Cycle:
      return "cycleText";
    case FeedsDeliveriesOrderType.Farm:
      return "farmName";
    case FeedsDeliveriesOrderType.PriceDate:
      return "priceDate";
    case FeedsDeliveriesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
