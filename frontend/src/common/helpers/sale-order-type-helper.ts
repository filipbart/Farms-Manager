import { SalesOrderType } from "../../models/sales/sales-filters";

export const mapSaleOrderTypeToField = (orderType: SalesOrderType): string => {
  switch (orderType) {
    case SalesOrderType.Cycle:
      return "cycleText";
    case SalesOrderType.Farm:
      return "farmName";
    case SalesOrderType.Henhouse:
      return "henhouseName";
    case SalesOrderType.SaleDate:
      return "insertionDate";

    case SalesOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
