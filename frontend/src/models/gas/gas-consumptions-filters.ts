import type { CycleDictModel } from "../common/dictionaries";
import type { OrderedPaginationParams } from "../common/pagination-params";

export const initialFilters: GasConsumptionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  showDeleted: false,
  page: 0,
  pageSize: 10,
};

export function filterReducer(
  state: GasConsumptionsFilterPaginationModel,
  action:
    | {
        type: "set";
        key: keyof GasConsumptionsFilterPaginationModel;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<GasConsumptionsFilterPaginationModel>;
      }
): GasConsumptionsFilterPaginationModel {
  let newState: GasConsumptionsFilterPaginationModel;

  switch (action.type) {
    case "set":
      newState = { ...state, [action.key]: action.value };
      break;
    case "setMultiple":
      newState = { ...state, ...action.payload };
      break;
    default:
      return state;
  }

  return newState;
}

export enum GasConsumptionsOrderType {
  Cycle = "Cycle",
  Farm = "Farm",
  QuantityConsumed = "QuantityConsumed",
  Cost = "Cost",
}

export const mapGasConsumptionOrderTypeToField = (
  orderType: GasConsumptionsOrderType
): string => {
  switch (orderType) {
    case GasConsumptionsOrderType.Cycle:
      return "cycleText";
    case GasConsumptionsOrderType.Farm:
      return "farmName";
    case GasConsumptionsOrderType.QuantityConsumed:
      return "quantityConsumed";
    case GasConsumptionsOrderType.Cost:
      return "cost";
    default:
      return "";
  }
};

export interface GasConsumptionsFilter {
  farmIds: string[];
  cycles: CycleDictModel[];
  showDeleted?: boolean;
}

export interface GasConsumptionsFilterPaginationModel
  extends GasConsumptionsFilter,
    OrderedPaginationParams<GasConsumptionsOrderType> {}
