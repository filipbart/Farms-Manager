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
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import type {
  AddExpenseContractorData,
  ExpenseContractorRow,
} from "../../../../models/expenses/expenses-contractors";
import { ExpensesService } from "../../../../services/expenses-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import { useExpensesTypes } from "../../../../hooks/expenses/useExpensesTypes";
import LoadingTextField from "../../../common/loading-textfield";

interface EditExpenseContractorModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  contractorToEdit?: ExpenseContractorRow;
}

const EditExpenseContractorModal: React.FC<EditExpenseContractorModalProps> = ({
  open,
  onClose,
  onSave,
  contractorToEdit,
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<AddExpenseContractorData>();

  const [loading, setLoading] = useState(false);
  const { expensesTypes, loadingExpensesTypes, fetchExpensesTypes } =
    useExpensesTypes();

  useEffect(() => {
    if (contractorToEdit && open) {
      reset(contractorToEdit);
    } else if (!open) {
      reset();
    }
  }, [contractorToEdit, open, reset]);

  useEffect(() => {
    fetchExpensesTypes();
  }, [fetchExpensesTypes]);

  const handleSave = async (data: AddExpenseContractorData) => {
    if (loading || !contractorToEdit) return;

    setLoading(true);
    await handleApiResponse(
      () => ExpensesService.updateExpenseContractor(contractorToEdit.id, data),
      () => {
        toast.success("Dane kontrahenta zostały zaktualizowane");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji danych"
    );

    setLoading(false);
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Edytuj dane kontrahenta</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Nazwa"
              error={!!errors?.name}
              helperText={errors.name ? (errors.name.message as string) : ""}
              {...register("name", { required: "Nazwa jest wymagana" })}
              fullWidth
            />

            <LoadingTextField
              label="Typ wydatku"
              select
              fullWidth
              loading={loadingExpensesTypes}
              value={watch("expenseTypeId") || ""}
              error={!!errors.expenseTypeId}
              helperText={errors.expenseTypeId?.message}
              {...register("expenseTypeId", { required: "Typ jest wymagany" })}
            >
              {expensesTypes.map((expenseType) => (
                <MenuItem key={expenseType.id} value={expenseType.id}>
                  {expenseType.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <TextField
              label="NIP"
              error={!!errors?.nip}
              helperText={errors.nip ? (errors.nip.message as string) : ""}
              {...register("nip", { required: "NIP jest wymagany" })}
              fullWidth
            />

            <TextField
              label="Adres"
              error={!!errors?.address}
              helperText={
                errors.address ? (errors.address.message as string) : ""
              }
              {...register("address", { required: "Adres jest wymagany" })}
              fullWidth
            />
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
            startIcon={<MdSave />}
            type="submit"
            variant="contained"
            color="primary"
            disabled={loading}
            loading={loading}
          >
            Zapisz zmiany
          </LoadingButton>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default EditExpenseContractorModal;
