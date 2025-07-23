// import type { OrderedPaginationParams } from "../../common/pagination-params";

// export const initialFilters: FeedsCorrectionsFilterPaginationModel = {
//   page: 0,
//   pageSize: 10,
// };

// export function filterReducer(
//   state: FeedsCorrectionsFilterPaginationModel,
//   action:
//     | {
//         type: "set";
//         key: keyof FeedsCorrectionsFilterPaginationModel;
//         value: any;
//       }
//     | {
//         type: "setMultiple";
//         payload: Partial<FeedsCorrectionsFilterPaginationModel>;
//       }
// ): FeedsCorrectionsFilterPaginationModel {
//   switch (action.type) {
//     case "set":
//       return { ...state, [action.key]: action.value };
//     case "setMultiple":
//       return { ...state, ...action.payload };
//     default:
//       return state;
//   }
// }

// export enum FeedsDeliveriesOrderType {
//   DateCreatedUtc = "DateCreatedUtc",
// }

// // eslint-disable-next-line @typescript-eslint/no-empty-object-type
// export default interface FeedsDeliveriesFilter {}

// export interface FeedsCorrectionsFilterPaginationModel
//   extends FeedsDeliveriesFilter,
//     OrderedPaginationParams<FeedsDeliveriesOrderType> {}
