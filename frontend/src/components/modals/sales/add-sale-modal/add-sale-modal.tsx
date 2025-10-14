import React, { useEffect, useReducer, useState } from "react";
import {
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
  Tooltip,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useFarms } from "../../../../hooks/useFarms";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { SaleEntryErrors } from "../../../../models/sales/sales-entry";
import { SaleType, SaleTypeLabels } from "../../../../models/sales/sales";
import { formReducer, initialState } from "./sale-form-reducer";
import { validateForm } from "./validation/validate-form";
import { useSlaughterhouses } from "../../../../hooks/useSlaughterhouses";
import { MdSave } from "react-icons/md";
import { useSaleFieldsExtra } from "../../../../hooks/sales/useSaleFieldsExtra";
import SaleEntriesSection from "./sale-entries";
import SaleEntriesTable from "./sale-entries-table";
import { validateEntry } from "./validation/validate-entry";
import type { SaleFormErrors } from "../../../../models/sales/sale-form-states";
import { toast } from "react-toastify";
import { SalesService } from "../../../../services/sales-service";
import FilePreview from "../../../common/file-preview";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

interface AddSaleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

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
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [loading, setLoading] = useState(false);
  const [form, dispatch] = useReducer(formReducer, initialState);
  const [errors, setErrors] = useState<SaleFormErrors>({});
  const [sendToIrz, setSendToIrz] = useState(false);
  const [entriesTableReady, setEntriesTableReady] = useState<number[]>([]);
  const [activePreviewFile, setActivePreviewFile] = useState<File | null>(null);

  useEffect(() => {
    const handlePaste = (event: ClipboardEvent) => {
      const items = event.clipboardData?.items;
      if (!items) return;

      const newFiles: File[] = [];

      for (const item of items) {
        if (item.kind === "file") {
          const file = item.getAsFile();
          if (file) {
            newFiles.push(file);
          }
        }
      }

      if (newFiles.length > 0) {
        dispatch({
          type: "SET_FILES",
          files: [...form.files, ...newFiles],
        });
        setActivePreviewFile(newFiles[0]);
      }
    };

    if (open) {
      window.addEventListener("paste", handlePaste);
    }

    return () => {
      window.removeEventListener("paste", handlePaste);
    };
  }, [open, form.files]);

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
    dispatch({ type: "RESET" });
    dispatch({ type: "SET_FIELD", name: "farmId", value: farmId });
    setHenhouses(farms.find((f) => f.id === farmId)?.henhouses || []);
    setCycles([]);
    setErrors({});

    const selectedFarm = farms.find((f) => f.id === farmId);
    if (selectedFarm?.activeCycle) {
      dispatch({
        type: "SET_FIELD",
        name: "identifierId",
        value: selectedFarm.activeCycle.id,
      });
    }

    setLoadingCycles(true);
    await handleApiResponse(
      () => FarmsService.getFarmCycles(farmId),
      (data) => {
        setCycles(data.responseData ?? []);
      },
      () => {
        setCycles([]);
      },
      "Nie udało się pobrać listy cykli."
    );
    setLoadingCycles(false);
    dispatch({ type: "ADD_ENTRY" });
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

    await handleApiResponse(
      () =>
        SalesService.addNewSale({
          saleType: form.saleType!,
          farmId: form.farmId,
          cycleId: form.identifierId,
          slaughterhouseId: form.slaughterhouseId,
          saleDate: form.saleDate!.format("YYYY-MM-DD"),
          entries: form.entries.map((entry) => ({
            henhouseId: entry.henhouseId,
            basePrice: Number(entry.basePrice),
            priceWithExtras: Number(entry.priceWithExtras),
            comment: entry.comment || undefined,
            otherExtras: entry.otherExtras.map((extra) => ({
              name: extra.name,
              value: Number(extra.value),
            })),
            quantity: Number(entry.quantity),
            weight: Number(entry.weight),
            confiscatedCount: Number(entry.confiscatedCount),
            confiscatedWeight: Number(entry.confiscatedWeight),
            deadCount: Number(entry.deadCount),
            deadWeight: Number(entry.deadWeight),
            farmerWeight: Number(entry.farmerWeight),
          })),
          files: form.files,
        }),
      async (data) => {
        if (!data || !data.responseData || !data.responseData.internalGroupId) {
          toast.error(
            "Nie udało się dodać sprzedaży, brak ID grupy wewnętrznej"
          );
        }

        if (sendToIrz) {
          await handleApiResponse(
            () =>
              SalesService.sendToIrzPlus({
                internalGroupId: data.responseData!.internalGroupId,
              }),
            () => {
              toast.success("Sprzedaż wysłana do IRZplus");
            },
            undefined,
            "Nie udało się wysłać sprzedaży do IRZplus"
          );
        }

        toast.success("Dodano sprzedaż");
        dispatch({ type: "RESET" });
        setErrors({});
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się dodać sprzedaży"
    );

    setLoading(false);
  };

  const handleClose = () => {
    setActivePreviewFile(null);
    onClose();
    setErrors({});
    setHenhouses([]);
    dispatch({ type: "RESET" });
  };

  const selectedSlaughterhouse = slaughterhouses.find(
    (s) => s.id === form.slaughterhouseId
  );
  const slaughterhouseTooltip = selectedSlaughterhouse
    ? `Numer IRZplus: ${selectedSlaughterhouse.producerNumber}`
    : "";

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="lg">
      <DialogTitle>Nowa sprzedaż</DialogTitle>
      <DialogContent>
        <Grid container spacing={2} mt={1}>
          {activePreviewFile && (
            <Grid
              size={{ xs: 12, md: 5 }}
              sx={{
                display: "flex",
                flexDirection: "column",
                height: "100%",
                pr: 2,
              }}
            >
              <Typography variant="subtitle2" gutterBottom>
                Podgląd pliku
              </Typography>

              <FilePreview file={activePreviewFile} maxHeight={900} />
            </Grid>
          )}

          <Grid size={{ xs: 12, md: activePreviewFile ? 7 : 12 }}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 6 }}>
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

              <Grid size={{ xs: 12, sm: 6 }}>
                <LoadingTextField
                  loading={loadingCycles}
                  fullWidth
                  label="Cykl"
                  select
                  value={form.identifierId}
                  onChange={(e) =>
                    dispatch({
                      type: "SET_FIELD",
                      name: "identifierId",
                      value: e.target.value,
                    })
                  }
                  error={!!errors.identifierId}
                  helperText={errors.identifierId}
                  disabled={
                    !form.farmId || loadingCycles || cycles.length === 0
                  }
                >
                  {cycles.map((cycle) => (
                    <MenuItem key={cycle.id} value={cycle.id}>
                      {`${cycle.identifier}/${cycle.year}`}
                    </MenuItem>
                  ))}
                </LoadingTextField>
              </Grid>

              <Grid size={{ xs: 12, sm: 6 }}>
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

              <Grid size={{ xs: 12, sm: 6 }}>
                <Tooltip title={slaughterhouseTooltip}>
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
                      setErrors((prev) => ({
                        ...prev,
                        slaughterhouseId: undefined,
                      }));
                    }}
                    error={!!errors.slaughterhouseId}
                    helperText={errors.slaughterhouseId}
                  >
                    {slaughterhouses.map((slaughterhouse) => (
                      <MenuItem
                        key={slaughterhouse.id}
                        value={slaughterhouse.id}
                      >
                        {slaughterhouse.name}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                </Tooltip>
              </Grid>

              <Grid size={{ xs: 12, sm: 6 }}>
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
              <Grid size={{ xs: 12 }}>
                {entriesTableReady.length > 0 && (
                  <SaleEntriesTable
                    entries={form.entries}
                    indexes={entriesTableReady}
                    onRemove={(indexToRemove) => {
                      dispatch({ type: "REMOVE_ENTRY", index: indexToRemove });
                      setEntriesTableReady((prev) =>
                        prev.filter((i) => i !== indexToRemove)
                      );
                      setErrors((prev) => {
                        const updated = { ...(prev.entries || {}) };
                        delete updated[indexToRemove];
                        return { ...prev, entries: updated };
                      });
                    }}
                    onEdit={(indexToEdit) => {
                      setEntriesTableReady((prev) =>
                        prev.filter((i) => i !== indexToEdit)
                      );
                    }}
                  />
                )}
              </Grid>

              <Grid size={12}>
                <SaleEntriesSection
                  dispatch={dispatch}
                  entries={form.entries}
                  henhouses={henhouses}
                  saleFieldsExtra={saleFieldsExtra}
                  setErrors={setErrors}
                  errors={errors.entries}
                  setEntryErrors={setEntryErrors}
                  farmId={form.farmId}
                  entriesTableReady={entriesTableReady}
                />
              </Grid>

              <Grid size={8} />
              <Grid size={4}>
                <Button
                  fullWidth
                  variant="outlined"
                  onClick={() => {
                    const lastIndex = form.entries.length - 1;
                    const lastEntry = form.entries[lastIndex];
                    const validation = validateEntry(lastEntry);

                    const isValid =
                      !validation || Object.keys(validation).length === 0;

                    if (!entriesTableReady.includes(lastIndex) && isValid) {
                      setEntriesTableReady((prev) => [...prev, lastIndex]);
                      setErrors((prev) => {
                        const newEntriesErrors = { ...(prev.entries || {}) };
                        delete newEntriesErrors[lastIndex];
                        return { ...prev, entries: newEntriesErrors };
                      });
                    } else if (entriesTableReady.includes(lastIndex)) {
                      dispatch({ type: "ADD_ENTRY" });
                      setErrors((prev) => {
                        const newEntriesErrors = { ...(prev.entries || {}) };
                        const newEntryIndex = form.entries.length;
                        delete newEntriesErrors[newEntryIndex];
                        return {
                          ...prev,
                          entries: newEntriesErrors,
                          entriesGeneral: undefined,
                        };
                      });
                    } else {
                      setEntryErrors(lastIndex, validation);
                    }
                  }}
                >
                  Dodaj pozycję
                </Button>
              </Grid>

              <Grid size={{ xs: 12 }}>
                <Button
                  variant="outlined"
                  component="label"
                  fullWidth
                  sx={{ mt: 2 }}
                >
                  Wybierz pliki lub wklej (Ctrl+V)
                  <input
                    type="file"
                    multiple
                    hidden
                    onChange={(e) => {
                      const files = Array.from(e.target.files || []);
                      dispatch({ type: "SET_FILES", files });
                      setActivePreviewFile(files[0] || null);
                    }}
                  />
                </Button>

                {form.files.length > 0 && (
                  <Grid container spacing={1} mt={1}>
                    {form.files.map((file, idx) => (
                      <Grid
                        container
                        key={idx}
                        alignItems="center"
                        spacing={1}
                        size={{ xs: 12 }}
                      >
                        <Grid size={{ xs: 3 }}>
                          <Button
                            variant="text"
                            size="small"
                            onClick={() => setActivePreviewFile(file)}
                          >
                            Podgląd
                          </Button>
                        </Grid>

                        <Grid size={{ xs: 3 }}>
                          <Button
                            variant="text"
                            size="small"
                            color="error"
                            onClick={() => {
                              dispatch({
                                type: "SET_FILES",
                                files: form.files.filter((_, i) => i !== idx),
                              });

                              if (activePreviewFile?.name === file.name) {
                                setActivePreviewFile(null);
                              }
                            }}
                          >
                            Usuń
                          </Button>
                        </Grid>

                        <Grid size={{ xs: 6 }}>
                          <Typography variant="body2">{file.name}</Typography>
                        </Grid>
                      </Grid>
                    ))}
                  </Grid>
                )}
              </Grid>
            </Grid>
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
          onClick={handleSave}
          variant="contained"
          color="primary"
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default AddSaleModal;
