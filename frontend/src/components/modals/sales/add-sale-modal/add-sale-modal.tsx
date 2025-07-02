import React, { useEffect, useReducer, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  MenuItem,
  Typography,
  Checkbox,
  FormControlLabel,
  TextField,
  Grid,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useFarms } from "../../../../hooks/useFarms";
import { FarmsService } from "../../../../services/farms-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { SaleEntryErrors } from "../../../../models/sales/sales-entry";
import { SaleType, SaleTypeLabels } from "../../../../models/sales/sales";
import { formReducer } from "./sale-form-reducer";
import type { SaleFormErrors, SaleFormState } from "./sale-form-types";
import { validateForm } from "./validation/validate-form";
import { useSlaughterhouses } from "../../../../hooks/useSlaughterhouses";
import { MdSave } from "react-icons/md";
import { useSaleFieldsExtra } from "../../../../hooks/sales/useSaleFieldsExtra";
import SaleEntriesSection from "./sale-entries";

interface AddSaleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const initialState: SaleFormState = {
  saleType: undefined,
  farmId: "",
  slaughterhouseId: "",
  identifierId: "",
  identifierDisplay: "",
  saleDate: null,
  entries: [],
};

const AddSaleModal: React.FC<AddSaleModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { slaughterhouses, loadingSlaughterhouses, fetchSlaughterhouses } =
    useSlaughterhouses();
  const { saleFieldsExtra, fetchSaleFieldsExtra } = useSaleFieldsExtra();
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [loadingLatestCycle, setLoadingLatestCycle] = useState(false);
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<SaleFormErrors>({});
  const [sendToIrz, setSendToIrz] = useState(false);

  useEffect(() => {
    fetchFarms();
    fetchSlaughterhouses();
    fetchSaleFieldsExtra();
  }, []);

  useEffect(() => {
    if (open) {
      dispatch({ type: "ADD_ENTRY" });
    }
  }, [open]);

  const handleFarmChange = async (farmId: string) => {
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    dispatch({ type: "SET_FIELD", name: "identifierId", value: "" });
    dispatch({ type: "SET_FIELD", name: "identifierDisplay", value: "" });
    setHenhouses(farms.find((f) => f.id === farmId)?.henhouses || []);
    setErrors({});

    console.log(henhouses);

    setLoadingLatestCycle(true);
    await handleApiResponse(
      () => FarmsService.getLatestCycle(farmId),
      (data) => {
        if (!data?.responseData) {
          setErrors((prev) => ({
            ...prev,
            identifierId: "Brak aktywnego cyklu",
          }));
          return;
        }
        const cycle = data.responseData;
        dispatch({ type: "SET_FIELD", name: "identifierId", value: cycle.id });
        dispatch({
          type: "SET_FIELD",
          name: "identifierDisplay",
          value: `${cycle.identifier}/${cycle.year}`,
        });
      },
      undefined,
      "Nie udało się pobrać ostatniego cyklu"
    );
    setLoadingLatestCycle(false);
  };

  const setEntryErrors = (index: number, entryErrors: SaleEntryErrors) => {
    setErrors((prev) => {
      const newEntriesErrors = { ...(prev.entries || {}) };
      newEntriesErrors[index] = entryErrors;
      return { ...prev, entries: newEntriesErrors };
    });
  };

  const handleSave = async () => {
    const validationErrors = validateForm(form);
    setErrors(validationErrors);

    if (Object.keys(validationErrors).length > 0) return;

    setLoading(true);
    console.log(form);

    // await handleApiResponse(
    //   () =>
    //     SalesService.addNewSale({
    //       saleType: form.saleType!,
    //       farmId: form.farmId,
    //       cycleId: form.identifierId,
    //       saleDate: form.saleDate!.format("YYYY-MM-DD"),
    //       basePrice: Number(form.basePrice),
    //       priceWithExtras: Number(form.priceWithExtras),
    //       comment: form.comment || undefined,
    //       otherExtras: form.otherExtras.map((extra) => ({
    //         name: extra.name,
    //         value: extra.value.toString(),
    //       })),
    //       entries: form.entries.map((entry) => ({
    //         henhouseId: entry.henhouseId,
    //         slaughterhouseId: entry.slaughterhouseId,
    //         quantity: Number(entry.quantity),
    //         weight: Number(entry.weight),
    //         confiscatedCount: Number(entry.confiscatedCount),
    //         confiscatedWeight: Number(entry.confiscatedWeight),
    //         deadCount: Number(entry.deadCount),
    //         deadWeight: Number(entry.deadWeight),
    //         farmerWeight: Number(entry.farmerWeight),
    //       })),
    //     }),
    //   async (data) => {
    //     if (!data || !data.responseData || !data.responseData.internalGroupId) {
    //       toast.error(
    //         "Nie udało się dodać sprzedaży, brak ID grupy wewnętrznej"
    //       );
    //     }

    //     if (sendToIrz) {
    //       // await handleApiResponse(
    //       //   () =>
    //       //     SalesService.sendToIrzPlus({
    //       //       internalGroupId: data.responseData!.internalGroupId,
    //       //     }),
    //       //   () => {
    //       //     toast.success("Wstawienie wysłane do IRZplus");
    //       //   },
    //       //   undefined,
    //       //   "Nie udało się wysłać wstawienia do IRZplus"
    //       // );
    //     }

    //     toast.success("Dodano sprzedaż");
    //     dispatch({ type: "RESET" });
    //     setErrors({});
    //     onSave();
    //     onClose();
    //   },
    //   undefined,
    //   "Nie udało się dodać sprzedaży"
    // );

    setLoading(false);
  };

  const handleClose = () => {
    onClose();
    dispatch({ type: "RESET" });
    setErrors({});
    setHenhouses([]);
  };

  return (
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason !== "backdropClick") {
          handleClose();
        }
      }}
      fullWidth
      maxWidth="xl"
    >
      <DialogTitle>Nowa sprzedaż</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} mt={1}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField
              select
              fullWidth
              label="Typ sprzedaży"
              value={form.saleType}
              onChange={(e) => {
                dispatch({
                  type: "SET_FIELD",
                  name: "saleType",
                  value: e.target.value,
                });
                setErrors((prev) => ({ ...prev, saleType: undefined }));
              }}
              error={!!errors.saleType}
              helperText={errors.saleType}
            >
              {Object.values(SaleType).map((value) => (
                <MenuItem key={value} value={value}>
                  {SaleTypeLabels[value]}
                </MenuItem>
              ))}
            </TextField>
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <LoadingTextField
              loading={loadingFarms}
              select
              fullWidth
              label="Ferma"
              value={form.farmId}
              onChange={(e) => handleFarmChange(e.target.value)}
              error={!!errors.farmId}
              helperText={errors.farmId}
            >
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <LoadingTextField
              loading={loadingLatestCycle}
              fullWidth
              label="Identyfikator"
              value={form.identifierDisplay}
              slotProps={{ input: { readOnly: true } }}
              error={!!errors.identifierId}
              helperText={errors.identifierId}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <LoadingTextField
              loading={loadingSlaughterhouses}
              select
              fullWidth
              label="Ubojnia"
              value={form.slaughterhouseId}
              onChange={(e) => {
                dispatch({
                  type: "SET_FIELD",
                  name: "slaughterhouseId",
                  value: e.target.value,
                });
                setErrors((prev) => ({ ...prev, slaughterhouseId: undefined }));
              }}
              error={!!errors.slaughterhouseId}
              helperText={errors.slaughterhouseId}
            >
              {slaughterhouses.map((slaughterhouse) => (
                <MenuItem key={slaughterhouse.id} value={slaughterhouse.id}>
                  {slaughterhouse.name}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <DatePicker
              label="Data sprzedaży"
              value={form.saleDate}
              onChange={(value) => {
                dispatch({ type: "SET_FIELD", name: "saleDate", value });
                setErrors((prev) => ({ ...prev, saleDate: undefined }));
              }}
              disableFuture
              format="DD.MM.YYYY"
              slotProps={{
                textField: {
                  error: !!errors.saleDate,
                  helperText: errors.saleDate,
                  fullWidth: true,
                },
              }}
            />
          </Grid>
          <Grid size={20}>
            <SaleEntriesSection
              form={form}
              dispatch={dispatch}
              entries={form.entries}
              henhouses={henhouses}
              saleFieldsExtra={saleFieldsExtra}
              setErrors={setErrors}
              errors={errors.entries}
              setEntryErrors={setEntryErrors}
              farmId={form.farmId}
            />
          </Grid>

          <Grid size={20}>
            <Button
              fullWidth
              variant="outlined"
              onClick={() => {
                dispatch({ type: "ADD_ENTRY" });
                setErrors((prev) => ({ ...prev, entriesGeneral: undefined }));
              }}
            >
              Dodaj pozycję
            </Button>
          </Grid>
        </Grid>
      </DialogContent>

      <DialogActions>
        <FormControlLabel
          control={
            <Checkbox
              checked={sendToIrz}
              onChange={() => setSendToIrz(!sendToIrz)}
              color="error"
              sx={{
                "&.MuiCheckbox-root": {
                  color: "error.main",
                },
                "&.Mui-checked": {
                  color: "error.main",
                },
              }}
            />
          }
          label={
            <Typography sx={{ color: "error.main" }}>
              Wyślij do IRZplus
            </Typography>
          }
        />

        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          startIcon={<MdSave />}
          loading={loading}
          loadingSize={20}
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

export default AddSaleModal;
