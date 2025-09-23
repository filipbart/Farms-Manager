export interface ExpensesContractorsFilters {
  searchPhrase: string;
}

export const initialFilters: ExpensesContractorsFilters = {
  searchPhrase: "",
};

export function filterReducer(
  state: ExpensesContractorsFilters,
  action:
    | {
        type: "set";
        key: keyof ExpensesContractorsFilters;
        value: any;
      }
    | {
        type: "setMultiple";
        payload: Partial<ExpensesContractorsFilters>;
      }
): ExpensesContractorsFilters {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}
