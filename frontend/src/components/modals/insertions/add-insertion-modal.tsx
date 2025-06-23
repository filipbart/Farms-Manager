import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useEffect, useState, type ChangeEvent } from "react";
import { Dayjs } from "dayjs";
import { useFarms } from "../../../hooks/useFarms";
import LoadingTextField from "../../common/loading-textfield";
import { useHatcheries } from "../../../hooks/useHatcheries";
import type { HouseRowModel } from "../../../models/farms/house-row-model";
import { FarmsService } from "../../../services/farms-service";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type LatestCycle from "../../../models/farms/latest-cycle";
import { InsertionsService } from "../../../services/insertions-service";
import { toast } from "react-toastify";
import LoadingButton from "../../common/loading-button";

interface AddInsertionModalProps {
  open: boolean;
  onClose: () => void;
}

interface InsertionFormState {
  farmId: string;
  henhouseId: string;
  identifierId: string;
  identifierDisplay: string;
  insertionDate: Dayjs | null;
  quantity: number | "";
  hatcheryId: string;
  bodyWeight: number | "";
}

interface InsertionFormErrors {
  [key: string]: string | undefined;
}

const defaultForm: InsertionFormState = {
  farmId: "",
  henhouseId: "",
  identifierId: "",
  identifierDisplay: "",
  insertionDate: null,
  quantity: "",
  hatcheryId: "",
  bodyWeight: "",
};

const AddInsertionModal: React.FC<AddInsertionModalProps> = ({
  open,
  onClose,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { hatcheries, loadingHatcheries, fetchHatcheries } = useHatcheries();

  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [loadingHenhouses, setLoadingHenhouses] = useState(false);
  const [loadingLatestCycle, setLoadingLatestCycle] = useState(false);
  const [loading, setLoading] = useState(false);

  const [form, setForm] = useState<InsertionFormState>(defaultForm);
  const [errors, setErrors] = useState<InsertionFormErrors>({});

  useEffect(() => {
    fetchFarms();
    fetchHatcheries();
  }, []);

  const updateField = (name: string, value: any) => {
    setForm((prev) => ({ ...prev, [name]: value }));
    setErrors((prev) => ({ ...prev, [name]: undefined }));
  };

  const handleChange = async (
    e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;

    updateField(name, value);

    if (name === "farmId") {
      updateField("henhouseId", "");
      updateField("identifierId", "");
      updateField("identifierDisplay", "");
      setHenhouses([]);
      setErrors((prev) => ({
        ...prev,
        henhouseId: undefined,
        identifierId: undefined,
      }));

      setLoadingHenhouses(true);
      try {
        await handleApiResponse<PaginateModel<HouseRowModel>>(
          () => FarmsService.getFarmHousesAsync(value),
          (data) => setHenhouses(data.responseData?.items ?? []),
          undefined,
          "Nie udało się pobrać listy kurników"
        );
      } finally {
        setLoadingHenhouses(false);
      }

      setLoadingLatestCycle(true);
      try {
        await handleApiResponse<LatestCycle>(
          () => FarmsService.getLatestCycle(value),
          (data) => {
            if (!data?.responseData) {
              setErrors((prev) => ({
                ...prev,
                identifierId: "Brak aktywnego cyklu",
              }));
              updateField("identifierId", "");
              updateField("identifierDisplay", "");
              return;
            }
            const cycle = data.responseData;
            updateField("identifierId", cycle.id);
            updateField(
              "identifierDisplay",
              `${cycle.identifier}/${cycle.year}`
            );
          },
          undefined,
          "Nie udało się pobrać ostatniego cyklu"
        );
      } finally {
        setLoadingLatestCycle(false);
      }
    }
  };

  const validateForm = (): boolean => {
    const newErrors: InsertionFormErrors = {};

    if (!form.farmId) newErrors.farmId = "Ferma jest wymagana";
    if (!form.henhouseId) newErrors.henhouseId = "Kurnik jest wymagany";
    if (!form.identifierId) newErrors.identifierId = "Brak aktywnego cyklu";
    if (!form.insertionDate)
      newErrors.insertionDate = "Data wstawienia jest wymagana";
    if (form.quantity === "" || form.quantity < 1)
      newErrors.quantity = "Ilość musi być większa niż 0";
    if (!form.hatcheryId) newErrors.hatcheryId = "Wylęgarnia jest wymagana";
    if (form.bodyWeight === "" || form.bodyWeight < 0)
      newErrors.bodyWeight = "Masa ciała musi być większa niż 0";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    console.log("Zapisuję dane:", form);
    setLoading(true);
    try {
      await handleApiResponse(
        () =>
          InsertionsService.addNewInsertion({
            farmId: form.farmId,
            henhouseId: form.henhouseId,
            identifierId: form.identifierId,
            insertionDate: form.insertionDate!.toDate(),
            quantity: form.quantity as number,
            hatcheryId: form.hatcheryId,
            bodyWeight: form.bodyWeight as number,
          }),
        () => {
          toast.success("Pomyślnie dodano wstawienie");
          setForm(defaultForm);
          setErrors({});
          onClose();
        },
        undefined,
        "Nie udało się dodać wstawienia. Sprawdź dane"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setForm(defaultForm);
    setErrors({});
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź Dane Wstawienia</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <LoadingTextField
            loading={loadingFarms}
            select
            name="farmId"
            label="Ferma"
            value={form.farmId}
            onChange={handleChange}
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
            loading={loadingHenhouses}
            select
            name="henhouseId"
            label="Kurnik"
            value={form.henhouseId}
            onChange={handleChange}
            disabled={!form.farmId}
            error={!!errors.henhouseId}
            helperText={errors.henhouseId}
            fullWidth
          >
            {henhouses.map((house) => (
              <MenuItem key={house.id} value={house.id}>
                {house.name}
              </MenuItem>
            ))}
          </LoadingTextField>

          <LoadingTextField
            loading={loadingLatestCycle}
            name="identifierDisplay"
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
            disableFuture
            onChange={(newValue) => {
              updateField("insertionDate", newValue);
            }}
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                error: !!errors.insertionDate,
                helperText: errors.insertionDate,
                fullWidth: true,
              },
            }}
          />

          <TextField
            name="quantity"
            label="Sztuki wstawione"
            type="number"
            inputProps={{ min: 1, step: 1 }}
            value={form.quantity}
            onChange={handleChange}
            error={!!errors.quantity}
            helperText={errors.quantity}
            fullWidth
          />

          <LoadingTextField
            loading={loadingHatcheries}
            select
            name="hatcheryId"
            label="Wylęgarnia"
            value={form.hatcheryId}
            onChange={handleChange}
            error={!!errors.hatcheryId}
            helperText={errors.hatcheryId}
            fullWidth
          >
            {hatcheries.map((hatchery) => (
              <MenuItem key={hatchery.id} value={hatchery.id}>
                {hatchery.name}
              </MenuItem>
            ))}
          </LoadingTextField>

          <TextField
            name="bodyWeight"
            label="Śr. masa ciała"
            value={form.bodyWeight}
            onChange={handleChange}
            type="number"
            inputProps={{ min: 0, step: 0.01 }}
            error={!!errors.bodyWeight}
            helperText={errors.bodyWeight}
            fullWidth
          />
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={handleClose} variant="outlined" color="inherit">
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          color="primary"
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default AddInsertionModal;
