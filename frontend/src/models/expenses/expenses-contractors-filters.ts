export const initialFilters: ExpensesContractorsFilters = {
  searchPhrase: "",
  showDeleted: false,
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
  let newState: ExpensesContractorsFilters;

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

export interface ExpensesContractorsFilters {
  searchPhrase: string;
  showDeleted?: boolean;
}
