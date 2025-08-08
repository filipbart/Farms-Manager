import React, { useEffect, useReducer, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  MenuItem,
  Box,
  Typography,
  Checkbox,
  FormControlLabel,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { toast } from "react-toastify";
import type { Dayjs } from "dayjs";
import { MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { useUtilizationPlants } from "../../../../hooks/useUtilizationPlants";
import type { FallenStockEntry } from "../../../../models/fallen-stocks/fallen-stocks";
import { FallenStockService } from "../../../../services/production-data/fallen-stocks-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import FallenStockEntriesTable from "./fallen-stocks-entry-table";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";

interface AddFallenStocksModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface FallenStockFormState {
  farmId: string;
  cycleId: string;
  cycleDisplay: string;
  utilizationPlantId: string;
  date: Dayjs | null;
  entries: (FallenStockEntry & { isEditing: boolean })[];
}

interface FallenStockFormErrors {
  farmId?: string;
  cycleId?: string;
  utilizationPlantId?: string;
  date?: string;
  entries?: { [index: number]: { henhouseId?: string; quantity?: string } };
  entriesGeneral?: string;
}

const initialState: FallenStockFormState = {
  farmId: "",
  cycleId: "",
  cycleDisplay: "",
  utilizationPlantId: "",
  date: null,
  entries: [],
};

function formReducer(
  state: FallenStockFormState,
  action: any
): FallenStockFormState {
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
          { henhouseId: "", quantity: 0, isEditing: true },
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

const AddFallenStocksModal: React.FC<AddFallenStocksModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const {
    utilizationPlants,
    loadingUtilizationPlants,
    fetchUtilizationPlants,
  } = useUtilizationPlants();
  const { loadLatestCycle, loadingCycle: loadingLatestCycle } =
    useLatestCycle();

  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<FallenStockFormErrors>({});
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [sendToIrz, setSendToIrz] = useState(false); // ZMIANA

  useEffect(() => {
    fetchFarms();
    fetchUtilizationPlants();
  }, [fetchFarms, fetchUtilizationPlants]);

  useEffect(() => {
    if (open) {
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open]);

  const handleFarmChange = async (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    dispatch({ type: "SET_FIELD", name: "cycleId", value: "" });
    dispatch({ type: "SET_FIELD", name: "cycleDisplay", value: "" });
    setErrors({});

    const selectedFarm = farms.find((f) => f.id === farmId);
    setHenhouses(selectedFarm?.henhouses ?? []);

    const cycle = await loadLatestCycle(farmId);
    if (!cycle) {
      setErrors((prev) => ({ ...prev, cycleId: "Brak aktywnego cyklu" }));
      return;
    }

    dispatch({ type: "SET_FIELD", name: "cycleId", value: cycle.id });
    dispatch({
      type: "SET_FIELD",
      name: "cycleDisplay",
      value: `${cycle.identifier}/${cycle.year}`,
    });
  };

  const validateEntry = (entry: FallenStockEntry) => {
    const e: { henhouseId?: string; quantity?: string } = {};
    if (!entry.henhouseId) e.henhouseId = "Kurnik jest wymagany";
    const quantityNum = Number(entry.quantity);
    if (!entry.quantity || isNaN(quantityNum) || quantityNum <= 0) {
      e.quantity = "Ilość musi być > 0";
    }
    return e;
  };

  const validateForm = (): boolean => {
    const newErrors: FallenStockFormErrors = {};

    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.cycleId) newErrors.cycleId = "Brak aktywnego cyklu";
    if (!form.utilizationPlantId)
      newErrors.utilizationPlantId = "Zakład jest wymagany";
    if (!form.date) newErrors.date = "Data jest wymagana";

    if (form.entries.length === 0) {
      newErrors.entriesGeneral = "Musisz dodać przynajmniej jedną pozycję";
    } else {
      const entryErrors: { [index: number]: any } = {};
      form.entries.forEach((entry, index) => {
        const e = validateEntry(entry);
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

    await handleApiResponse(
      () =>
        FallenStockService.addNewFallenStock({
          farmId: form.farmId,
          cycleId: form.cycleId,
          utilizationPlantId: form.utilizationPlantId,
          date: form.date!.format("YYYY-MM-DD"),
          entries: form.entries.map(({ henhouseId, quantity }) => ({
            henhouseId,
            quantity: Number(quantity),
          })),
        }),
      async (data) => {
        if (sendToIrz) {
          await handleApiResponse(
            () =>
              FallenStockService.sendToIrzPlus({
                internalGroupId: data.responseData!.internalGroupId,
              }),
            () => toast.success("Zgłoszenie wysłane do IRZplus"),
            undefined,
            "Nie udało się wysłać zgłoszenia do IRZplus"
          );
        }

        toast.success("Dodano zgłoszenie sztuk padłych");
        dispatch({ type: "RESET" });
        setErrors({});
        setSendToIrz(false);
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się dodać zgłoszenia"
    );

    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    setSendToIrz(false);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="lg">
      <DialogTitle>Nowe zgłoszenie sztuk padłych</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          {/* Pola formularza (Ferma, Cykl, Zakład, Data) bez zmian */}
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
          <LoadingTextField
            loading={loadingLatestCycle}
            label="Cykl"
            value={form.cycleDisplay}
            slotProps={{ htmlInput: { readOnly: true } }}
            error={!!errors.cycleId}
            helperText={errors.cycleId}
            fullWidth
          />
          <LoadingTextField
            loading={loadingUtilizationPlants}
            select
            label="Zakład utylizacyjny"
            value={form.utilizationPlantId}
            onChange={(e) =>
              dispatch({
                type: "SET_FIELD",
                name: "utilizationPlantId",
                value: e.target.value,
              })
            }
            error={!!errors.utilizationPlantId}
            helperText={errors.utilizationPlantId}
            fullWidth
          >
            {utilizationPlants.map((plant) => (
              <MenuItem key={plant.id} value={plant.id}>
                {plant.name}
              </MenuItem>
            ))}
          </LoadingTextField>
          <DatePicker
            label="Data zgłoszenia"
            value={form.date}
            onChange={(value) => {
              dispatch({ type: "SET_FIELD", name: "date", value });
              setErrors((prev) => ({ ...prev, date: undefined }));
            }}
            disableFuture
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                error: !!errors.date,
                helperText: errors.date,
                fullWidth: true,
              },
            }}
          />

          {errors.entriesGeneral && (
            <Box sx={{ mb: 1 }}>
              <Typography color="error">{errors.entriesGeneral}</Typography>
            </Box>
          )}

          <FallenStockEntriesTable
            entries={form.entries}
            henhouses={henhouses}
            errors={errors.entries}
            dispatch={dispatch}
            farmId={form.farmId}
          />

          <Button
            variant="outlined"
            onClick={() => dispatch({ type: "ADD_ENTRY" })}
          >
            Dodaj pozycję
          </Button>
        </Box>
      </DialogContent>
      <DialogActions>
        <FormControlLabel
          control={
            <Checkbox
              checked={sendToIrz}
              onChange={() => setSendToIrz(!sendToIrz)}
              color="error"
            />
          }
          label={
            <Typography sx={{ color: "error.main" }}>
              Wyślij do IRZplus
            </Typography>
          }
        />
        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          loadingSize={20}
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

export default AddFallenStocksModal;
