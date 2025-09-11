import React, { useEffect, useReducer, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  MenuItem,
  Box,
  TextField,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { toast } from "react-toastify";
import { HatcheriesService } from "../../../services/hatcheries-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingTextField from "../../common/loading-textfield";
import LoadingButton from "../../common/loading-button";
import type { Dayjs } from "dayjs";
import { MdSave } from "react-icons/md";
import AppDialog from "../../common/app-dialog";
import type { HatcheryName } from "../../../models/hatcheries/hatcheries-prices";

interface AddHatcheryPriceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

interface PriceFormState {
  hatcheryName: string;
  price: string | number;
  date: Dayjs | null;
  comment: string;
}

interface PriceFormErrors {
  hatcheryName?: string;
  price?: string;
  date?: string;
}

const initialState: PriceFormState = {
  hatcheryName: "",
  price: "",
  date: null,
  comment: "",
};

function formReducer(state: PriceFormState, action: any): PriceFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.name]: action.value };
    case "RESET":
      return initialState;
    default:
      return state;
  }
}

const AddHatcheryPriceModal: React.FC<AddHatcheryPriceModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<PriceFormErrors>({});
  const [hatcheries, setHatcheries] = useState<HatcheryName[]>([]);
  const [loadingHatcheries, setLoadingHatcheries] = useState(false);

  useEffect(() => {
    if (open) {
      const fetchHatcheries = async () => {
        setLoadingHatcheries(true);
        await handleApiResponse(
          () => HatcheriesService.getPricesNames(),
          (data) => {
            setHatcheries(data.responseData?.hatcheries ?? []);
          },
          undefined,
          "Błąd podczas pobierania nazw wylęgarni"
        );
        setLoadingHatcheries(false);
      };
      fetchHatcheries();
    }
  }, [open]);

  const validateForm = (): boolean => {
    const newErrors: PriceFormErrors = {};

    if (!form.hatcheryName) {
      newErrors.hatcheryName = "Wybór wylęgarni jest wymagany";
    }
    if (!form.date) {
      newErrors.date = "Data jest wymagana";
    }
    const priceNum = Number(form.price);
    if (form.price === "" || isNaN(priceNum) || priceNum <= 0) {
      newErrors.price = "Cena musi być liczbą większą od zera";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        HatcheriesService.addHatcheryPrice({
          hatcheryName: form.hatcheryName,
          price: Number(form.price),
          date: form.date!.format("YYYY-MM-DD"),
          comment: form.comment,
        }),
      () => {
        toast.success("Dodano nową cenę");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się dodać ceny"
    );
    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź cenę</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <LoadingTextField
            loading={loadingHatcheries}
            select
            label="Wylęgarnia"
            value={form.hatcheryName}
            onChange={(e) => {
              dispatch({
                type: "SET_FIELD",
                name: "hatcheryName",
                value: e.target.value,
              });
              setErrors((p) => ({ ...p, hatcheryName: undefined }));
            }}
            error={!!errors.hatcheryName}
            helperText={errors.hatcheryName}
            fullWidth
          >
            {hatcheries.map((hatchery) => (
              <MenuItem key={hatchery.id} value={hatchery.name}>
                {hatchery.name}
              </MenuItem>
            ))}
          </LoadingTextField>

          <DatePicker
            label="Data ceny"
            value={form.date}
            onChange={(value) => {
              dispatch({ type: "SET_FIELD", name: "date", value });
              setErrors((p) => ({ ...p, date: undefined }));
            }}
            format="DD.MM.YYYY"
            slotProps={{
              textField: {
                error: !!errors.date,
                helperText: errors.date,
                fullWidth: true,
              },
            }}
          />

          <TextField
            label="Cena [zł]"
            type="number"
            value={form.price}
            onChange={(e) => {
              dispatch({
                type: "SET_FIELD",
                name: "price",
                value: e.target.value,
              });
              setErrors((p) => ({ ...p, price: undefined }));
            }}
            error={!!errors.price}
            helperText={errors.price}
            fullWidth
            slotProps={{
              htmlInput: {
                step: "0.01",
                min: "0",
              },
            }}
          />

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

export default AddHatcheryPriceModal;
