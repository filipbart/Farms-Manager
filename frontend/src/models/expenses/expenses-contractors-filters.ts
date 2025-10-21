const LOCAL_STORAGE_KEY = "expensesContractorsFilters";

const saveFiltersToLocalStorage = (filters: ExpensesContractorsFilters) => {
  try {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(filters));
  } catch (error) {
    console.error("Failed to save filters to localStorage", error);
  }
};

export const loadFiltersFromLocalStorage =
  (): Partial<ExpensesContractorsFilters> | null => {
    try {
      const savedFilters = localStorage.getItem(LOCAL_STORAGE_KEY);
      return savedFilters ? JSON.parse(savedFilters) : null;
    } catch (error) {
      console.error("Failed to load filters from localStorage", error);
      return null;
    }
  };

const getInitialFilters = (): ExpensesContractorsFilters => {
  const defaultFilters: ExpensesContractorsFilters = {
    searchPhrase: "",
    showDeleted: false,
  };

  const savedFilters = loadFiltersFromLocalStorage();

  if (savedFilters) {
    return {
      ...defaultFilters,
      ...savedFilters,
    };
  }

  return defaultFilters;
};

export const initialFilters = getInitialFilters();

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

  saveFiltersToLocalStorage(newState);
  return newState;
}

export interface ExpensesContractorsFilters {
  searchPhrase: string;
  showDeleted?: boolean;
}
