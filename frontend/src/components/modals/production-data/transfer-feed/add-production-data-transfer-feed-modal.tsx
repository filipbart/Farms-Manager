import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
  Typography,
  Divider,
  Grid,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import AppDialog from "../../../common/app-dialog";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import { ProductionDataTransferFeedService } from "../../../../services/production-data/production-data-transfer-feed-service";
import { ProductionDataService } from "../../../../services/production-data/production-data-service";
import type { AddTransferFeedData } from "../../../../models/production-data/transfer-feed";

interface AddProductionDataTransferFeedModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddProductionDataTransferFeedModal: React.FC<
  AddProductionDataTransferFeedModalProps
> = ({ open, onClose, onSave }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    setError,
    clearErrors,
    watch,
  } = useForm<
    AddTransferFeedData & {
      fromCycleDisplay: string;
      toCycleDisplay: string;
    }
  >();

  const [loading, setLoading] = useState(false);
  const [isCalculatingValue, setIsCalculatingValue] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const { loadLatestCycle, loadingCycle: loadingFromCycle } = useLatestCycle();
  const { loadLatestCycle: loadToCycle, loadingCycle: loadingToCycle } =
    useLatestCycle();

  const [availableFromHenhouses, setAvailableFromHenhouses] = useState<
    HouseRowModel[]
  >([]);
  const [availableToHenhouses, setAvailableToHenhouses] = useState<
    HouseRowModel[]
  >([]);

  const fromCycleId = watch("fromCycleId");
  const fromHenhouseId = watch("fromHenhouseId");
  const feedName = watch("feedName");
  const tonnage = watch("tonnage");

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchFeedsNames();
    }
  }, [open, fetchFarms, fetchFeedsNames]);

  useEffect(() => {
    const calculateValue = async () => {
      if (fromCycleId && fromHenhouseId && feedName && tonnage > 0) {
        setIsCalculatingValue(true);
        try {
          const response = await ProductionDataService.calculateValue({
            cycleId: fromCycleId,
            henhouseId: fromHenhouseId,
            feedName,
            tonnage,
          });
          if (response.success && response.responseData) {
            setValue("value", response.responseData.value);
          }
        } catch (error) {
          console.error("Błąd podczas obliczania wartości", error);
          setValue("value", 0);
        } finally {
          setIsCalculatingValue(false);
        }
      }
    };
    const debounceTimeout = setTimeout(() => {
      calculateValue();
    }, 500);

    return () => clearTimeout(debounceTimeout);
  }, [fromCycleId, fromHenhouseId, feedName, tonnage, setValue]);

  const handleFromFarmChange = async (farmId: string) => {
    setValue("fromCycleId", "");
    setValue("fromCycleDisplay", "");
    setValue("fromHenhouseId", "");
    clearErrors("fromCycleId");
    setAvailableFromHenhouses([]);

    const selectedFarm = farms.find((f) => f.id === farmId);
    if (selectedFarm) {
      setAvailableFromHenhouses(selectedFarm.henhouses);
    }
    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("fromCycleId", cycle.id);
      setValue("fromCycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("fromCycleId");
    } else {
      setError("fromCycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleToFarmChange = async (farmId: string) => {
    setValue("toCycleId", "");
    setValue("toCycleDisplay", "");
    setValue("toHenhouseId", "");
    clearErrors("toCycleId");
    setAvailableToHenhouses([]);

    const selectedFarm = farms.find((f) => f.id === farmId);
    if (selectedFarm) {
      setAvailableToHenhouses(selectedFarm.henhouses);
    }
    const cycle = await loadToCycle(farmId);
    if (cycle) {
      setValue("toCycleId", cycle.id);
      setValue("toCycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("toCycleId");
    } else {
      setError("toCycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleSave = async (data: AddTransferFeedData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataTransferFeedService.addFeedTransfer(data),
      () => {
        toast.success("Pomyślnie dodano przeniesienie paszy");
        onSave();
        close();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania danych"
    );
    setLoading(false);
  };

  const close = () => {
    reset();
    setAvailableFromHenhouses([]);
    setAvailableToHenhouses([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="lg">
      <DialogTitle>Dodaj przeniesienie paszy</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={4} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Box display="flex" flexDirection="column" gap={2.5}>
                <Typography variant="h6">Przeniesiono Z</Typography>
                <LoadingTextField
                  loading={loadingFarms}
                  select
                  label="Ferma"
                  fullWidth
                  error={!!errors.fromFarmId}
                  helperText={errors.fromFarmId?.message}
                  {...register("fromFarmId", {
                    required: "Farma jest wymagana",
                    onChange: (e) => handleFromFarmChange(e.target.value),
                  })}
                >
                  {farms.map((farm) => (
                    <MenuItem key={farm.id} value={farm.id}>
                      {farm.name}
                    </MenuItem>
                  ))}
                </LoadingTextField>
                <LoadingTextField
                  loading={loadingFromCycle}
                  label="Cykl"
                  value={watch("fromCycleDisplay") || ""}
                  slotProps={{ input: { readOnly: true } }}
                  error={!!errors.fromCycleId}
                  helperText={errors.fromCycleId?.message}
                  fullWidth
                />
                <TextField
                  select
                  label="Kurnik"
                  fullWidth
                  disabled={
                    !watch("fromFarmId") || availableFromHenhouses.length === 0
                  }
                  error={!!errors.fromHenhouseId}
                  helperText={errors.fromHenhouseId?.message}
                  {...register("fromHenhouseId", {
                    required: "Kurnik jest wymagany",
                  })}
                >
                  {availableFromHenhouses.map((henhouse) => (
                    <MenuItem key={henhouse.id} value={henhouse.id}>
                      {henhouse.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Box>
            </Grid>
            <Grid size={{ xs: 12, md: 6 }}>
              <Box display="flex" flexDirection="column" gap={2.5}>
                <Typography variant="h6">Przeniesiono Do</Typography>
                <LoadingTextField
                  loading={loadingFarms}
                  select
                  label="Ferma"
                  fullWidth
                  error={!!errors.toFarmId}
                  helperText={errors.toFarmId?.message}
                  {...register("toFarmId", {
                    required: "Farma jest wymagana",
                    onChange: (e) => handleToFarmChange(e.target.value),
                  })}
                >
                  {farms.map((farm) => (
                    <MenuItem key={farm.id} value={farm.id}>
                      {farm.name}
                    </MenuItem>
                  ))}
                </LoadingTextField>
                <LoadingTextField
                  loading={loadingToCycle}
                  label="Cykl"
                  value={watch("toCycleDisplay") || ""}
                  slotProps={{ input: { readOnly: true } }}
                  error={!!errors.toCycleId}
                  helperText={errors.toCycleId?.message}
                  fullWidth
                />
                <TextField
                  select
                  label="Kurnik"
                  fullWidth
                  disabled={
                    !watch("toFarmId") || availableToHenhouses.length === 0
                  }
                  error={!!errors.toHenhouseId}
                  helperText={errors.toHenhouseId?.message}
                  {...register("toHenhouseId", {
                    required: "Kurnik jest wymagany",
                  })}
                >
                  {availableToHenhouses.map((henhouse) => (
                    <MenuItem key={henhouse.id} value={henhouse.id}>
                      {henhouse.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Box>
            </Grid>
            <Grid size={{ xs: 12 }}>
              <Divider sx={{ my: 2 }} />
            </Grid>
            <Grid container size={{ xs: 12 }} spacing={2.5}>
              <Grid size={{ xs: 12, sm: 4 }}>
                <LoadingTextField
                  loading={loadingFeedsNames}
                  select
                  label="Typ (nazwa) paszy"
                  fullWidth
                  error={!!errors.feedName}
                  helperText={errors.feedName?.message}
                  {...register("feedName", {
                    required: "Nazwa paszy jest wymagana",
                  })}
                >
                  {feedsNames.map((feed) => (
                    <MenuItem key={feed.id} value={feed.name}>
                      {feed.name}
                    </MenuItem>
                  ))}
                </LoadingTextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField
                  label="Tonaż [t]"
                  type="number"
                  slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                  error={!!errors.tonnage}
                  helperText={errors.tonnage?.message}
                  {...register("tonnage", {
                    required: "Tonaż jest wymagany",
                    valueAsNumber: true,
                    min: { value: 0, message: "Wartość nie może być ujemna" },
                  })}
                  fullWidth
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <LoadingTextField
                  label="Wartość [zł]"
                  type="number"
                  loading={isCalculatingValue}
                  value={String(watch("value") ?? "")}
                  slotProps={{ input: { readOnly: true } }}
                  error={!!errors.value}
                  helperText={errors.value?.message}
                  {...register("value", {
                    required: "Wartość jest wymagana",
                    valueAsNumber: true,
                    min: { value: 0, message: "Wartość nie może być ujemna" },
                  })}
                  fullWidth
                />
              </Grid>
            </Grid>
          </Grid>
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
            disabled={loading || isCalculatingValue}
            loading={loading}
          >
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddProductionDataTransferFeedModal;
