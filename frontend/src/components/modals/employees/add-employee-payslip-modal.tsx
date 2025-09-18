import React, { useEffect, useReducer, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  MenuItem,
  Box,
  Typography,
  TextField,
} from "@mui/material";
import { toast } from "react-toastify";
import { useFarmsForPayslips } from "../../../hooks/employees/useFarmsForPayslips";
import { EmployeePayslipsService } from "../../../services/employee-payslips-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingTextField from "../../common/loading-textfield";
import LoadingButton from "../../common/loading-button";
import AppDialog from "../../common/app-dialog";
import { MdSave } from "react-icons/md";
import {
  PayrollPeriod,
  type AddEmployeePayslipData,
  type AddEmployeePayslipEntry,
} from "../../../models/employees/employees-payslips";
import type { EmployeeFarmPayslipModel } from "../../../models/employees/payslips-farms";
import PayslipEntriesTable from "./employee-payslip-entries-table";
import { FarmsService } from "../../../services/farms-service";
import type CycleDto from "../../../models/farms/latest-cycle";
import { getCurrentPayrollPeriod as getPreviousPayrollPeriod } from "../../../utils/payrollPeriod";

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

interface AddEmployeePayslipModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}
interface PayslipFormErrors {
  farmId?: string;
  cycleId?: string;
  payrollPeriod?: string;
  entries?: {
    [index: number]: Partial<Record<keyof AddEmployeePayslipEntry, string>>;
  };
  entriesGeneral?: string;
}

const initialState: AddEmployeePayslipData = {
  farmId: "",
  cycleId: "",
  payrollPeriod: getPreviousPayrollPeriod(),
  entries: [],
};

function formReducer(
  state: AddEmployeePayslipData,
  action: any
): AddEmployeePayslipData {
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
          {
            employeeId: "",
            baseSalary: 0,
            bankTransferAmount: 0,
            bonusAmount: 0,
            overtimePay: 0,
            overtimeHours: 0,
            deductions: 0,
            otherAllowances: 0,
          },
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

const AddEmployeePayslipModal: React.FC<AddEmployeePayslipModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<PayslipFormErrors>({});
  const [loading, setLoading] = useState(false);

  const {
    payslipsFarms,
    loading: loadingFarms,
    fetchPayslipsFarms,
  } = useFarmsForPayslips();

  const [employeesForSelectedFarm, setEmployeesForSelectedFarm] = useState<
    EmployeeFarmPayslipModel[]
  >([]);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  useEffect(() => {
    if (open) {
      // Reset state and fetch initial data when modal opens
      dispatch({ type: "RESET" });
      setErrors({});
      setCycles([]);
      setEmployeesForSelectedFarm([]);
      fetchPayslipsFarms();
    } else {
      dispatch({ type: "RESET" });
    }
  }, [open, fetchPayslipsFarms]);

  useEffect(() => {
    if (open && form.entries.length === 0) {
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open, form.entries.length]);

  const handleFarmChange = (farmId: string) => {
    dispatch({ type: "RESET" });
    setCycles([]);
    setErrors({});

    const selectedFarm = payslipsFarms.find((f) => f.id === farmId);
    if (selectedFarm) {
      dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
      dispatch({
        type: "SET_FIELD",
        name: "cycleId",
        value: selectedFarm.cycle.id,
      });
      setEmployeesForSelectedFarm(selectedFarm.employees);
    }

    dispatch({ type: "ADD_ENTRY" });
  };

  useEffect(() => {
    const fetchCyclesForFarm = async (farmId: string) => {
      setLoadingCycles(true);

      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        undefined,
        "Nie udało się pobrać cykli dla wybranej fermy"
      );

      setLoadingCycles(false);
    };

    if (form.farmId) {
      fetchCyclesForFarm(form.farmId);
    }
  }, [form.farmId]);

  const validateForm = (): boolean => {
    const newErrors: PayslipFormErrors = {};
    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana.";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany.";
    if (!form.payrollPeriod)
      newErrors.payrollPeriod = "Okres rozliczeniowy jest wymagany.";

    if (form.entries.length === 0) {
      newErrors.entriesGeneral =
        "Musisz dodać przynajmniej jednego pracownika.";
    } else {
      const entryErrors: PayslipFormErrors["entries"] = {};
      form.entries.forEach((entry, index) => {
        const currentEntryErrors: Partial<
          Record<keyof AddEmployeePayslipEntry, string>
        > = {};
        if (!entry.employeeId)
          currentEntryErrors.employeeId = "Wybierz pracownika";
        if (Number(entry.baseSalary) <= 0)
          currentEntryErrors.baseSalary = "Wymagane";

        if (Object.keys(currentEntryErrors).length > 0) {
          entryErrors[index] = currentEntryErrors;
        }
      });
      if (Object.keys(entryErrors).length > 0) {
        newErrors.entries = entryErrors;
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;
    setLoading(true);

    const dataToSave = {
      ...form,
      entries: form.entries.map((entry) => ({
        ...entry,
        baseSalary: Number(entry.baseSalary),
        bankTransferAmount: Number(entry.bankTransferAmount),
        bonusAmount: Number(entry.bonusAmount),
        overtimePay: Number(entry.overtimePay),
        overtimeHours: Number(entry.overtimeHours),
        deductions: Number(entry.deductions),
        otherAllowances: Number(entry.otherAllowances),
      })),
    };

    await handleApiResponse(
      () => EmployeePayslipsService.addPayslip(dataToSave),
      () => {
        toast.success("Pomyślnie dodano rozliczenie wypłat.");
        onSave();
        handleClose();
      },
      undefined,
      "Błąd podczas dodawania rozliczenia."
    );
    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    setCycles([]);
    setEmployeesForSelectedFarm([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
      <DialogTitle>Dodaj rozliczenie wypłat</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <Box display="grid" gridTemplateColumns="repeat(3, 1fr)" gap={2}>
            <LoadingTextField
              loading={loadingFarms}
              select
              label="Ferma"
              value={form.farmId}
              onChange={(e) => handleFarmChange(e.target.value)}
              error={!!errors.farmId}
              helperText={errors.farmId}
            >
              {payslipsFarms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>

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
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>

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
            >
              {Object.values(PayrollPeriod).map((period) => (
                <MenuItem key={period} value={period}>
                  {polishMonthsMap[period]}
                </MenuItem>
              ))}
            </TextField>
          </Box>

          {errors.entriesGeneral && (
            <Typography color="error" variant="caption">
              {errors.entriesGeneral}
            </Typography>
          )}

          <PayslipEntriesTable
            entries={form.entries}
            employees={employeesForSelectedFarm}
            errors={errors.entries}
            dispatch={dispatch}
            farmId={form.farmId}
            loadingEmployees={loadingFarms}
          />

          <Button
            variant="outlined"
            onClick={() => dispatch({ type: "ADD_ENTRY" })}
            disabled={!form.farmId}
            sx={{ mt: 2, alignSelf: "flex-start" }}
          >
            Dodaj pracownika
          </Button>
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
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default AddEmployeePayslipModal;
