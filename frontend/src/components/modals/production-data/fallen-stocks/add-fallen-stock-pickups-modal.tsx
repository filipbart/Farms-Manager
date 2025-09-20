import React, { useEffect, useReducer, useState, useMemo } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TextField,
  IconButton,
  Grid,
  MenuItem,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { toast } from "react-toastify";
import type { Dayjs } from "dayjs";
import { MdDelete, MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import type { AddFallenStockPickups } from "../../../../models/fallen-stocks/fallen-stock-pickups";
import { FallenStockPickupService } from "../../../../services/production-data/fallen-stock-pickups-service";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface PickupFormState {
  cycleId: string;
  entries: ({ date: Dayjs | null; quantity: number | string } & {
    isNew: boolean;
  })[];
}

interface PickupFormErrors {
  cycleId?: string;
  entries?: { [index: number]: { date?: string; quantity?: string } };
  entriesGeneral?: string;
}

const initialState: PickupFormState = {
  cycleId: "",
  entries: [],
};

function formReducer(state: PickupFormState, action: any): PickupFormState {
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
        entries: [...state.entries, { date: null, quantity: "", isNew: true }],
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

interface AddFallenStockPickupsModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  farmId: string | null | undefined;
}

const AddFallenStockPickupsModal: React.FC<AddFallenStockPickupsModalProps> = ({
  open,
  onClose,
  onSave,
  farmId,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();

  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<PickupFormErrors>({});
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  const selectedFarmName = useMemo(() => {
    if (!farmId || farms.length === 0) return "";
    return farms.find((f) => f.id === farmId)?.name;
  }, [farms, farmId]);

  useEffect(() => {
    if (open && farmId && farms.length > 0) {
      const fetchCyclesForFarm = async () => {
        setLoadingCycles(true);
        setCycles([]);
        dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
        setErrors((prev) => ({ ...prev, cycleId: undefined }));

        const selectedFarm = farms.find((f) => f.id === farmId);

        await handleApiResponse(
          () => FarmsService.getFarmCycles(farmId),
          (data) => {
            const fetchedCycles = data.responseData ?? [];
            setCycles(fetchedCycles);
            if (
              selectedFarm?.activeCycle &&
              fetchedCycles.some((c) => c.id === selectedFarm.activeCycle.id)
            ) {
              dispatch({
                type: "SET_FIELD",
                name: "cycleId",
                value: selectedFarm.activeCycle.id,
              });
            } else if (fetchedCycles.length === 0) {
              setErrors((prev) => ({
                ...prev,
                cycleId: "Brak cykli dla tej fermy",
              }));
            }
          },
          () => setCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingCycles(false);
      };
      fetchCyclesForFarm();
    } else if (!farmId) {
      setCycles([]);
      dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
    }
  }, [open, farmId, farms]);

  useEffect(() => {
    if (open) {
      dispatch({ type: "RESET" });
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open]);

  const validateForm = (): boolean => {
    const newErrors: PickupFormErrors = {};
    if (!farmId) {
      toast.error("Brak wybranej fermy.");
      return false;
    }
    if (!form.cycleId) newErrors.cycleId = "Brak aktywnego cyklu";

    if (form.entries.length === 0) {
      newErrors.entriesGeneral = "Musisz dodać przynajmniej jedną pozycję";
    } else {
      const entryErrors: { [index: number]: any } = {};
      form.entries.forEach((entry, index) => {
        const e: { date?: string; quantity?: string } = {};
        if (!entry.date) e.date = "Wymagane";
        const quantityNum = Number(entry.quantity);
        if (!entry.quantity || isNaN(quantityNum) || quantityNum <= 0)
          e.quantity = "Ilość > 0";
        if (Object.keys(e).length > 0) entryErrors[index] = e;
      });
      if (Object.keys(entryErrors).length > 0) newErrors.entries = entryErrors;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;
    setLoading(true);

    const payload: AddFallenStockPickups = {
      farmId: farmId!,
      cycleId: form.cycleId,
      entries: form.entries.map(({ date, quantity }) => ({
        date: date!.format("YYYY-MM-DD"),
        quantity: Number(quantity),
      })),
    };

    await handleApiResponse(
      () => FallenStockPickupService.addFallenStockPickups(payload),
      () => {
        toast.success("Dodano wpisy odbioru sztuk padłych");
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się dodać wpisów"
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
      <DialogTitle>Nowy odbiór sztuk padłych</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} sx={{ mt: 0.5 }}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={loadingFarms}
              label="Ferma"
              value={selectedFarmName ?? "Ładowanie..."}
              fullWidth
              slotProps={{ input: { readOnly: true } }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <LoadingTextField
              loading={loadingCycles}
              label="Cykl"
              select
              fullWidth
              disabled={!farmId || loadingCycles || cycles.length === 0}
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
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
        </Grid>

        <Typography variant="h6" sx={{ mt: 3, mb: 1 }}>
          Pozycje
        </Typography>
        {errors.entriesGeneral && (
          <Typography color="error" sx={{ mb: 1 }}>
            {errors.entriesGeneral}
          </Typography>
        )}
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ width: "50%" }}>Data odbioru</TableCell>
                <TableCell>Ilość [szt.]</TableCell>
                <TableCell align="center">Akcje</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {form.entries.map((entry, index) => (
                <TableRow key={index}>
                  <TableCell>
                    <DatePicker
                      value={entry.date}
                      onChange={(value) =>
                        dispatch({
                          type: "UPDATE_ENTRY",
                          index,
                          name: "date",
                          value,
                        })
                      }
                      disableFuture
                      format="DD.MM.YYYY"
                      slotProps={{
                        textField: {
                          fullWidth: true,
                          size: "small",
                          error: !!errors.entries?.[index]?.date,
                          helperText: errors.entries?.[index]?.date,
                        },
                      }}
                    />
                  </TableCell>
                  <TableCell>
                    <TextField
                      type="number"
                      value={entry.quantity}
                      onChange={(e) =>
                        dispatch({
                          type: "UPDATE_ENTRY",
                          index,
                          name: "quantity",
                          value: e.target.value,
                        })
                      }
                      fullWidth
                      size="small"
                      error={!!errors.entries?.[index]?.quantity}
                      helperText={errors.entries?.[index]?.quantity}
                      InputProps={{ inputProps: { min: 1 } }}
                    />
                  </TableCell>
                  <TableCell align="center">
                    <IconButton
                      color="error"
                      onClick={() => dispatch({ type: "REMOVE_ENTRY", index })}
                    >
                      <MdDelete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        <Button
          variant="outlined"
          sx={{ mt: 2 }}
          onClick={() => dispatch({ type: "ADD_ENTRY" })}
        >
          Dodaj kolejną pozycję
        </Button>
      </DialogContent>
      <DialogActions>
        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default AddFallenStockPickupsModal;
