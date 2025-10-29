import React, { useState, useEffect } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  Typography,
  TextField,
  Box,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
} from "@mui/material";
import type { SaleEntry } from "../../../../models/sales/sales-entry";
import type { SaleListModel } from "../../../../models/sales/sales";
import { SalesService } from "../../../../services/sales-service";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import LoadingButton from "../../../common/loading-button";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";
import LoadingTextField from "../../../common/loading-textfield";
import FilePreview from "../../../common/file-preview";
import { MdAttachFile } from "react-icons/md";

interface EditSaleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  sale: SaleListModel | null;
}

const EditSaleModal: React.FC<EditSaleModalProps> = ({
  open,
  onClose,
  onSave,
  sale,
}) => {
  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [form, setForm] = useState<SaleListModel | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [errors, setErrors] = useState<{
    [key: string]: any;
    otherExtras?: { name?: string; value?: string }[];
  }>({});

  useEffect(() => {
    if (sale && open) {
      setForm({ ...sale });
      setErrors({});
      setSelectedFile(null);
    }
  }, [sale, open]);

  useEffect(() => {
    const fetchCycles = async (farmId: string) => {
      if (!farmId) {
        setCycles([]);
        return;
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
        "Nie udało się pobrać cykli dla wybranej fermy."
      );
      setLoadingCycles(false);
    };

    if (form?.farmId) {
      fetchCycles(form.farmId);
    }
  }, [form?.farmId]);

  const handleChange = (name: keyof SaleEntry, value: any) => {
    if (!form) return;
    setForm({ ...form, [name]: value });
    setErrors((prev) => ({ ...prev, [name]: undefined }));
  };

  const handleExtraChange = (
    index: number,
    field: "name" | "value",
    value: string
  ) => {
    if (!form) return;
    const updatedExtras = [...form.otherExtras];
    updatedExtras[index] = { ...updatedExtras[index], [field]: value };
    setForm({ ...form, otherExtras: updatedExtras });
  };

  const addExtra = () => {
    if (!form) return;
    setForm({
      ...form,
      otherExtras: [...form.otherExtras, { name: "", value: 0 }],
    });
  };

  const removeExtra = (index: number) => {
    if (!form) return;
    const updatedExtras = [...form.otherExtras];
    updatedExtras.splice(index, 1);
    setForm({ ...form, otherExtras: updatedExtras });
  };

  const validate = (): boolean => {
    if (!form) return false;

    const newErrors: typeof errors = {};

    if (!form.saleDate) newErrors.saleDate = "Wymagana data sprzedaży";
    if (!form.cycleId) newErrors.cycleId = "Cykl jest wymagany";

    if (Number(form.quantity) <= 0) newErrors.quantity = "Wymagana wartość > 0";
    if (Number(form.weight) <= 0) newErrors.weight = "Wymagana wartość > 0";

    if (Number(form.confiscatedCount) < 0)
      newErrors.confiscatedCount = "Nie może być ujemna";
    if (Number(form.confiscatedWeight) < 0)
      newErrors.confiscatedWeight = "Nie może być ujemna";

    if (Number(form.deadCount) < 0) newErrors.deadCount = "Nie może być ujemna";
    if (Number(form.deadWeight) < 0)
      newErrors.deadWeight = "Nie może być ujemna";

    if (Number(form.farmerWeight) < 0)
      newErrors.farmerWeight = "Nie może być ujemna";

    if (Number(form.basePrice) < 0) newErrors.basePrice = "Nie może być ujemna";
    if (Number(form.priceWithExtras) < 0)
      newErrors.priceWithExtras = "Nie może być ujemna";

    const otherExtrasErrors =
      form.otherExtras?.map((extra) => {
        const err: Record<string, string> = {};
        if (!extra.name?.trim()) err.name = "Wymagana nazwa";
        if (Number(extra.value) < 0) err.value = "Nie może być ujemna";
        return err;
      }) || [];

    if (otherExtrasErrors.some((e) => Object.keys(e).length > 0)) {
      (newErrors as any).otherExtras = otherExtrasErrors;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!form || !form.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        SalesService.updateSale(
          form.id,
          {
            cycleId: form.cycleId,
            saleDate: form.saleDate,
            quantity: Number(form.quantity),
            weight: Number(form.weight),
            confiscatedCount: Number(form.confiscatedCount),
            confiscatedWeight: Number(form.confiscatedWeight),
            deadCount: Number(form.deadCount),
            deadWeight: Number(form.deadWeight),
            farmerWeight: Number(form.farmerWeight),
            basePrice: Number(form.basePrice),
            priceWithExtras: Number(form.priceWithExtras),
            comment: form.comment,
            otherExtras: form.otherExtras.map((extra) => ({
              name: extra.name,
              value: Number(extra.value),
            })),
          },
          selectedFile || undefined
        ),
      () => {
        toast.success("Zaktualizowano sprzedaż");
        onSave();
      },
      undefined,
      "Nie udało się zapisać zmian"
    );
    setLoading(false);

    onSave();
  };

  if (!form) return null;

  return (
    <AppDialog
      open={open}
      onClose={onClose}
      maxWidth={!form.directoryPath ? "lg" : "md"}
      fullWidth
    >
      <DialogTitle>Edycja pozycji sprzedaży</DialogTitle>
      <DialogContent dividers>
        <Grid container spacing={2} sx={{ mt: 1 }}>
          {!form.directoryPath && (
            <>
              {selectedFile && (
                <Grid size={12}>
                  <Box mb={2}>
                    <Typography variant="subtitle2" gutterBottom>
                      Podgląd nowego pliku
                    </Typography>
                    <FilePreview
                      file={selectedFile}
                      maxHeight={400}
                      showPreviewButton={false}
                    />
                  </Box>
                </Grid>
              )}

              <Grid size={12}>
                <Button
                  variant="outlined"
                  component="label"
                  startIcon={<MdAttachFile />}
                  fullWidth
                >
                  {selectedFile ? "Zmień plik" : "Dodaj plik"}
                  <input
                    type="file"
                    hidden
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) {
                        setSelectedFile(file);
                      }
                    }}
                  />
                </Button>
                {selectedFile && (
                  <Box
                    mt={1}
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                  >
                    <Typography variant="body2">{selectedFile.name}</Typography>
                    <Button
                      size="small"
                      color="error"
                      onClick={() => setSelectedFile(null)}
                    >
                      Usuń
                    </Button>
                  </Box>
                )}
              </Grid>
            </>
          )}

          <Grid size={12}>
            <TextField label="Ferma" fullWidth value={form.farmName} disabled />
          </Grid>
          <Grid size={12}>
            <LoadingTextField
              label="Cykl"
              select
              fullWidth
              loading={loadingCycles}
              value={form.cycleId}
              onChange={(e) =>
                setForm((f) => (f ? { ...f, cycleId: e.target.value } : null))
              }
              error={!!errors.cycleId}
              helperText={errors.cycleId}
              disabled={!form.farmId || loadingCycles || cycles.length === 0}
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid sx={{ xs: 12, sm: 4 }}>
            <DatePicker
              label="Data wstawienia"
              value={dayjs(form.saleDate)}
              onChange={(value) =>
                setForm((f) =>
                  f
                    ? {
                        ...f,
                        saleDate: value?.format("YYYY-MM-DD") || "",
                        id: f.id,
                      }
                    : null
                )
              }
              disableFuture
              format="DD.MM.YYYY"
              slotProps={{
                textField: {
                  error: !!errors.insertionDate,
                  helperText: errors.insertionDate,
                  fullWidth: true,
                },
              }}
            />
          </Grid>
          <Grid size={12}>
            <Typography variant="h6">Dane produkcyjne</Typography>
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Sztuki"
              type="number"
              value={form.quantity}
              onChange={(e) => handleChange("quantity", Number(e.target.value))}
              error={!!errors.quantity}
              helperText={errors.quantity}
              fullWidth
            />
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Waga (kg)"
              type="number"
              value={form.weight}
              onChange={(e) => handleChange("weight", Number(e.target.value))}
              error={!!errors.weight}
              helperText={errors.weight}
              fullWidth
            />
          </Grid>

          <Grid size={12}>
            <Typography variant="h6">Konfiskata</Typography>
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Sztuki"
              type="number"
              value={form.confiscatedCount}
              onChange={(e) =>
                handleChange("confiscatedCount", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Waga (kg)"
              type="number"
              value={form.confiscatedWeight}
              onChange={(e) =>
                handleChange("confiscatedWeight", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid size={12}>
            <Typography variant="h6">Martwe</Typography>
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Sztuki"
              type="number"
              value={form.deadCount}
              onChange={(e) =>
                handleChange("deadCount", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid sx={{ xs: 12, sm: 3 }}>
            <TextField
              label="Waga (kg)"
              type="number"
              value={form.deadWeight}
              onChange={(e) =>
                handleChange("deadWeight", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid size={12} />

          <Grid size={6}>
            <TextField
              label="Waga hodowcy (kg)"
              type="number"
              value={form.farmerWeight}
              onChange={(e) =>
                handleChange("farmerWeight", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid size={12}>
            <Typography variant="h6">Dane finansowe</Typography>
          </Grid>

          <Grid sx={{ xs: 12, sm: 6 }}>
            <TextField
              label="Cena bazowa (zł)"
              type="number"
              value={form.basePrice}
              onChange={(e) =>
                handleChange("basePrice", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid sx={{ xs: 12, sm: 6 }}>
            <TextField
              label="Cena z dodatkami (zł)"
              type="number"
              value={form.priceWithExtras}
              onChange={(e) =>
                handleChange("priceWithExtras", Number(e.target.value))
              }
              fullWidth
            />
          </Grid>

          <Grid size={12}>
            <Typography variant="h6">Inne dodatki</Typography>
          </Grid>

          {form.otherExtras.map((extra, idx) => (
            <Grid size={12} key={idx}>
              <Box display="flex" gap={2} mb={1}>
                <FormControl
                  fullWidth
                  error={!!errors.otherExtras?.[idx]?.name}
                >
                  <InputLabel id={`extra-name-${idx}`}>Nazwa</InputLabel>
                  <Select
                    labelId={`extra-name-${idx}`}
                    value={extra.name}
                    label="Nazwa"
                    onChange={(e) =>
                      handleExtraChange(idx, "name", e.target.value)
                    }
                  >
                    {sale?.otherExtras.map((field) => (
                      <MenuItem key={field.name} value={field.name}>
                        {field.name}
                      </MenuItem>
                    ))}
                  </Select>
                  {errors.otherExtras?.[idx]?.name && (
                    <Typography variant="caption" color="error">
                      {errors.otherExtras[idx].name}
                    </Typography>
                  )}
                </FormControl>

                <TextField
                  label="Wartość (zł)"
                  type="number"
                  value={extra.value}
                  onChange={(e) =>
                    handleExtraChange(idx, "value", e.target.value)
                  }
                  fullWidth
                  error={!!errors.otherExtras?.[idx]?.value}
                  helperText={errors.otherExtras?.[idx]?.value}
                />

                <Button color="error" onClick={() => removeExtra(idx)}>
                  Usuń
                </Button>
              </Box>
            </Grid>
          ))}

          <Grid sx={{ xs: 12 }}>
            <Button variant="outlined" onClick={addExtra}>
              Dodaj dodatek
            </Button>
          </Grid>

          <Grid size={12}>
            <TextField
              label="Komentarz"
              multiline
              fullWidth
              value={form.comment}
              onChange={(e) => handleChange("comment", e.target.value)}
            />
          </Grid>

          {!form.directoryPath && (
            <>
              {selectedFile && (
                <Grid size={12}>
                  <Box mb={2}>
                    <Typography variant="subtitle2" gutterBottom>
                      Podgląd nowego pliku
                    </Typography>
                    <FilePreview
                      file={selectedFile}
                      maxHeight={400}
                      showPreviewButton={false}
                    />
                  </Box>
                </Grid>
              )}

              <Grid size={12}>
                <Button
                  variant="outlined"
                  component="label"
                  startIcon={<MdAttachFile />}
                  fullWidth
                  sx={{ mt: 2 }}
                >
                  {selectedFile ? "Zmień plik" : "Dodaj plik"}
                  <input
                    type="file"
                    hidden
                    onChange={(e) => {
                      const file = e.target.files?.[0];
                      if (file) {
                        setSelectedFile(file);
                      }
                    }}
                  />
                </Button>
                {selectedFile && (
                  <Box
                    mt={1}
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                  >
                    <Typography variant="body2">{selectedFile.name}</Typography>
                    <Button
                      size="small"
                      color="error"
                      onClick={() => setSelectedFile(null)}
                    >
                      Usuń
                    </Button>
                  </Box>
                )}
              </Grid>
            </>
          )}
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Anuluj</Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          startIcon={<MdSave />}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditSaleModal;
