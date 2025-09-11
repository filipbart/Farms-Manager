import React, { useEffect, useReducer, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { toast } from "react-toastify";
import { HatcheriesService } from "../../../services/hatcheries-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import LoadingButton from "../../common/loading-button";
import type { Dayjs } from "dayjs";
import dayjs from "dayjs";
import { MdSave } from "react-icons/md";
import AppDialog from "../../common/app-dialog";
import type { HatcheryPriceListModel } from "../../../models/hatcheries/hatcheries-prices";

interface EditHatcheryPriceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  hatcheryPrice: HatcheryPriceListModel | undefined;
}

interface PriceFormState {
  hatcheryName: string;
  price: string | number;
  date: Dayjs | null;
  comment: string;
}

interface PriceFormErrors {
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
    case "SET_ALL":
      return { ...state, ...action.payload };
    case "RESET":
      return initialState;
    default:
      return state;
  }
}

const EditHatcheryPriceModal: React.FC<EditHatcheryPriceModalProps> = ({
  open,
  onClose,
  onSave,
  hatcheryPrice,
}) => {
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<PriceFormErrors>({});

  useEffect(() => {
    if (hatcheryPrice) {
      dispatch({
        type: "SET_ALL",
        payload: {
          price: hatcheryPrice.price,
          date: dayjs(hatcheryPrice.date),
          comment: hatcheryPrice.comment || "",
        },
      });
    }
  }, [hatcheryPrice]);

  const validateForm = (): boolean => {
    const newErrors: PriceFormErrors = {};

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
    if (!validateForm() || !hatcheryPrice) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        HatcheriesService.editHatcheryPrice(hatcheryPrice.id, {
          price: Number(form.price),
          date: form.date!.format("YYYY-MM-DD"),
          comment: form.comment,
        }),
      () => {
        toast.success("Cena została zaktualizowana");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się zaktualizować ceny"
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
      <DialogTitle>Edytuj cenę</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
          <TextField
            label="Wylęgarnia"
            value={hatcheryPrice?.hatcheryName || ""}
            fullWidth
            disabled
          />

          <DatePicker
            label="Data ceny"
            value={form.date}
            onChange={(value) => {
              dispatch({ type: "SET_FIELD", name: "date", value });
              setErrors((p) => ({ ...p, date: undefined }));
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
          Zapisz zmiany
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditHatcheryPriceModal;
