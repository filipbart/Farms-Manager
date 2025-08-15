import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
  Typography,
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
  Grid,
} from "@mui/material";
import { useEffect, useReducer, useState } from "react";
import { toast } from "react-toastify";
import { MdDelete, MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import AppDialog from "../../../common/app-dialog";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import { ProductionDataFlockLossService } from "../../../../services/production-data/flock-loss-measures-service";

interface AddProductionDataFlockLossModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface FlockLossMeasureEntry {
  henhouseId: string;
  quantity: number | "";
}

interface FlockLossMeasureFormState {
  farmId: string;
  cycleId: string;
  cycleDisplay: string;
  measureNumber: number | "";
  day: number | "";
  entries: FlockLossMeasureEntry[];
}

interface FlockLossMeasureFormErrors {
  farmId?: string;
  cycleId?: string;
  measureNumber?: string;
  day?: string;
  entries?: { [index: number]: { henhouseId?: string; quantity?: string } };
}

const initialState: FlockLossMeasureFormState = {
  farmId: "",
  cycleId: "",
  cycleDisplay: "",
  measureNumber: "",
  day: "",
  entries: [],
};

function formReducer(
  state: FlockLossMeasureFormState,
  action: any
): FlockLossMeasureFormState {
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
        entries: [...state.entries, { henhouseId: "", quantity: "" }],
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

const AddProductionDataFlockLossModal: React.FC<
  AddProductionDataFlockLossModalProps
> = ({ open, onClose, onSave }) => {
  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<FlockLossMeasureFormErrors>({});

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  useEffect(() => {
    if (open && form.entries.length === 0) {
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open, form.entries.length]);

  const handleFarmChange = async (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
    dispatch({ type: "SET_FIELD", name: "cycleDisplay", value: "" });
    dispatch({
      type: "SET_FIELD",
      name: "entries",
      value: [{ henhouseId: "", quantity: "" }],
    });
    setErrors({});

    const selectedFarm = farms.find((f) => f.id === farmId);
    setAvailableHenhouses(selectedFarm?.henhouses ?? []);

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      dispatch({ type: "SET_FIELD", name: "cycleId", value: cycle.id });
      dispatch({
        type: "SET_FIELD",
        name: "cycleDisplay",
        value: `${cycle.identifier}/${cycle.year}`,
      });
    } else {
      setErrors((prev) => ({ ...prev, cycleId: "Brak aktywnego cyklu" }));
    }
  };

  const validate = (): boolean => {
    const newErrors: FlockLossMeasureFormErrors = {};
    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany";
    if (!form.measureNumber)
      newErrors.measureNumber = "Numer pomiaru jest wymagany";
    if (Number(form.day) <= 0) newErrors.day = "Doba jest wymagana";

    const entryErrors: { [index: number]: any } = {};
    form.entries.forEach((entry, index) => {
      const errorsForEntry: any = {};
      if (!entry.henhouseId) errorsForEntry.henhouseId = "Wymagany";
      if (Number(entry.quantity) <= 0)
        errorsForEntry.quantity = "Błędna wartość";
      if (Object.keys(errorsForEntry).length > 0) {
        entryErrors[index] = errorsForEntry;
      }
    });

    if (Object.keys(entryErrors).length > 0) newErrors.entries = entryErrors;

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validate()) return;
    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataFlockLossService.addFlockLossMeasure({
          farmId: form.farmId,
          cycleId: form.cycleId,
          measureNumber: Number(form.measureNumber),
          day: Number(form.day),
          entries: form.entries.map((e) => ({
            henhouseId: e.henhouseId,
            quantity: Number(e.quantity),
          })),
        }),
      () => {
        toast.success("Pomyślnie dodano wpisy pomiaru");
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
    setAvailableHenhouses([]);
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="md">
      <DialogTitle>Dodaj pomiar upadków i wybrakowań</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <LoadingTextField
                loading={loadingFarms}
                select
                label="Ferma"
                fullWidth
                value={form.farmId}
                error={!!errors.farmId}
                helperText={errors.farmId}
                onChange={(e) => handleFarmChange(e.target.value)}
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
                loading={loadingCycle}
                label="Cykl"
                value={form.cycleDisplay}
                InputProps={{ readOnly: true }}
                error={!!errors.cycleId}
                helperText={errors.cycleId}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                select
                label="Numer pomiaru"
                value={form.measureNumber}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "measureNumber",
                    value: e.target.value,
                  })
                }
                error={!!errors.measureNumber}
                helperText={errors.measureNumber}
                fullWidth
              >
                {[1, 2, 3, 4].map((num) => (
                  <MenuItem key={num} value={num}>
                    Pomiar {num}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                type="number"
                label="Doba"
                value={form.day}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "day",
                    value: e.target.value,
                  })
                }
                error={!!errors.day}
                helperText={errors.day}
                fullWidth
              />
            </Grid>
          </Grid>

          <Typography variant="h6" sx={{ mt: 1 }}>
            Pozycje pomiaru
          </Typography>
          <TableContainer component={Paper} variant="outlined">
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Kurnik</TableCell>
                  <TableCell align="right">
                    Upadki i wybrakowania [szt.]
                  </TableCell>
                  <TableCell align="center" sx={{ width: "60px" }}>
                    Akcje
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {form.entries.map((entry, index) => {
                  const entryErrors = errors.entries?.[index];
                  const usedHenhouses = form.entries.map((e) => e.henhouseId);
                  return (
                    <TableRow key={index}>
                      <TableCell sx={{ minWidth: 250 }}>
                        <TextField
                          select
                          value={entry.henhouseId}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "henhouseId",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.henhouseId}
                          helperText={entryErrors?.henhouseId}
                          fullWidth
                          size="small"
                          disabled={availableHenhouses.length === 0}
                        >
                          {availableHenhouses
                            .filter(
                              (h) =>
                                h.id === entry.henhouseId ||
                                !usedHenhouses.includes(h.id)
                            )
                            .map((h) => (
                              <MenuItem key={h.id} value={h.id}>
                                {h.name}
                              </MenuItem>
                            ))}
                        </TextField>
                      </TableCell>
                      <TableCell align="right">
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
                          error={!!entryErrors?.quantity}
                          helperText={entryErrors?.quantity}
                          size="small"
                          sx={{ width: 120 }}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <IconButton
                          color="error"
                          onClick={() =>
                            dispatch({ type: "REMOVE_ENTRY", index })
                          }
                        >
                          <MdDelete />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </TableContainer>
          <Box>
            <Button
              variant="text"
              onClick={() => dispatch({ type: "ADD_ENTRY" })}
              disabled={!form.farmId}
            >
              + Dodaj pozycję
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

export default AddProductionDataFlockLossModal;
