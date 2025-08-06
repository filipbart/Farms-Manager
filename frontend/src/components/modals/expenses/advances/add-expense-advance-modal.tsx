import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
} from "@mui/material";
import { useEffect, useReducer, useState } from "react";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { useAdvanceCategories } from "../../../../hooks/expenses/advances/useAdvanceCategories";
import { AdvanceType } from "../../../../models/expenses/advances/categories";
import type {
  ExpenseAdvanceEntry,
  AddExpenseAdvance,
} from "../../../../models/expenses/advances/expenses-advances";
import AdvanceEntriesTable from "./expense-advance-entries-table";
import { ExpensesAdvancesService } from "../../../../services/expenses-advances-service";

interface AddExpenseAdvanceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  employeeId: string;
}

interface AdvanceFormState {
  type: AdvanceType;
  entries: ExpenseAdvanceEntry[];
}

const initialState: AdvanceFormState = {
  type: AdvanceType.Expense,
  entries: [],
};

function formReducer(state: AdvanceFormState, action: any): AdvanceFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "UPDATE_ENTRY": {
      const updatedEntries = [...state.entries];
      updatedEntries[action.index] = {
        ...updatedEntries[action.index],
        [action.name]: action.value,
      };
      return { ...state, entries: updatedEntries };
    }
    case "ADD_ENTRY":
      return {
        ...state,
        entries: [
          ...state.entries,
          { date: "", name: "", amount: 0, categoryName: "" },
        ],
      };
    case "REMOVE_ENTRY":
      return {
        ...state,
        entries: state.entries.filter((_, idx) => idx !== action.index),
      };
    case "RESET":
      return initialState;
    default:
      return state;
  }
}

const AddExpenseAdvanceModal: React.FC<AddExpenseAdvanceModalProps> = ({
  open,
  onClose,
  onSave,
  employeeId,
}) => {
  const [loading, setLoading] = useState(false);
  const { categories, fetchCategories } = useAdvanceCategories();
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<any>({});

  useEffect(() => {
    if (open) {
      fetchCategories();
      if (form.entries.length === 0) {
        dispatch({ type: "ADD_ENTRY" });
      }
    }
  }, [open, fetchCategories, form.entries.length]);

  const validate = (): boolean => {
    const newErrors: { entries?: { [index: number]: any } } = {};
    const entryErrors: { [index: number]: any } = {};

    form.entries.forEach((entry, index) => {
      const errorsForEntry: any = {};
      if (!entry.date) {
        errorsForEntry.date = "Data jest wymagana";
      }

      if (!entry.name) {
        errorsForEntry.name = "Nazwa jest wymagana";
      }

      if (!entry.categoryName) {
        errorsForEntry.categoryName = "Kategoria jest wymagana";
      }
      const amount = Number(entry.amount);
      if (isNaN(amount) || amount <= 0) {
        errorsForEntry.amount = "Kwota musi być większa od 0";
      }

      if (Object.keys(errorsForEntry).length > 0) {
        entryErrors[index] = errorsForEntry;
      }
    });

    if (Object.keys(entryErrors).length > 0) {
      newErrors.entries = entryErrors;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validate()) return;
    setLoading(true);

    const dataToSave: AddExpenseAdvance = {
      type: form.type,
      entries: form.entries.map((e) => ({
        ...e,
        amount: Number(e.amount),
      })),
    };

    await handleApiResponse(
      () => ExpensesAdvancesService.addExpenseAdvance(employeeId, dataToSave),
      () => {
        toast.success("Pomyślnie dodano wpisy");
        onSave();
        close();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania danych"
    );
    setLoading(false);
  };

  const close = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="xl">
      <DialogTitle>Dodaj wpis do ewidencji</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <TextField
            select
            label="Typ wpisu"
            value={form.type}
            onChange={(e) =>
              dispatch({
                type: "SET_FIELD",
                name: "type",
                value: e.target.value,
              })
            }
            sx={{ maxWidth: 300 }}
          >
            <MenuItem value={AdvanceType.Income}>Przychód</MenuItem>
            <MenuItem value={AdvanceType.Expense}>Wydatek</MenuItem>
          </TextField>

          <AdvanceEntriesTable
            entries={form.entries}
            categories={categories.filter((c) => c.type === form.type)}
            errors={errors.entries}
            dispatch={dispatch}
          />

          <Box>
            <Button
              variant="text"
              onClick={() => dispatch({ type: "ADD_ENTRY" })}
            >
              + Dodaj kolejną pozycję
            </Button>
          </Box>
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button
          onClick={close}
          variant="outlined"
          color="inherit"
          disabled={loading}
        >
          Anuluj
        </Button>
        <LoadingButton
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          disabled={loading}
          loading={loading}
          onClick={handleSave}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default AddExpenseAdvanceModal;
