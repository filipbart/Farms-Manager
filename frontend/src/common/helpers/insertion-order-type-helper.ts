import { InsertionOrderType } from "../../models/insertions/insertions-filters";

export const mapInsertionOrderTypeToField = (
  orderType: InsertionOrderType
): string => {
  switch (orderType) {
    case InsertionOrderType.Cycle:
      return "cycleText";
    case InsertionOrderType.Farm:
      return "farmName";
    case InsertionOrderType.Henhouse:
      return "henhouseName";
    case InsertionOrderType.InsertionDate:
      return "insertionDate";
    case InsertionOrderType.Quantity:
      return "quantity";
    case InsertionOrderType.Hatchery:
      return "hatcheryName";
    case InsertionOrderType.BodyWeight:
      return "bodyWeight";
    case InsertionOrderType.DateCreatedUtc:
      return "dateCreatedUtc";
    default:
      return "";
  }
};
