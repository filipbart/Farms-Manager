import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
  Grid,
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, useForm, useFieldArray } from "react-hook-form";
import { MdDelete, MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { FeedsService } from "../../../../services/feeds-service";
import LoadingButton from "../../../common/loading-button";
import LoadingTextField from "../../../common/loading-textfield";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddFeedPriceFormData } from "../../../../models/feeds/prices/feed-price";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface AddFeedPriceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddFeedPriceModal: React.FC<AddFeedPriceModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const {
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<AddFeedPriceFormData>({
    defaultValues: {
      farmId: "",
      identifierId: "",
      priceDate: "",
      entries: [{ nameId: "", price: undefined }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: "entries",
  });

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  const handleSave = async (data: AddFeedPriceFormData) => {
    if (loading) return;
    setLoading(true);
    await handleApiResponse(
      () => FeedsService.addFeedPrice(data),
      () => {
        onSave();
        close();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania ceny paszy"
    );
    setLoading(false);
  };

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchFeedsNames();
    }
  }, [open, fetchFarms, fetchFeedsNames]);

  const watchedFarmId = watch("farmId");

  useEffect(() => {
    const fetchCyclesForFarm = async (farmId: string) => {
      setLoadingCycles(true);

      const selectedFarm = farms.find((f) => f.id === farmId);
      if (selectedFarm?.activeCycle) {
        setValue("identifierId", selectedFarm.activeCycle.id);
      } else {
        setValue("identifierId", "");
      }

      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        () => setCycles([]),
        "Nie udało się pobrać listy cykli."
      );
      setLoadingCycles(false);
    };

    if (watchedFarmId && farms.length > 0) {
      fetchCyclesForFarm(watchedFarmId);
    } else {
      setCycles([]);
      setValue("identifierId", "");
    }
  }, [watchedFarmId, farms, setValue]);

  const close = () => {
    reset();
    onClose();
  };

  const watchedEntries = watch("entries");

  return (
    <AppDialog open={open} onClose={close} fullWidth maxWidth="md">
      <DialogTitle>Wprowadź nowe ceny pasz</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="farmId"
                control={control}
                rules={{ required: "Ferma jest wymagana" }}
                render={({ field }) => (
                  <LoadingTextField
                    {...field}
                    loading={loadingFarms}
                    select
                    label="Ferma"
                    fullWidth
                    error={!!errors.farmId}
                    helperText={errors.farmId?.message}
                    onChange={async (e) => {
                      field.onChange(e);
                    }}
                  >
                    {farms.map((farm) => (
                      <MenuItem key={farm.id} value={farm.id}>
                        {farm.name}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="identifierId"
                control={control}
                rules={{ required: "Cykl jest wymagany" }}
                render={({ field }) => (
                  <LoadingTextField
                    {...field}
                    loading={loadingCycles}
                    label="Cykl"
                    select
                    fullWidth
                    disabled={!watchedFarmId || cycles.length === 0}
                    error={!!errors.identifierId}
                    helperText={errors.identifierId?.message}
                  >
                    {cycles.map((cycle) => (
                      <MenuItem key={cycle.id} value={cycle.id}>
                        {`${cycle.identifier}/${cycle.year}`}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
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
                      field.onChange(
                        date ? dayjs(date).format("YYYY-MM-DD") : ""
                      )
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
            </Grid>
          </Grid>
          <TableContainer component={Paper} variant="outlined" sx={{ mt: 3 }}>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Typ (nazwa) paszy</TableCell>
                  <TableCell align="right">Cena [zł]</TableCell>
                  <TableCell align="center" sx={{ width: "60px" }}>
                    Akcje
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {fields.map((item, index) => {
                  const selectedNameIds =
                    watchedEntries?.map((e) => e.nameId) ?? [];
                  return (
                    <TableRow key={item.id}>
                      <TableCell>
                        <Controller
                          name={`entries.${index}.nameId`}
                          control={control}
                          rules={{ required: "Nazwa jest wymagana" }}
                          render={({ field }) => (
                            <LoadingTextField
                              {...field}
                              loading={loadingFeedsNames}
                              select
                              fullWidth
                              size="small"
                              error={!!errors.entries?.[index]?.nameId}
                              helperText={
                                errors.entries?.[index]?.nameId?.message
                              }
                            >
                              {feedsNames
                                .filter(
                                  (feed) =>
                                    !selectedNameIds.includes(feed.id) ||
                                    feed.id === watchedEntries[index]?.nameId
                                )
                                .map((feedName) => (
                                  <MenuItem
                                    key={feedName.id}
                                    value={feedName.id}
                                  >
                                    {feedName.name}
                                  </MenuItem>
                                ))}
                            </LoadingTextField>
                          )}
                        />
                      </TableCell>
                      <TableCell align="right">
                        <Controller
                          name={`entries.${index}.price`}
                          control={control}
                          rules={{
                            required: "Cena jest wymagana",
                            min: { value: 0, message: "Błędna wartość" },
                          }}
                          render={({ field }) => (
                            <TextField
                              {...field}
                              type="number"
                              size="small"
                              sx={{ width: 120 }}
                              inputMode="decimal"
                              InputProps={{
                                inputProps: { min: 0, step: "0.01" },
                              }}
                              error={!!errors.entries?.[index]?.price}
                              helperText={
                                errors.entries?.[index]?.price?.message
                              }
                            />
                          )}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <IconButton
                          color="error"
                          onClick={() => remove(index)}
                          disabled={fields.length <= 1}
                        >
                          <MdDelete />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </TableContainer>
          <Box mt={1}>
            <Button
              variant="text"
              onClick={() => append({ nameId: "", price: undefined })}
            >
              + Dodaj pozycję
            </Button>
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
