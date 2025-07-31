import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: GasDeliveriesFilterPaginationModel = {
  farmIds: [],
  contractorIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: GasDeliveriesFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof GasDeliveriesFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<GasDeliveriesFilterPaginationModel>;
      }
): GasDeliveriesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

export enum GasDeliveriesOrderType {
  Farm = "Farm",
  Contractor = "Contractor",
  InvoiceDate = "InvoiceDate",
  InvoiceNumber = "InvoiceNumber",
  UnitPrice = "UnitPrice",
  Quantity = "Quantity",
}

export const mapGasDeliveryOrderTypeToField = (
  orderType: GasDeliveriesOrderType
): string => {
  switch (orderType) {
    case GasDeliveriesOrderType.Farm:
      return "farmName";
    case GasDeliveriesOrderType.Contractor:
      return "contractorName";
    case GasDeliveriesOrderType.InvoiceDate:
      return "invoiceDate";
    case GasDeliveriesOrderType.InvoiceNumber:
      return "invoiceNumber";
    case GasDeliveriesOrderType.UnitPrice:
      return "unitPrice";
    case GasDeliveriesOrderType.Quantity:
      return "quantity";
    default:
      return "";
  }
};

export interface GasDeliveriesFilter {
  farmIds: string[];
  contractorIds: string[];
  dateSince: string;
  dateTo: string;
}

export interface GasDeliveriesFilterPaginationModel
  extends GasDeliveriesFilter,
    OrderedPaginationParams<GasDeliveriesOrderType> {}
