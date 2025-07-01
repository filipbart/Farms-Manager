import React, { useEffect, useReducer, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  MenuItem,
  Box,
  Typography,
  Checkbox,
  FormControlLabel,
  TextField,
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
import FinancialSection from "./financial-section";
import SaleEntriesTable from "./sale-entries-table";
import { validateForm } from "./validation/validate-form";
import { useSlaughterhouses } from "../../../../hooks/useSlaughterhouses";

interface AddSaleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const initialState: SaleFormState = {
  saleType: undefined,
  farmId: "",
  identifierId: "",
  identifierDisplay: "",
  saleDate: null,
  entries: [],
  basePrice: "",
  priceWithExtras: "",
  comment: "",
  otherExtras: [],
};

const AddSaleModal: React.FC<AddSaleModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { slaughterhouses, loadingSlaughterhouses, fetchSlaughterhouses } =
    useSlaughterhouses();
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [loadingLatestCycle, setLoadingLatestCycle] = useState(false);
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<SaleFormErrors>({});
  const [sendToIrz, setSendToIrz] = useState(false);

  useEffect(() => {
    fetchFarms();
    fetchSlaughterhouses();
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

    // await handleApiResponse(
    //   () =>
    //     SalesService.addNewSale({
    //       farmId: form.farmId,
    //       cycleId: form.identifierId,
    //       saleDate: form.saleDate!.format("YYYY-MM-DD"),
    //       entries: form.entries.map(
    //         ({ henhouseId, hatcheryId, quantity, bodyWeight }) => ({
    //           henhouseId,
    //           hatcheryId,
    //           quantity: Number(quantity),
    //           bodyWeight: Number(bodyWeight),
    //         })
    //       ),
    //     }),
    //   async (data) => {
    //     if (!data || !data.responseData || !data.responseData.internalGroupId) {
    //       toast.error(
    //         "Nie udało się dodać wstawienia, brak ID grupy wewnętrznej"
    //       );
    //     }

    //     if (sendToIrz) {
    //       await handleApiResponse(
    //         () =>
    //           SalesService.sendToIrzPlus({
    //             internalGroupId: data.responseData!.internalGroupId,
    //           }),
    //         () => {
    //           toast.success("Wstawienie wysłane do IRZplus");
    //         },
    //         undefined,
    //         "Nie udało się wysłać wstawienia do IRZplus"
    //       );
    //     }

    //     toast.success("Dodano wstawienie");
    //     dispatch({ type: "RESET" });
    //     setErrors({});
    //     onSave();
    //     onClose();
    //   },
    //   undefined,
    //   "Nie udało się dodać wstawienia"
    // );

    setLoading(false);
  };

  const handleClose = () => {
    dispatch({ type: "RESET" });
    setErrors({});
    setHenhouses([]);
    onClose();
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
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
          <TextField
            select
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
            fullWidth
          >
            {Object.values(SaleType).map((value) => (
              <MenuItem key={value} value={value}>
                {SaleTypeLabels[value]}
              </MenuItem>
            ))}
          </TextField>

          <LoadingTextField
            loading={loadingFarms}
            select
            label="Ferma"
            value={form.farmId}
            onChange={(e) => handleFarmChange(e.target.value)}
            error={!!errors.farmId}
            helperText={errors.farmId}
            fullWidth
          >
            {farms.map((farm) => (
              <MenuItem key={farm.id} value={farm.id}>
                {farm.name}
              </MenuItem>
            ))}
          </LoadingTextField>

          <LoadingTextField
            loading={loadingLatestCycle}
            label="Identyfikator"
            value={form.identifierDisplay}
            slotProps={{ input: { readOnly: true } }}
            error={!!errors.identifierId}
            helperText={errors.identifierId}
            fullWidth
          />

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

          <FinancialSection form={form} dispatch={dispatch} errors={errors} />

          {errors.entriesGeneral && (
            <Box sx={{ mb: 1 }}>
              <Typography color="error">{errors.entriesGeneral}</Typography>
            </Box>
          )}

          <SaleEntriesTable
            entries={form.entries}
            henhouses={henhouses}
            slaughterhouses={slaughterhouses}
            errors={errors.entries}
            dispatch={dispatch}
            setEntryErrors={setEntryErrors}
            loadingSlaughterhouses={loadingSlaughterhouses}
            farmId={form.farmId}
          />

          <Button
            variant="outlined"
            onClick={() => {
              dispatch({ type: "ADD_ENTRY" });
              setErrors((prev) => ({ ...prev, entriesGeneral: undefined }));
            }}
          >
            Dodaj pozycję
          </Button>
        </Box>
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
