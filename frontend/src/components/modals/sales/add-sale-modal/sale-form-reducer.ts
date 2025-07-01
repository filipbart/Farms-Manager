import type { SaleFormState } from "./sale-form-types";

export const initialState: SaleFormState = {
  saleType: undefined,
  farmId: "",
  identifierId: "",
  identifierDisplay: "",
  saleDate: null,
  entries: [],
  basePrice: "",
  priceWithExtras: "",
  comment: "",
  otherExtras: [],
};

export function formReducer(state: SaleFormState, action: any): SaleFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "UPDATE_ENTRY":
      const updatedEntries = [...state.entries];
      updatedEntries[action.index] = {
        ...updatedEntries[action.index],
        [action.name]: action.value,
      };
      return { ...state, entries: updatedEntries };
    case "ADD_ENTRY":
      return {
        ...state,
        entries: [
          ...state.entries,
          {
            henhouseId: "",
            slaughterhouseId: "",
            quantity: "",
            weight: "",
            confiscatedCount: "",
            confiscatedWeight: "",
            deadCount: "",
            deadWeight: "",
            farmerWeight: "",
            isEditing: true,
          },
        ],
      };
    case "REMOVE_ENTRY":
      return {
        ...state,
        entries: state.entries.filter((_, idx) => idx !== action.index),
      };
    case "ADD_OTHER_EXTRA":
      return {
        ...state,
        otherExtras: [...state.otherExtras, { name: "", value: "" }],
      };
    case "REMOVE_OTHER_EXTRA":
      return {
        ...state,
        otherExtras: state.otherExtras.filter((_, idx) => idx !== action.index),
      };
    case "UPDATE_OTHER_EXTRA":
      return {
        ...state,
        otherExtras: state.otherExtras.map((extra, idx) =>
          idx === action.index
            ? { ...extra, [action.field]: action.value }
            : extra
        ),
      };
    case "RESET":
      return initialState;
    default:
      return state;
  }
}
