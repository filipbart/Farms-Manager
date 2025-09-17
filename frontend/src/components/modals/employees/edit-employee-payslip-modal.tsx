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
  MenuItem,
} from "@mui/material";
import { toast } from "react-toastify";
import { EmployeePayslipsService } from "../../../services/employee-payslips-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";
import AppDialog from "../../common/app-dialog";
import { MdSave } from "react-icons/md";
import {
  PayrollPeriod,
  type EmployeePayslipListModel,
  type UpdateEmployeePayslip,
} from "../../../models/employees/employees-payslips";
import { useFarms } from "../../../hooks/useFarms";
import { FarmsService } from "../../../services/farms-service";
import type CycleDto from "../../../models/farms/latest-cycle";
import LoadingTextField from "../../common/loading-textfield";

interface EditEmployeePayslipModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  payslipToEdit: EmployeePayslipListModel | null;
}

interface PayslipFormState {
  farmId: string;
  cycleId: string;
  payrollPeriod: PayrollPeriod | "";
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
  farmId?: string;
  cycleId?: string;
  payrollPeriod?: string;
  baseSalary?: string;
  bankTransferAmount?: string;
  bonusAmount?: string;
  overtimePay?: string;
  overtimeHours?: string;
  deductions?: string;
  otherAllowances?: string;
}

const initialState: PayslipFormState = {
  farmId: "",
  cycleId: "",
  payrollPeriod: "",
  baseSalary: "",
  bankTransferAmount: "",
  bonusAmount: "",
  overtimePay: "",
  overtimeHours: "",
  deductions: "",
  otherAllowances: "",
  comment: "",
};

const polishMonthsMap = {
  [PayrollPeriod.January]: "Styczeń",
  [PayrollPeriod.February]: "Luty",
  [PayrollPeriod.March]: "Marzec",
  [PayrollPeriod.April]: "Kwiecień",
  [PayrollPeriod.May]: "Maj",
  [PayrollPeriod.June]: "Czerwiec",
  [PayrollPeriod.July]: "Lipiec",
  [PayrollPeriod.August]: "Sierpień",
  [PayrollPeriod.September]: "Wrzesień",
  [PayrollPeriod.October]: "Październik",
  [PayrollPeriod.November]: "Listopad",
  [PayrollPeriod.December]: "Grudzień",
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
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  useEffect(() => {
    if (payslipToEdit && farms.length > 0) {
      dispatch({
        type: "SET_ALL",
        payload: {
          farmId: payslipToEdit.farmId,
          cycleId: payslipToEdit.cycleId,
          payrollPeriod: payslipToEdit.payrollPeriod,
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
  }, [payslipToEdit, farms]);

  useEffect(() => {
    const fetchCyclesForFarm = async (farmId: string) => {
      if (!farmId) {
        setCycles([]);
        return;
      }
      setLoadingCycles(true);
      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        undefined,
        "Nie udało się pobrać cykli dla wybranej fermy"
      );
      setLoadingCycles(false);
    };

    fetchCyclesForFarm(form.farmId);
  }, [form.farmId]);

  const handleFarmChange = (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    // Reset cycle when farm changes
    dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
    setCycles([]);
  };

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
    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana.";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany.";
    if (!form.payrollPeriod)
      newErrors.payrollPeriod = "Okres rozliczeniowy jest wymagany.";

    (Object.keys(initialState) as Array<keyof PayslipFormState>).forEach(
      (key) => {
        if (key !== "comment" && Number(form[key]) < 0) {
          newErrors[key as keyof PayslipFormErrors] =
            "Wartość nie może być ujemna";
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
      farmId: form.farmId,
      cycleId: form.cycleId,
      payrollPeriod: form.payrollPeriod as PayrollPeriod,
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
    setCycles([]);
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
              <LoadingTextField
                loading={loadingFarms}
                select
                label="Ferma"
                value={form.farmId}
                onChange={(e) => handleFarmChange(e.target.value)}
                error={!!errors.farmId}
                helperText={errors.farmId}
                fullWidth
              >
                {farms.map((farm) => (
                  <MenuItem key={farm.id} value={farm.id}>
                    {farm.name}
                  </MenuItem>
                ))}
              </LoadingTextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <LoadingTextField
                loading={loadingCycles}
                label="Cykl"
                select
                value={form.cycleId}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "cycleId",
                    value: e.target.value,
                  })
                }
                error={!!errors.cycleId}
                helperText={errors.cycleId}
                disabled={!form.farmId || loadingCycles || cycles.length === 0}
                fullWidth
              >
                {cycles.map((cycle) => (
                  <MenuItem key={cycle.id} value={cycle.id}>
                    {`${cycle.identifier}/${cycle.year}`}
                  </MenuItem>
                ))}
              </LoadingTextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                select
                label="Okres rozliczeniowy"
                value={form.payrollPeriod}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "payrollPeriod",
                    value: e.target.value,
                  })
                }
                error={!!errors.payrollPeriod}
                helperText={errors.payrollPeriod}
                fullWidth
              >
                {Object.values(PayrollPeriod).map((period) => (
                  <MenuItem key={period} value={period}>
                    {polishMonthsMap[period]}
                  </MenuItem>
                ))}
              </TextField>
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
