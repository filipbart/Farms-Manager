import {
  DialogTitle,
  DialogContent,
  Grid,
  Box,
  Typography,
  TextField,
  Divider,
  DialogActions,
  Button,
  MenuItem,
} from "@mui/material";
import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import type {
  ProductionDataTransferFeedListModel,
  UpdateTransferFeedData,
} from "../../../../models/production-data/transfer-feed";
import { ProductionDataTransferFeedService } from "../../../../services/production-data/production-data-transfer-feed-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface EditProductionDataTransferFeedModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  feedTransfer: ProductionDataTransferFeedListModel | null;
}

type FormDataType = UpdateTransferFeedData & {
  fromCycleId: string;
  toCycleId: string;
};

const EditProductionDataTransferFeedModal: React.FC<
  EditProductionDataTransferFeedModalProps
> = ({ open, onClose, onSave, feedTransfer }) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    watch,
  } = useForm<FormDataType>();

  const [loading, setLoading] = useState(false);
  const [fromCycles, setFromCycles] = useState<CycleDto[]>([]);
  const [toCycles, setToCycles] = useState<CycleDto[]>([]);
  const [loadingFromCycles, setLoadingFromCycles] = useState(false);
  const [loadingToCycles, setLoadingToCycles] = useState(false);

  const fromCycleId = watch("fromCycleId");
  const toCycleId = watch("toCycleId");

  useEffect(() => {
    if (feedTransfer && open) {
      reset({
        fromCycleId: feedTransfer.fromCycleId,
        toCycleId: feedTransfer.toCycleId,
        tonnage: feedTransfer.tonnage,
        value: feedTransfer.value,
      });

      const fetchFromCycles = async () => {
        if (!feedTransfer.fromFarmId) return;
        setLoadingFromCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(feedTransfer.fromFarmId),
          (data) => setFromCycles(data.responseData ?? []),
          () => setFromCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingFromCycles(false);
      };

      const fetchToCycles = async () => {
        if (!feedTransfer.toFarmId) return;
        setLoadingToCycles(true);
        await handleApiResponse(
          () => FarmsService.getFarmCycles(feedTransfer.toFarmId),
          (data) => setToCycles(data.responseData ?? []),
          () => setToCycles([]),
          "Nie udało się pobrać listy cykli."
        );
        setLoadingToCycles(false);
      };

      fetchFromCycles();
      fetchToCycles();
    }
  }, [feedTransfer, open, reset]);

  const handleSave = async (data: FormDataType) => {
    if (!feedTransfer) return;
    setLoading(true);
    await handleApiResponse(
      () =>
        ProductionDataTransferFeedService.updateFeedTransfer(feedTransfer.id, {
          fromCycleId: data.fromCycleId,
          toCycleId: data.toCycleId,
          tonnage: data.tonnage,
          value: data.value,
        }),
      () => {
        toast.success("Pomyślnie zaktualizowano przeniesienie paszy");
        onSave();
        onClose();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji danych"
    );
    setLoading(false);
  };

  const close = () => {
    reset();
    setFromCycles([]);
    setToCycles([]);
    onClose();
  };

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="lg">
      <DialogTitle>Edytuj przeniesienie paszy</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={4} sx={{ mt: 0.5 }}>
            {/* --- SEKCJA Z (tylko do odczytu) --- */}
            <Grid size={{ xs: 12, md: 6 }}>
              <Box display="flex" flexDirection="column" gap={2.5}>
                <Typography variant="h6">Przeniesiono Z</Typography>
                <TextField
                  label="Ferma"
                  value={feedTransfer?.fromFarmName || ""}
                  slotProps={{ input: { readOnly: true } }}
                  fullWidth
                />
                <LoadingTextField
                  loading={loadingFromCycles}
                  label="Cykl"
                  select
                  fullWidth
                  disabled={
                    !feedTransfer?.fromFarmId ||
                    loadingFromCycles ||
                    fromCycles.length === 0
                  }
                  value={fromCycleId || ""}
                  error={!!errors.fromCycleId}
                  helperText={errors.fromCycleId?.message}
                  {...register("fromCycleId", {
                    required: "Cykl jest wymagany",
                  })}
                >
                  {fromCycles.map((cycle) => (
                    <MenuItem key={cycle.id} value={cycle.id}>
                      {`${cycle.identifier}/${cycle.year}`}
                    </MenuItem>
                  ))}
                </LoadingTextField>
                <TextField
                  label="Kurnik"
                  value={feedTransfer?.fromHenhouseName || ""}
                  slotProps={{ input: { readOnly: true } }}
                  fullWidth
                />
              </Box>
            </Grid>

            {/* --- SEKCJA DO (tylko do odczytu) --- */}
            <Grid size={{ xs: 12, md: 6 }}>
              <Box display="flex" flexDirection="column" gap={2.5}>
                <Typography variant="h6">Przeniesiono Do</Typography>
                <TextField
                  label="Ferma"
                  value={feedTransfer?.toFarmName || ""}
                  slotProps={{ input: { readOnly: true } }}
                  fullWidth
                />
                <LoadingTextField
                  loading={loadingToCycles}
                  label="Cykl"
                  select
                  fullWidth
                  disabled={
                    !feedTransfer?.toFarmId ||
                    loadingToCycles ||
                    toCycles.length === 0
                  }
                  value={toCycleId || ""}
                  error={!!errors.toCycleId}
                  helperText={errors.toCycleId?.message}
                  {...register("toCycleId", { required: "Cykl jest wymagany" })}
                >
                  {toCycles.map((cycle) => (
                    <MenuItem key={cycle.id} value={cycle.id}>
                      {`${cycle.identifier}/${cycle.year}`}
                    </MenuItem>
                  ))}
                </LoadingTextField>
                <TextField
                  label="Kurnik"
                  value={feedTransfer?.toHenhouseName || ""}
                  slotProps={{ input: { readOnly: true } }}
                  fullWidth
                />
              </Box>
            </Grid>

            {/* --- SEKCJA WSPÓLNA (edycja) --- */}
            <Grid size={{ xs: 12 }}>
              <Divider sx={{ my: 2 }} />
            </Grid>

            <Grid container size={{ xs: 12 }} spacing={2.5}>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField
                  label="Typ (nazwa) paszy"
                  value={feedTransfer?.feedName || ""}
                  slotProps={{ input: { readOnly: true } }}
                  fullWidth
                />
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
                <TextField
                  label="Wartość [zł]"
                  type="number"
                  slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
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

export default EditProductionDataTransferFeedModal;
