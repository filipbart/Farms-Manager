import React, { useEffect, useReducer, useState } from "react";
import {
  Dialog,
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
import { useFarms } from "../../../hooks/useFarms";
import { useHatcheries } from "../../../hooks/useHatcheries";
import { InsertionsService } from "../../../services/insertions-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingTextField from "../../common/loading-textfield";
import LoadingButton from "../../common/loading-button";
import type { Dayjs } from "dayjs";
import type { HouseRowModel } from "../../../models/farms/house-row-model";
import InsertionEntriesTable from "./insertions-entry-table";
import type {
  InsertionEntry,
  InsertionEntryErrors,
} from "../../../models/insertions/insertion-entry";
import { MdSave } from "react-icons/md";
import { useLatestCycle } from "../../../hooks/useLatestCycle";

interface AddInsertionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface InsertionFormState {
  farmId: string;
  identifierId: string;
  identifierDisplay: string;
  insertionDate: Dayjs | null;
  entries: InsertionEntry[];
}

interface InsertionFormErrors {
  farmId?: string;
  identifierId?: string;
  insertionDate?: string;
  entries?: { [index: number]: InsertionEntryErrors };
  entriesGeneral?: string;
}

const initialState: InsertionFormState = {
  farmId: "",
  identifierId: "",
  identifierDisplay: "",
  insertionDate: null,
  entries: [],
};

function formReducer(
  state: InsertionFormState,
  action: any
): InsertionFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "UPDATE_ENTRY": {
      const updatedEntries = [...state.entries];
      updatedEntries[action.index] = {
        ...updatedEntries[action.index],
        [action.name]: action.value,
      };
      return {
        ...state,
        entries: updatedEntries,
      };
    }
    case "ADD_ENTRY":
      return {
        ...state,
        entries: [
          ...state.entries,
          {
            henhouseId: "",
            hatcheryId: "",
            quantity: "",
            bodyWeight: "",
            isEditing: true,
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

const AddInsertionModal: React.FC<AddInsertionModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { hatcheries, loadingHatcheries, fetchHatcheries } = useHatcheries();
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [loadingHenhouses, setLoadingHenhouses] = useState(false);
  const { loadLatestCycle, loadingCycle: loadingLatestCycle } =
    useLatestCycle();
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<InsertionFormErrors>({});
  const [sendToIrz, setSendToIrz] = useState(false);

  useEffect(() => {
    fetchFarms();
    fetchHatcheries();
  }, []);

  useEffect(() => {
    if (open) {
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open]);

  const handleFarmChange = async (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    dispatch({ type: "SET_FIELD", name: "identifierId", value: "" });
    dispatch({ type: "SET_FIELD", name: "identifierDisplay", value: "" });
    setHenhouses([]);
    setErrors({});

    setLoadingHenhouses(true);
    await handleApiResponse(
      () => InsertionsService.getAvailableHenhouses(farmId),
      (data) => setHenhouses(data.responseData?.items ?? []),
      undefined,
      "Nie udało się pobrać listy kurników"
    );
    setLoadingHenhouses(false);

    const cycle = await loadLatestCycle(farmId);
    if (!cycle) {
      setErrors((prev) => ({
        ...prev,
        identifierId: "Brak aktywnego cyklu",
      }));
      return;
    }

    dispatch({ type: "SET_FIELD", name: "identifierId", value: cycle.id });
    dispatch({
      type: "SET_FIELD",
      name: "identifierDisplay",
      value: `${cycle.identifier}/${cycle.year}`,
    });
  };

  const validateEntry = (entry: InsertionEntry): InsertionEntryErrors => {
    const e: InsertionEntryErrors = {};

    if (!entry.hatcheryId) e.hatcheryId = "Wylęgarnia jest wymagana";
    if (!entry.henhouseId) e.henhouseId = "Kurnik jest wymagany";

    const quantityNum = Number(entry.quantity);
    if (entry.quantity === "" || isNaN(quantityNum) || quantityNum <= 0)
      e.quantity = "Ilość musi być większa niż 0";

    const bodyWeightNum = Number(entry.bodyWeight);
    if (entry.bodyWeight === "" || isNaN(bodyWeightNum) || bodyWeightNum <= 0)
      e.bodyWeight = "Masa musi być większa niż 0";

    return e;
  };

  const setEntryErrors = (index: number, entryErrors: InsertionEntryErrors) => {
    setErrors((prev) => {
      const newEntriesErrors = { ...(prev.entries || {}) };
      newEntriesErrors[index] = entryErrors;
      return { ...prev, entries: newEntriesErrors };
    });
  };

  const validateForm = (): boolean => {
    const newErrors: InsertionFormErrors = {};

    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.identifierId) newErrors.identifierId = "Brak aktywnego cyklu";
    if (!form.insertionDate) newErrors.insertionDate = "Data jest wymagana";

    if (form.entries.length === 0) {
      newErrors.entriesGeneral = "Musisz dodać przynajmniej jedną pozycję";
    } else {
      const entryErrors: { [index: number]: InsertionEntryErrors } = {};

      form.entries.forEach((entry, index) => {
        const e: InsertionEntryErrors = validateEntry(entry);

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
        InsertionsService.addNewInsertion({
          farmId: form.farmId,
          cycleId: form.identifierId,
          insertionDate: form.insertionDate!.format("YYYY-MM-DD"),
          entries: form.entries.map(
            ({ henhouseId, hatcheryId, quantity, bodyWeight }) => ({
              henhouseId,
              hatcheryId,
              quantity: Number(quantity),
              bodyWeight: Number(bodyWeight),
            })
          ),
        }),
      async (data) => {
        if (!data || !data.responseData || !data.responseData.internalGroupId) {
          toast.error(
            "Nie udało się dodać wstawienia, brak ID grupy wewnętrznej"
          );
        }

        if (sendToIrz) {
          await handleApiResponse(
            () =>
              InsertionsService.sendToIrzPlus({
                internalGroupId: data.responseData!.internalGroupId,
              }),
            () => {
              toast.success("Wstawienie wysłane do IRZplus");
            },
            undefined,
            "Nie udało się wysłać wstawienia do IRZplus"
          );
        }

        toast.success("Dodano wstawienie");
        dispatch({ type: "RESET" });
        setErrors({});
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się dodać wstawienia"
    );

    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    setHenhouses([]);
    onClose();
  };

  return (
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason !== "backdropClick") {
          handleClose();
        }
      }}
      fullWidth
      maxWidth="lg"
    >
      <DialogTitle>Nowe wstawienie</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
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
            label="Identyfikator"
            value={form.identifierDisplay}
            slotProps={{ input: { readOnly: true } }}
            error={!!errors.identifierId}
            helperText={errors.identifierId}
            fullWidth
          />

          <DatePicker
            label="Data wstawienia"
            value={form.insertionDate}
            onChange={(value) => {
              dispatch({ type: "SET_FIELD", name: "insertionDate", value });
              setErrors((prev) => ({ ...prev, insertionDate: undefined }));
            }}
            disableFuture
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                error: !!errors.insertionDate,
                helperText: errors.insertionDate,
                fullWidth: true,
              },
            }}
          />

          {errors.entriesGeneral && (
            <Box sx={{ mb: 1 }}>
              <Typography color="error">{errors.entriesGeneral}</Typography>
            </Box>
          )}

          <InsertionEntriesTable
            entries={form.entries}
            henhouses={henhouses}
            hatcheries={hatcheries}
            errors={errors.entries}
            dispatch={dispatch}
            setEntryErrors={setEntryErrors}
            validateEntry={validateEntry}
            loadingHenhouses={loadingHenhouses}
            loadingHatcheries={loadingHatcheries}
            farmId={form.farmId}
          />

          <Button
            variant="outlined"
            onClick={() => {
              dispatch({ type: "ADD_ENTRY" });
              setErrors((prev) => ({ ...prev, entriesGeneral: undefined }));
            }}
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
              sx={{
                "&.MuiCheckbox-root": {
                  color: "error.main",
                },
                "&.Mui-checked": {
                  color: "error.main",
                },
              }}
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
    </Dialog>
  );
};

export default AddInsertionModal;
