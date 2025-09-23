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
    case FeedsDeliveriesOrderType.HenhouseName:
      return "henhouseName";
    case FeedsDeliveriesOrderType.InvoiceNumber:
      return "invoiceNumber";
    case FeedsDeliveriesOrderType.Quantity:
      return "quantity";
    case FeedsDeliveriesOrderType.InvoiceDate:
      return "invoiceDate";
    case FeedsDeliveriesOrderType.DueDate:
      return "dueDate";
    case FeedsDeliveriesOrderType.InvoiceTotal:
      return "invoiceTotal";
    case FeedsDeliveriesOrderType.SubTotal:
      return "subTotal";
    case FeedsDeliveriesOrderType.VatAmount:
      return "vatAmount";
    case FeedsDeliveriesOrderType.PaymentDateUtc:
      return "paymentDateUtc";
    case FeedsDeliveriesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
