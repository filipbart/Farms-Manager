import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
} from "@mui/material";
import { useEffect, useState, useCallback, useMemo } from "react";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import type { ExpenseContractorRow } from "../../../../models/expenses/expenses-contractors";
import { ExpensesService } from "../../../../services/expenses-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import { useExpensesTypes } from "../../../../hooks/expenses/useExpensesTypes";
import LoadingTextField from "../../../common/loading-textfield";

const INITIAL_FORM_STATE = {
  name: "",
  expenseTypeId: "",
  nip: "",
  address: "",
};
const INITIAL_ERRORS_STATE = {
  name: "",
  expenseTypeId: "",
  nip: "",
  address: "",
};

interface EditExpenseContractorModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  contractorToEdit?: ExpenseContractorRow | null;
}

const EditExpenseContractorModal: React.FC<EditExpenseContractorModalProps> = ({
  open,
  onClose,
  onSave,
  contractorToEdit,
}) => {
  const [loading, setLoading] = useState(false);
  const { expensesTypes, loadingExpensesTypes, fetchExpensesTypes } =
    useExpensesTypes();
  const [form, setForm] = useState(INITIAL_FORM_STATE);
  const [errors, setErrors] = useState(INITIAL_ERRORS_STATE);

  useEffect(() => {
    if (open) {
      fetchExpensesTypes();
    }
  }, [open, fetchExpensesTypes]);

  const expenseTypeMap = useMemo(() => {
    return new Map(
      expensesTypes.map((type) => [
        type.name.trim().toLowerCase(),
        String(type.id),
      ])
    );
  }, [expensesTypes]);

  useEffect(() => {
    if (open && contractorToEdit && expensesTypes.length > 0) {
      const nameToFind =
        contractorToEdit.expenseType?.trim().toLowerCase() || "";
      const foundId = expenseTypeMap.get(nameToFind) || "";

      setForm({
        name: contractorToEdit.name,
        nip: contractorToEdit.nip,
        address: contractorToEdit.address,
        expenseTypeId: foundId,
      });
    }
  }, [contractorToEdit, open, expensesTypes.length, expenseTypeMap]);

  const handleFieldChange = useCallback(
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const { name, value } = event.target;
      setForm((prevForm) => ({ ...prevForm, [name]: value }));
    },
    []
  );

  const validateAndSave = async () => {
    if (!contractorToEdit) return;

    const newErrors = {
      name: !form.name.trim() ? "Nazwa jest wymagana" : "",
      expenseTypeId: !form.expenseTypeId ? "Typ jest wymagany" : "",
      nip: !form.nip.trim() ? "NIP jest wymagany" : "",
      address: !form.address.trim() ? "Adres jest wymagany" : "",
    };

    setErrors(newErrors);

    if (Object.values(newErrors).some((error) => error)) {
      return;
    }

    setLoading(true);
    try {
      const selectedExpenseType = expensesTypes.find(
        (type) => String(type.id) === form.expenseTypeId
      );
      const dataToSave = {
        ...form,
        expenseType: selectedExpenseType?.name || "",
      };

      await handleApiResponse(
        () =>
          ExpensesService.updateExpenseContractor(
            contractorToEdit.id,
            dataToSave
          ),
        () => {
          toast.success("Dane kontrahenta zostały zaktualizowane");
          onSave();
          close();
        },
        undefined,
        "Wystąpił błąd podczas aktualizacji danych"
      );
    } finally {
      setLoading(false);
    }
  };

  const close = useCallback(() => {
    setForm(INITIAL_FORM_STATE);
    setErrors(INITIAL_ERRORS_STATE);
    onClose();
  }, [onClose]);

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj dane kontrahenta</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <TextField
            name="name"
            label="Nazwa"
            value={form.name}
            onChange={handleFieldChange}
            error={!!errors.name}
            helperText={errors.name}
            fullWidth
          />
          <LoadingTextField
            name="expenseTypeId"
            label="Typ wydatku"
            select
            fullWidth
            value={form.expenseTypeId}
            onChange={handleFieldChange}
            loading={loadingExpensesTypes}
            error={!!errors.expenseTypeId}
            helperText={errors.expenseTypeId}
          >
            {expensesTypes.map((type) => (
              <MenuItem key={type.id} value={String(type.id)}>
                {type.name}
              </MenuItem>
            ))}
          </LoadingTextField>
          <TextField
            name="nip"
            label="NIP"
            value={form.nip}
            onChange={handleFieldChange}
            error={!!errors.nip}
            helperText={errors.nip}
            fullWidth
          />
          <TextField
            name="address"
            label="Adres"
            value={form.address}
            onChange={handleFieldChange}
            error={!!errors.address}
            helperText={errors.address}
            fullWidth
          />
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={close} variant="outlined" color="inherit">
          Anuluj
        </Button>
        <LoadingButton
          startIcon={<MdSave />}
          variant="contained"
          color="primary"
          loading={loading}
          onClick={validateAndSave}
        >
          Zapisz zmiany
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default EditExpenseContractorModal;
