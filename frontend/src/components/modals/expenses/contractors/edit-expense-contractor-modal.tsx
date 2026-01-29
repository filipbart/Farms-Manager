import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  Autocomplete,
  Chip,
} from "@mui/material";
import { useEffect, useState, useCallback } from "react";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import type { ExpenseContractorRow } from "../../../../models/expenses/expenses-contractors";
import { ExpensesService } from "../../../../services/expenses-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import { useExpensesTypes } from "../../../../hooks/expenses/useExpensesTypes";
import AppDialog from "../../../common/app-dialog";
import { isValidNip } from "../../../../utils/validation";

const INITIAL_FORM_STATE = {
  name: "",
  expenseTypeIds: [] as string[],
  nip: "",
  address: "",
};
const INITIAL_ERRORS_STATE = {
  name: "",
  expenseTypeIds: "",
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

  useEffect(() => {
    if (open && contractorToEdit) {
      setForm({
        name: contractorToEdit.name,
        nip: contractorToEdit.nip,
        address: contractorToEdit.address,
        expenseTypeIds: contractorToEdit.expenseTypes?.map((t) => t.id) || [],
      });
    }
  }, [contractorToEdit, open]);

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
      expenseTypeIds:
        form.expenseTypeIds.length === 0
          ? "Wymagany jest co najmniej jeden typ"
          : "",
      nip: !form.nip.trim()
        ? "NIP jest wymagany"
        : isValidNip(form.nip)
        ? ""
        : "NIP jest nieprawidłowy",
      address: !form.address.trim() ? "Adres jest wymagany" : "",
    };

    setErrors(newErrors);

    if (Object.values(newErrors).some((error) => error)) {
      return;
    }

    setLoading(true);
    try {
      const dataToSave = {
        name: form.name,
        nip: form.nip,
        address: form.address,
        expenseTypeIds: form.expenseTypeIds,
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
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
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
          <Autocomplete
            multiple
            options={expensesTypes}
            getOptionLabel={(option) => option.name}
            loading={loadingExpensesTypes}
            value={expensesTypes.filter((t) =>
              form.expenseTypeIds.includes(t.id)
            )}
            onChange={(_, newValue) => {
              setForm((prev) => ({
                ...prev,
                expenseTypeIds: newValue.map((v) => v.id),
              }));
            }}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip
                  label={option.name}
                  {...getTagProps({ index })}
                  key={option.id}
                />
              ))
            }
            renderInput={(params) => (
              <TextField
                {...params}
                label="Typy wydatków"
                error={!!errors.expenseTypeIds}
                helperText={errors.expenseTypeIds}
              />
            )}
          />
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
    </AppDialog>
  );
};

export default EditExpenseContractorModal;
