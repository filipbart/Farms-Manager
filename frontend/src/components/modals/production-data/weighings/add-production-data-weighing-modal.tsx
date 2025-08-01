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
import { useHatcheries } from "../../../../hooks/useHatcheries";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import AppDialog from "../../../common/app-dialog";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import { ProductionDataWeighingsService } from "../../../../services/production-data/production-data-weighings-service";
import type { WeighingDataEntry } from "../../../../models/production-data/weighings";

interface AddProductionDataWeighingModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface WeighingFormState {
  farmId: string;
  cycleId: string;
  cycleDisplay: string;
  weighingNumber: number | "";
  entries: WeighingDataEntry[];
}

interface WeighingFormErrors {
  farmId?: string;
  cycleId?: string;
  weighingNumber?: string;
  entries?: { [index: number]: any };
}

const initialState: WeighingFormState = {
  farmId: "",
  cycleId: "",
  cycleDisplay: "",
  weighingNumber: "",
  entries: [],
};

function formReducer(state: WeighingFormState, action: any): WeighingFormState {
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
          { henhouseId: "", hatcheryId: "", day: 0, weight: 0 },
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

const AddProductionDataWeighingModal: React.FC<
  AddProductionDataWeighingModalProps
> = ({ open, onClose, onSave }) => {
  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { hatcheries, loadingHatcheries, fetchHatcheries } = useHatcheries();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<WeighingFormErrors>({});

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchHatcheries();
    }
  }, [open, fetchFarms, fetchHatcheries]);

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
      value: [{ henhouseId: "", hatcheryId: "", day: 0, weight: 0 }],
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
    const newErrors: WeighingFormErrors = {};
    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany";
    if (!form.weighingNumber)
      newErrors.weighingNumber = "Numer ważenia jest wymagany";

    const entryErrors: { [index: number]: any } = {};
    form.entries.forEach((entry, index) => {
      const errorsForEntry: any = {};
      if (!entry.henhouseId) errorsForEntry.henhouseId = "Wymagany";
      if (!entry.hatcheryId) errorsForEntry.hatcheryId = "Wymagana";
      if (Number(entry.day) <= 0) errorsForEntry.day = "Błędna wartość";
      if (Number(entry.weight) <= 0) errorsForEntry.weight = "Błędna wartość";
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
        ProductionDataWeighingsService.addWeighing({
          farmId: form.farmId,
          cycleId: form.cycleId,
          weighingNumber: Number(form.weighingNumber),
          entries: form.entries.map((e) => ({
            ...e,
            day: Number(e.day),
            weight: Number(e.weight),
          })),
        }),
      () => {
        toast.success("Pomyślnie dodano wpisy ważenia");
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
    <AppDialog open={open} onClose={close} fullWidth maxWidth="lg">
      <DialogTitle>Dodaj kolejne ważenie</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 4 }}>
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
            <Grid size={{ xs: 12, sm: 4 }}>
              <LoadingTextField
                loading={loadingCycle}
                label="Cykl"
                value={form.cycleDisplay}
                slotProps={{ input: { readOnly: true } }}
                error={!!errors.cycleId}
                helperText={errors.cycleId}
                fullWidth
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField
                select
                label="Numer ważenia"
                value={form.weighingNumber}
                onChange={(e) =>
                  dispatch({
                    type: "SET_FIELD",
                    name: "weighingNumber",
                    value: e.target.value,
                  })
                }
                error={!!errors.weighingNumber}
                helperText={errors.weighingNumber}
                fullWidth
              >
                {[2, 3, 4, 5].map((num) => (
                  <MenuItem key={num} value={num}>
                    Ważenie {num}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
          </Grid>

          <Typography variant="h6" sx={{ mt: 1 }}>
            Pozycje ważenia
          </Typography>
          <TableContainer component={Paper} variant="outlined">
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Kurnik</TableCell>
                  <TableCell>Wylęgarnia</TableCell>
                  <TableCell align="right">Doba</TableCell>
                  <TableCell align="right">Śr. masa ciała [g]</TableCell>
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
                      <TableCell sx={{ minWidth: 250 }}>
                        <LoadingTextField
                          loading={loadingHatcheries}
                          select
                          value={entry.hatcheryId}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "hatcheryId",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.hatcheryId}
                          helperText={entryErrors?.hatcheryId}
                          fullWidth
                          size="small"
                        >
                          {hatcheries.map((h) => (
                            <MenuItem key={h.id} value={h.id}>
                              {h.name}
                            </MenuItem>
                          ))}
                        </LoadingTextField>
                      </TableCell>
                      <TableCell align="right">
                        <TextField
                          type="number"
                          value={entry.day}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "day",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.day}
                          helperText={entryErrors?.day}
                          size="small"
                          sx={{ width: 100 }}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <TextField
                          type="number"
                          value={entry.weight}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "weight",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.weight}
                          helperText={entryErrors?.weight}
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

export default AddProductionDataWeighingModal;
