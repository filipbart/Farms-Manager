import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { FeedsService } from "../../../../services/feeds-service";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddFeedPriceFormData } from "../../../../models/feeds/prices/feed-price";
import AppDialog from "../../../common/app-dialog";

interface AddHatcheryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddFeedPriceModal: React.FC<AddHatcheryModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    setError,
    clearErrors,
    watch,
  } = useForm<AddFeedPriceFormData>();

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();

  const handleSave = async (data: AddFeedPriceFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => FeedsService.addFeedPrice(data),
      () => {
        onSave();
        reset();
        onClose();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania ceny paszy"
    );

    setLoading(false);
  };

  useEffect(() => {
    fetchFarms();
    fetchFeedsNames();
  }, [fetchFarms, fetchFeedsNames]);

  const handleFarmChange = async (farmId: string) => {
    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("identifierId", cycle.id);
      clearErrors("identifierId");
      setValue("identifierDisplay", `${cycle.identifier}/${cycle.year}`);
    } else {
      setError("identifierId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleFeedsNameChange = (name: string) => {
    if (feedsNames.some((feed) => feed.name === name)) {
      setError("nameId", {
        type: "manual",
        message: "Pasza o tej nazwie już istnieje",
      });
    } else {
      clearErrors("nameId");
    }
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Wprowadź nową cenę</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <LoadingTextField
              loading={loadingFarms}
              select
              label="Ferma"
              fullWidth
              error={!!errors.farmId}
              helperText={errors.farmId?.message}
              {...register("farmId", {
                required: "Farma jest wymagana",
                onChange: async (e) => {
                  const value = e.target.value;
                  await handleFarmChange(value);
                },
              })}
            >
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <LoadingTextField
              loading={loadingCycle}
              label="Cykl"
              value={watch("identifierDisplay") || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />

            <Controller
              name="priceDate"
              control={control}
              rules={{ required: "Data publikacji jest wymagana" }}
              render={({ field }) => (
                <DatePicker
                  label="Data publikacji"
                  disableFuture
                  format="DD.MM.YYYY"
                  value={field.value ? dayjs(field.value) : null}
                  onChange={(date) =>
                    field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
                  }
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.priceDate,
                      helperText: errors.priceDate?.message,
                    },
                  }}
                />
              )}
            />

            <LoadingTextField
              loading={loadingFeedsNames}
              select
              label="Typ (nazwa) paszy"
              fullWidth
              error={!!errors.nameId}
              helperText={errors.nameId?.message}
              {...register("nameId", {
                required: "Typ (nazwa) paszy jest wymagany",
                onChange: (e) => {
                  const value = e.target.value;
                  handleFeedsNameChange(value);
                },
              })}
            >
              {feedsNames.map((feedName) => (
                <MenuItem key={feedName.id} value={feedName.id}>
                  {feedName.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <TextField
              label="Cena [zł]"
              type="number"
              inputMode="decimal"
              slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
              error={!!errors.price}
              helperText={errors.price?.message}
              {...register("price", {
                required: "Cena [zł] jest wymagana",
                valueAsNumber: true,
              })}
              fullWidth
            />
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
            type="submit"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            disabled={loading}
            loading={loading}
          >
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddFeedPriceModal;
