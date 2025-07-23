import { FeedsDeliveriesOrderType } from "../../models/feeds/deliveries/deliveries-filters";

export const mapFeedsDeliveriesOrderTypeToField = (
  orderType: FeedsDeliveriesOrderType
): string => {
  switch (orderType) {
    case FeedsDeliveriesOrderType.Cycle:
      return "cycleText";
    case FeedsDeliveriesOrderType.Farm:
      return "farmName";
    case FeedsDeliveriesOrderType.ItemName:
      return "itemName";
    case FeedsDeliveriesOrderType.VendorName:
      return "vendorName";
    case FeedsDeliveriesOrderType.UnitPrice:
      return "unitPrice";
    case FeedsDeliveriesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
