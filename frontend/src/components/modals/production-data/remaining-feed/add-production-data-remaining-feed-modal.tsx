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
import { ProductionDataRemainingFeedService } from "../../../../services/production-data/production-data-remaining-feed-service";
import type { AddRemainingFeedData } from "../../../../models/production-data/remaining-feed";
import { ProductionDataService } from "../../../../services/production-data/production-data-service";

interface AddProductionDataRemainingFeedModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddProductionDataRemainingFeedModal: React.FC<
  AddProductionDataRemainingFeedModalProps
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
  } = useForm<AddRemainingFeedData & { cycleDisplay: string }>();

  const [loading, setLoading] = useState(false);
  const [isCalculatingValue, setIsCalculatingValue] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [availableHenhouses, setAvailableHenhouses] = useState<HouseRowModel[]>(
    []
  );

  const cycleId = watch("cycleId");
  const henhouseId = watch("henhouseId");
  const feedName = watch("feedName");
  const remainingTonnage = watch("remainingTonnage");

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchFeedsNames();
    }
  }, [open, fetchFarms, fetchFeedsNames]);

  useEffect(() => {
    const calculateValue = async () => {
      if (cycleId && henhouseId && feedName && remainingTonnage > 0) {
        setIsCalculatingValue(true);
        try {
          const response = await ProductionDataService.calculateValue({
            cycleId,
            henhouseId,
            feedName,
            tonnage: remainingTonnage,
          });
          if (response.success && response.responseData) {
            setValue("remainingValue", response.responseData.value);
          }
        } catch (error) {
          console.error("Błąd podczas obliczania wartości", error);
          setValue("remainingValue", 0);
        } finally {
          setIsCalculatingValue(false);
        }
      }
    };

    const debounceTimeout = setTimeout(() => {
      calculateValue();
    }, 500);

    return () => clearTimeout(debounceTimeout);
  }, [henhouseId, feedName, remainingTonnage, setValue]);

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("cycleDisplay", "");
    setValue("henhouseId", "");
    clearErrors("cycleId");
    setAvailableHenhouses([]);

    const selectedFarm = farms.find((f) => f.id === farmId);
    if (selectedFarm) {
      setAvailableHenhouses(selectedFarm.henhouses);
    }

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      setValue("cycleDisplay", `${cycle.identifier}/${cycle.year}`);
      clearErrors("cycleId");
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleSave = async (data: AddRemainingFeedData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => ProductionDataRemainingFeedService.addRemainingFeed(data),
      () => {
        toast.success("Pomyślnie dodano wpis o pozostałej paszy");
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
    setAvailableHenhouses([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="sm">
      <DialogTitle>Dodaj wpis o pozostałej paszy</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Box display="flex" flexDirection="column" gap={2.5} mt={1}>
            <LoadingTextField
              loading={loadingFarms}
              select
              label="Ferma"
              fullWidth
              error={!!errors.farmId}
              helperText={errors.farmId?.message}
              {...register("farmId", {
                required: "Farma jest wymagana",
                onChange: (e) => handleFarmChange(e.target.value),
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
              value={watch("cycleDisplay") || ""}
              InputProps={{ readOnly: true }}
              error={!!errors.cycleId}
              helperText={errors.cycleId?.message}
              fullWidth
            />

            <TextField
              select
              label="Kurnik"
              fullWidth
              disabled={!watch("farmId") || availableHenhouses.length === 0}
              error={!!errors.henhouseId}
              helperText={errors.henhouseId?.message}
              {...register("henhouseId", {
                required: "Kurnik jest wymagany",
              })}
            >
              {availableHenhouses.map((henhouse) => (
                <MenuItem key={henhouse.id} value={henhouse.id}>
                  {henhouse.name}
                </MenuItem>
              ))}
            </TextField>

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

            <TextField
              label="Tonaż pozostały [t]"
              type="number"
              slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
              error={!!errors.remainingTonnage}
              helperText={errors.remainingTonnage?.message}
              {...register("remainingTonnage", {
                required: "Tonaż jest wymagany",
                valueAsNumber: true,
                min: { value: 0, message: "Wartość nie może być ujemna" },
              })}
              fullWidth
            />
            <LoadingTextField
              label="Wartość pozostała [zł]"
              type="number"
              loading={isCalculatingValue}
              value={String(watch("remainingValue") || 0)}
              slotProps={{ input: { readOnly: true } }}
              error={!!errors.remainingValue}
              helperText={errors.remainingValue?.message}
              {...register("remainingValue", {
                required: "Wartość jest wymagana",
                valueAsNumber: true,
                min: { value: 0, message: "Wartość nie może być ujemna" },
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

export default AddProductionDataRemainingFeedModal;
