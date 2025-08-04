import { Box, Typography, Paper, MenuItem, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import LoadingButton from "../../components/common/loading-button";
import LoadingTextField from "../../components/common/loading-textfield";
import { useFarms } from "../../hooks/useFarms";
import { useLatestCycle } from "../../hooks/useLatestCycle";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { FarmsService } from "../../services/farms-service";

export interface CycleSettingsData {
  farmId: string;
  cycle: string;
}

const SettingsCyclesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    formState: { errors, isDirty },
  } = useForm<CycleSettingsData>({
    defaultValues: {
      farmId: "",
      cycle: "",
    },
  });

  const selectedFarmId = watch("farmId");

  const currentYear = new Date().getFullYear();
  const cycleOptions = Array.from({ length: 6 }, (_, i) => {
    const identifier = i + 1;
    return {
      label: `${identifier}/${currentYear}`,
      value: `${identifier}-${currentYear}`,
    };
  });

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  useEffect(() => {
    const fetchAndSetCycle = async () => {
      if (selectedFarmId && farms.length > 0) {
        const cycle = await loadLatestCycle(selectedFarmId);
        if (cycle) {
          const cycleValue = `${cycle.identifier}-${cycle.year}`;
          if (watch("cycle") !== cycleValue) {
            setValue("cycle", cycleValue, { shouldDirty: true });
          }
        }
      }
    };

    fetchAndSetCycle();
  }, [selectedFarmId, farms.length]);

  const onSubmit = async (data: CycleSettingsData) => {
    setLoading(true);
    await handleApiResponse(
      () => FarmsService.updateFarmCycle(data),
      async () => {
        toast.success("Cykl został poprawnie zaktualizowany");
        await fetchFarms();
        reset(data);
      },
      undefined,
      "Błąd podczas aktualizacji cyklu"
    );
    setLoading(false);
  };

  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>
        Ustawienia Cykli
      </Typography>
      <Paper
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        sx={{ p: 3, maxWidth: 600 }}
        variant="outlined"
      >
        <Grid container spacing={3} direction="column">
          <Grid>
            <LoadingTextField
              loading={loadingFarms}
              select
              label="Ferma"
              fullWidth
              value={watch("farmId") || ""}
              error={!!errors.farmId}
              helperText={errors.farmId?.message}
              {...register("farmId", { required: "Wybierz fermę" })}
            >
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid>
            <LoadingTextField
              select
              label="Aktywny cykl"
              fullWidth
              loading={loadingCycle}
              value={watch("cycle") || ""}
              disabled={!watch("farmId")}
              error={!!errors.cycle}
              helperText={
                errors.cycle?.message ??
                "Wybierz cykl, który ma być aktywny dla tej fermy."
              }
              {...register("cycle", { required: "Wybierz aktywny cykl" })}
            >
              {cycleOptions.map((cycle) => (
                <MenuItem key={cycle.value} value={cycle.value}>
                  {cycle.label}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid sx={{ display: "flex", justifyContent: "flex-end" }}>
            <LoadingButton
              type="submit"
              variant="contained"
              startIcon={<MdSave />}
              loading={loading}
              disabled={!isDirty || loading}
            >
              Zapisz zmiany
            </LoadingButton>
          </Grid>
        </Grid>
      </Paper>
    </Box>
  );
};

export default SettingsCyclesPage;
