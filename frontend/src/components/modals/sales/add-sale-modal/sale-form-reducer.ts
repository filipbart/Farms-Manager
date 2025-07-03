import type { SaleFormState } from "../../../../models/sales/sale-form-states";

export const initialState: SaleFormState = {
  saleType: undefined,
  farmId: "",
  identifierId: "",
  slaughterhouseId: "",
  identifierDisplay: "",
  saleDate: null,
  entries: [],
  entriesTableReady: [],
};

export function formReducer(state: SaleFormState, action: any): SaleFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };

    case "UPDATE_ENTRY": {
      const updatedEntries1 = [...state.entries];
      updatedEntries1[action.index] = {
        ...updatedEntries1[action.index],
        [action.name]: action.value,
      };
      return { ...state, entries: updatedEntries1 };
    }

    case "MARK_ENTRY_AS_READY":
      return {
        ...state,
        entriesTableReady: [...state.entriesTableReady, action.index],
      };

    case "UNMARK_ENTRY_AS_READY":
      return {
        ...state,
        entriesTableReady: state.entriesTableReady.filter(
          (i) => i !== action.index
        ),
      };

    case "ADD_ENTRY":
      return {
        ...state,
        entries: [
          ...state.entries,
          {
            henhouseId: "",
            henhouseName: "",
            basePrice: "",
            priceWithExtras: "",
            comment: "",
            otherExtras: [],
            quantity: "",
            weight: "",
            confiscatedCount: "",
            confiscatedWeight: "",
            deadCount: "",
            deadWeight: "",
            farmerWeight: "",
          },
        ],
      };

    case "REMOVE_ENTRY": {
      const updatedEntries = state.entries.filter(
        (_, idx) => idx !== action.index
      );
      const updatedReady = state.entriesTableReady
        .filter((i) => i !== action.index)
        .map((i) => (i > action.index ? i - 1 : i));

      return {
        ...state,
        entries: updatedEntries,
        entriesTableReady: updatedReady,
      };
    }

    case "ADD_OTHER_EXTRA": {
      const updatedEntries = [...state.entries];
      const entry = updatedEntries[action.entryIndex];

      if (!entry) return state;

      const updatedOtherExtras = [
        ...(entry.otherExtras || []),
        { name: "", value: "" } as const,
      ];

      updatedEntries[action.entryIndex] = {
        ...entry,
        otherExtras: updatedOtherExtras,
      };

      return {
        ...state,
        entries: updatedEntries,
      };
    }

    case "REMOVE_OTHER_EXTRA": {
      const updatedEntries3 = [...state.entries];
      const entry = updatedEntries3[action.entryIndex];

      if (!entry) return state;

      const updatedOtherExtras = (entry.otherExtras || []).filter(
        (_, idx) => idx !== action.extraIndex
      );

      updatedEntries3[action.entryIndex] = {
        ...entry,
        otherExtras: updatedOtherExtras,
      };

      return { ...state, entries: updatedEntries3 };
    }

    case "UPDATE_OTHER_EXTRA": {
      const updatedEntries4 = [...state.entries];
      const entry = updatedEntries4[action.entryIndex];

      if (!entry) return state;

      const updatedOtherExtras = (entry.otherExtras || []).map((extra, idx) => {
        if (idx !== action.extraIndex) return extra;

        let newValue = action.value;

        if (action.field === "value") {
          if (newValue === "") {
            newValue = "" as const;
          } else {
            newValue = Number(newValue);
          }
        }

        return { ...extra, [action.field]: newValue };
      });

      updatedEntries4[action.entryIndex] = {
        ...entry,
        otherExtras: updatedOtherExtras,
      };

      return { ...state, entries: updatedEntries4 };
    }

    case "RESET":
      return initialState;

    default:
      return state;
  }
}
