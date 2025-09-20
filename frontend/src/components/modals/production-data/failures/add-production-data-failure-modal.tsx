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
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import AppDialog from "../../../common/app-dialog";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import { ProductionDataFailuresService } from "../../../../services/production-data/production-data-failures-service";
import type { ProductionDataFailureEntry } from "../../../../models/production-data/failures";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface AddProductionDataFailureModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface FailureFormState {
  farmId: string;
  cycleId: string;
  entries: ProductionDataFailureEntry[];
}

interface FailureFormErrors {
  farmId?: string;
  cycleId?: string;
  entries?: { [index: number]: any };
}

const initialState: FailureFormState = {
  farmId: "",
  cycleId: "",
  entries: [],
};

function formReducer(state: FailureFormState, action: any): FailureFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "SET_ENTRIES":
      return { ...state, entries: action.payload };
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
          { henhouseId: "", deadCount: 0, defectiveCount: 0 },
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

const AddProductionDataFailureModal: React.FC<
  AddProductionDataFailureModalProps
> = ({ open, onClose, onSave }) => {
  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<FailureFormErrors>({});

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

  useEffect(() => {
    const fetchCyclesForFarm = async (farmId: string) => {
      setLoadingCycles(true);

      const selectedFarm = farms.find((f) => f.id === farmId);
      if (selectedFarm?.activeCycle) {
        dispatch({
          type: "SET_FIELD",
          name: "cycleId",
          value: selectedFarm.activeCycle.id,
        });
      } else {
        dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
      }

      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        () => setCycles([]),
        "Nie udało się pobrać listy cykli."
      );
      setLoadingCycles(false);
    };

    if (form.farmId && farms.length > 0) {
      fetchCyclesForFarm(form.farmId);
    }
  }, [form.farmId, farms]);

  const handleFarmChange = async (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    dispatch({
      type: "SET_ENTRIES",
      payload: [{ henhouseId: "", deadCount: 0, defectiveCount: 0 }],
    });
    setErrors({});

    const selectedFarm = farms.find((f) => f.id === farmId);
    setAvailableHenhouses(selectedFarm?.henhouses ?? []);
  };

  const validate = (): boolean => {
    const newErrors: FailureFormErrors = {};
    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany";

    const entryErrors: { [index: number]: any } = {};
    form.entries.forEach((entry, index) => {
      const errorsForEntry: any = {};
      if (!entry.henhouseId) errorsForEntry.henhouseId = "Wymagany";
      if (Number(entry.deadCount) < 0)
        errorsForEntry.deadCount = "Błędna wartość";
      if (Number(entry.defectiveCount) < 0)
        errorsForEntry.defectiveCount = "Błędna wartość";
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
        ProductionDataFailuresService.addFailure({
          farmId: form.farmId,
          cycleId: form.cycleId,
          failureEntries: form.entries.map((e) => ({
            henhouseId: e.henhouseId,
            deadCount: Number(e.deadCount),
            defectiveCount: Number(e.defectiveCount),
          })),
        }),
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
    setAvailableHenhouses([]);
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="md">
      <DialogTitle>Dodaj wpis o upadkach i wybrakowaniach</DialogTitle>
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
                loading={loadingCycles}
                label="Cykl"
                select
                fullWidth
                disabled={!form.farmId || cycles.length === 0}
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

          <Typography variant="h6" sx={{ mt: 1 }}>
            Wpisy dla kurników
          </Typography>
          <TableContainer component={Paper} variant="outlined">
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Kurnik</TableCell>
                  <TableCell align="right">Upadki [szt.]</TableCell>
                  <TableCell align="right">Wybrakowania [szt.]</TableCell>
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
                      <TableCell>
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
                          sx={{ minWidth: 200 }}
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
                          value={entry.deadCount}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "deadCount",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.deadCount}
                          helperText={entryErrors?.deadCount}
                          fullWidth
                          size="small"
                        />
                      </TableCell>
                      <TableCell align="right">
                        <TextField
                          type="number"
                          value={entry.defectiveCount}
                          onChange={(e) =>
                            dispatch({
                              type: "UPDATE_ENTRY",
                              index,
                              name: "defectiveCount",
                              value: e.target.value,
                            })
                          }
                          error={!!entryErrors?.defectiveCount}
                          helperText={entryErrors?.defectiveCount}
                          fullWidth
                          size="small"
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
              + Dodaj kolejny kurnik
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

export default AddProductionDataFailureModal;
