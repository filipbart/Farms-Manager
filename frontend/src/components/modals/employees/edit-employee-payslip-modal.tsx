import React, { useEffect, useReducer, useState, useMemo } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
  Grid,
  Typography,
  Divider,
} from "@mui/material";
import { toast } from "react-toastify";
import { EmployeePayslipsService } from "../../../services/employee-payslips-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";
import AppDialog from "../../common/app-dialog";
import { MdSave } from "react-icons/md";
import type {
  EmployeePayslipListModel,
  UpdateEmployeePayslip,
} from "../../../models/employees/employees-payslips";

interface EditEmployeePayslipModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  payslipToEdit: EmployeePayslipListModel | null;
}

interface PayslipFormState {
  baseSalary: string | number;
  bankTransferAmount: string | number;
  bonusAmount: string | number;
  overtimePay: string | number;
  overtimeHours: string | number;
  deductions: string | number;
  otherAllowances: string | number;
  comment: string;
}

interface PayslipFormErrors {
  baseSalary?: string;
  bankTransferAmount?: string;
  bonusAmount?: string;
  overtimePay?: string;
  overtimeHours?: string;
  deductions?: string;
  otherAllowances?: string;
}

const initialState: PayslipFormState = {
  baseSalary: "",
  bankTransferAmount: "",
  bonusAmount: "",
  overtimePay: "",
  overtimeHours: "",
  deductions: "",
  otherAllowances: "",
  comment: "",
};

function formReducer(state: PayslipFormState, action: any): PayslipFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "SET_ALL":
      return { ...state, ...action.payload };
    case "RESET":
      return initialState;
    default:
      return state;
  }
}

const EditEmployeePayslipModal: React.FC<EditEmployeePayslipModalProps> = ({
  open,
  onClose,
  onSave,
  payslipToEdit,
}) => {
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<PayslipFormErrors>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (payslipToEdit) {
      dispatch({
        type: "SET_ALL",
        payload: {
          baseSalary: payslipToEdit.baseSalary,
          bankTransferAmount: payslipToEdit.bankTransferAmount,
          bonusAmount: payslipToEdit.bonusAmount,
          overtimePay: payslipToEdit.overtimePay,
          overtimeHours: payslipToEdit.overtimeHours,
          deductions: payslipToEdit.deductions,
          otherAllowances: payslipToEdit.otherAllowances,
          comment: payslipToEdit.comment || "",
        },
      });
    }
  }, [payslipToEdit]);

  const netPay = useMemo(() => {
    const baseSalary = Number(form.baseSalary) || 0;
    const bankTransferAmount = Number(form.bankTransferAmount) || 0;
    const bonusAmount = Number(form.bonusAmount) || 0;
    const overtimePay = Number(form.overtimePay) || 0;
    const otherAllowances = Number(form.otherAllowances) || 0;
    const deductions = Number(form.deductions) || 0;

    const result =
      baseSalary -
      bankTransferAmount +
      bonusAmount +
      overtimePay -
      deductions +
      otherAllowances;
    return result.toFixed(2);
  }, [form]);

  const validateForm = (): boolean => {
    const newErrors: PayslipFormErrors = {};
    (Object.keys(initialState) as Array<keyof PayslipFormState>).forEach(
      (key) => {
        if (key !== "comment" && Number(form[key]) < 0) {
          newErrors[key] = "Wartość nie może być ujemna";
        }
      }
    );

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm() || !payslipToEdit) return;

    setLoading(true);

    const dataToSave: UpdateEmployeePayslip = {
      baseSalary: Number(form.baseSalary),
      bankTransferAmount: Number(form.bankTransferAmount),
      bonusAmount: Number(form.bonusAmount),
      overtimePay: Number(form.overtimePay),
      overtimeHours: Number(form.overtimeHours),
      deductions: Number(form.deductions),
      otherAllowances: Number(form.otherAllowances),
      comment: form.comment,
    };

    await handleApiResponse(
      () => EmployeePayslipsService.updatePayslip(payslipToEdit.id, dataToSave),
      () => {
        toast.success("Wpis wypłaty został zaktualizowany");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się zaktualizować wpisu"
    );
    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="md">
      <DialogTitle>Edytuj wpis wypłaty</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Pracownik"
                value={payslipToEdit?.employeeFullName || ""}
                fullWidth
                disabled
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Ferma"
                value={payslipToEdit?.farmName || ""}
                fullWidth
                disabled
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Cykl"
                value={payslipToEdit?.cycleText || ""}
                fullWidth
                disabled
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Okres rozliczeniowy"
                value={payslipToEdit?.payrollPeriodDesc || ""}
                fullWidth
                disabled
              />
            </Grid>
          </Grid>

          <Divider sx={{ my: 1 }} />

          <Typography variant="h6" gutterBottom>
            Składniki wynagrodzenia
          </Typography>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Pensja podstawowa [zł]"
                type="number"
                value={form.baseSalary}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "baseSalary",
                    value: e.target.value,
                  })
                }
                error={!!errors.baseSalary}
                helperText={errors.baseSalary}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Kwota na konto [zł]"
                type="number"
                value={form.bankTransferAmount}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "bankTransferAmount",
                    value: e.target.value,
                  })
                }
                error={!!errors.bankTransferAmount}
                helperText={errors.bankTransferAmount}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Premia [zł]"
                type="number"
                value={form.bonusAmount}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "bonusAmount",
                    value: e.target.value,
                  })
                }
                error={!!errors.bonusAmount}
                helperText={errors.bonusAmount}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Nadgodziny [zł]"
                type="number"
                value={form.overtimePay}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "overtimePay",
                    value: e.target.value,
                  })
                }
                error={!!errors.overtimePay}
                helperText={errors.overtimePay}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Nadgodziny [h]"
                type="number"
                value={form.overtimeHours}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "overtimeHours",
                    value: e.target.value,
                  })
                }
                error={!!errors.overtimeHours}
                helperText={errors.overtimeHours}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Potrącenia [zł]"
                type="number"
                value={form.deductions}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "deductions",
                    value: e.target.value,
                  })
                }
                error={!!errors.deductions}
                helperText={errors.deductions}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Inne dodatki [zł]"
                type="number"
                value={form.otherAllowances}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "otherAllowances",
                    value: e.target.value,
                  })
                }
                error={!!errors.otherAllowances}
                helperText={errors.otherAllowances}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <TextField
                label="Do wypłaty [zł]"
                value={netPay}
                fullWidth
                slotProps={{
                  htmlInput: { readOnly: true, sx: { fontWeight: "bold" } },
                }}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Komentarz (opcjonalnie)"
                value={form.comment}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "comment",
                    value: e.target.value,
                  })
                }
                multiline
                rows={3}
                fullWidth
              />
            </Grid>
          </Grid>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          startIcon={<MdSave />}
        >
          Zapisz zmiany
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditEmployeePayslipModal;
