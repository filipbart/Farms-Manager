import { FeedsPaymentsOrderType } from "../../models/feeds/payments/payments-filters";

export const mapFeedsPaymentsOrderTypeToField = (
  orderType: FeedsPaymentsOrderType
): string => {
  switch (orderType) {
    case FeedsPaymentsOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
