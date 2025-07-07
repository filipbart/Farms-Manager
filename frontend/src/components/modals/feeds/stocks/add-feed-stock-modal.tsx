import {
  Dialog,
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
import type { AddFeedStockFormData } from "../../../../services/feeds-service";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { FarmsService } from "../../../../services/farms-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";

interface AddHatcheryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddFeedStockModal: React.FC<AddHatcheryModalProps> = ({
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
  } = useForm<AddFeedStockFormData>();

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [loadingLatestCycle, setLoadingLatestCycle] = useState(false);

  const handleSave = async (data: AddFeedStockFormData) => {
    if (loading) return;
    setLoading(true);
    console.log("Saving feed stock:", data);
    setLoading(false);
  };

  useEffect(() => {
    fetchFarms();
  }, []);

  const handleFarmChange = async (farmId: string) => {
    setLoadingLatestCycle(true);
    console.log("Fetching latest cycle for farm:", farmId);
    await handleApiResponse(
      () => FarmsService.getLatestCycle(farmId),
      (data) => {
        if (!data?.responseData) {
          setError("identifierId", {
            type: "manual",
            message: "Brak aktywnego cyklu",
          });
          return;
        }
        const cycle = data.responseData;
        setValue("identifierId", cycle.id);
        clearErrors("identifierId");
        setValue("identifierDisplay", `${cycle.identifier}/${cycle.year}`);
      },
      undefined,
      "Nie udało się pobrać ostatniego cyklu"
    );
  };

  const close = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
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
                  await handleFarmChange(value); // <-- załaduj cykl
                },
              })}
            >
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>

            <TextField
              label="Cykl"
              value={watch("identifierDisplay") || ""}
              slotProps={{ input: { readOnly: true } }}
              fullWidth
            />

            <Controller
              name="stockDate"
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
                      error: !!errors.stockDate,
                      helperText: errors.stockDate?.message,
                    },
                  }}
                />
              )}
            />

            <TextField
              label="Typ (nazwa) paszy"
              error={!!errors.name}
              helperText={errors.name?.message}
              {...register("name", {
                required: "Typ (nazwa) paszy jest wymagany",
              })}
              fullWidth
            />

            <TextField
              label="Cena [zł]"
              type="number"
              error={!!errors.price}
              helperText={errors.price?.message}
              {...register("price", {
                required: "Cena [zł] jest wymagana",
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
    </Dialog>
  );
};

export default AddFeedStockModal;
