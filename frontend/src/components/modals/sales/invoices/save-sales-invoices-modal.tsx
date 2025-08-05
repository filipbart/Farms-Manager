import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  Divider,
  MenuItem,
  DialogActions,
  useTheme,
  useMediaQuery,
  TextField,
  Autocomplete,
} from "@mui/material";
import { useState, useEffect } from "react";
import { MdSave, MdZoomIn } from "react-icons/md";
import { Controller, useForm } from "react-hook-form";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { toast } from "react-toastify";

import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { getFileTypeFromUrl } from "../../../../utils/fileUtils";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { useSlaughterhouses } from "../../../../hooks/useSlaughterhouses";
import type {
  DraftSalesInvoice,
  SalesInvoiceData,
} from "../../../../models/sales/sales-invoices";
import { SalesService } from "../../../../services/sales-service";

interface SaveSalesInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  draftSalesInvoices: DraftSalesInvoice[];
  onSave: (savedDraft: DraftSalesInvoice) => void;
}

const SaveSalesInvoicesModal: React.FC<SaveSalesInvoicesModalProps> = ({
  open,
  onClose: handleClose,
  draftSalesInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [previewOpen, setPreviewOpen] = useState(false);

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const { slaughterhouses, loadingSlaughterhouses, fetchSlaughterhouses } =
    useSlaughterhouses();

  const [draftSalesInvoice, setDraftSalesInvoice] = useState<DraftSalesInvoice>(
    draftSalesInvoices[currentIndex]
  );

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    watch,
    setError,
    clearErrors,
  } = useForm<SalesInvoiceData>();

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("cycleDisplay", "");
    clearErrors("cycleId");

    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      setValue("cycleDisplay", `${cycle.identifier}/${cycle.year}`);
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchSlaughterhouses();
    }
  }, [open, fetchFarms, fetchSlaughterhouses]);

  useEffect(() => {
    const slaughterhouseName =
      draftSalesInvoice?.extractedFields.slaughterhouseName;
    const slaughterhouseIdFromBackend =
      draftSalesInvoice?.extractedFields.slaughterhouseId;

    if (slaughterhouseIdFromBackend) {
      setValue("slaughterhouseId", slaughterhouseIdFromBackend);
      return;
    }

    if (slaughterhouseName && slaughterhouses.length > 0) {
      const matched = slaughterhouses.find(
        (s) => s.name.toLowerCase() === slaughterhouseName.toLowerCase()
      );
      if (matched) {
        setValue("slaughterhouseId", matched.id);
      }
    }
  }, [slaughterhouses, draftSalesInvoice, setValue]);

  useEffect(() => {
    if (draftSalesInvoices.length === 0 && open) {
      handleClose();
      return;
    }
    const newIndex = Math.min(currentIndex, draftSalesInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }
    if (!farms.length) return;

    const currentDraft = draftSalesInvoices[newIndex];
    setDraftSalesInvoice(currentDraft);
    const data = { ...currentDraft.extractedFields };
    reset(data);

    if (data.farmId) {
      handleFarmChange(data.farmId);
    }
  }, [currentIndex, draftSalesInvoices, farms, reset, open, handleClose]);

  const handleSave = async (formData: SalesInvoiceData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        SalesService.saveSaleInvoice({
          draftId: draftSalesInvoice.draftId,
          filePath: draftSalesInvoice.filePath,
          data: formData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftSalesInvoice);
      },
      (error) => {
        toast.error(
          error?.domainException?.errorDescription ||
            "Wystąpił błąd podczas zapisywania faktury"
        );
      }
    );
    setLoading(false);
  };

  const fileType = getFileTypeFromUrl(draftSalesInvoice?.fileUrl ?? "");

  const renderPreview = () => {
    if (!draftSalesInvoice?.fileUrl)
      return <Typography>Brak podglądu</Typography>;

    if (fileType === "pdf") {
      return (
        <>
          <iframe
            src={draftSalesInvoice.fileUrl}
            title="PDF Preview"
            width="100%"
            height={isLg ? "900px" : isMd ? "700px" : "500px"}
            style={{ border: "1px solid #ccc" }}
          />
          <Button
            startIcon={<MdZoomIn />}
            onClick={() => setPreviewOpen(true)}
            sx={{ mt: 1 }}
          >
            Powiększ
          </Button>
        </>
      );
    } else if (fileType === "image") {
      return (
        <>
          <img
            src={draftSalesInvoice.fileUrl}
            alt="Image Preview"
            style={{
              maxWidth: "100%",
              maxHeight: "900px",
              border: "1px solid #ccc",
            }}
          />
          <Button
            startIcon={<MdZoomIn />}
            onClick={() => setPreviewOpen(true)}
            sx={{ mt: 1 }}
          >
            Powiększ
          </Button>
        </>
      );
    } else {
      return <Typography>Nieobsługiwany format pliku</Typography>;
    }
  };

  if (!draftSalesInvoice) return null;

  return (
    <>
      <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
        <DialogTitle>Podgląd i weryfikacja faktury sprzedażowej</DialogTitle>
        <form onSubmit={handleSubmit(handleSave)}>
          <input type="hidden" {...register("cycleId")} />

          <DialogContent dividers>
            <Grid container spacing={3}>
              <Grid size={{ md: 12, lg: 5, xl: 6 }}>
                <Typography variant="h6">Podgląd faktury</Typography>
                {renderPreview()}
              </Grid>
              <Grid size={{ md: 12, lg: 7, xl: 6 }}>
                <Grid container spacing={2.5}>
                  <Grid size={12}>
                    <Typography variant="h6">Dane na fakturze</Typography>
                  </Grid>

                  <Grid size={{ xs: 12, sm: 6 }}>
                    <LoadingTextField
                      label="Ferma (Sprzedawca)"
                      select
                      fullWidth
                      loading={loadingFarms}
                      value={watch("farmId") || ""}
                      error={!!errors.farmId}
                      helperText={errors.farmId?.message}
                      {...register("farmId", {
                        required: "Ferma jest wymagana",
                        onChange: (e) => handleFarmChange(e.target.value),
                      })}
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
                      loading={loadingCycle}
                      label="Cykl"
                      value={watch("cycleDisplay") || ""}
                      slotProps={{ input: { readOnly: true } }}
                      error={!!errors.cycleId}
                      helperText={errors.cycleId?.message}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12 }}>
                    <Controller
                      name="slaughterhouseId"
                      control={control}
                      rules={{ required: "Rzeźnia jest wymagana" }}
                      render={({ field, fieldState: { error } }) => (
                        <Autocomplete
                          options={slaughterhouses}
                          getOptionLabel={(option) => option.name || ""}
                          isOptionEqualToValue={(option, value) =>
                            option.id === value.id
                          }
                          loading={loadingSlaughterhouses}
                          value={
                            slaughterhouses.find((s) => s.id === field.value) ||
                            null
                          }
                          onChange={(_, newValue) => {
                            field.onChange(newValue ? newValue.id : "");
                          }}
                          renderInput={(params) => (
                            <TextField
                              {...params}
                              label="Rzeźnia (Nabywca)"
                              error={!!error}
                              helperText={error?.message}
                            />
                          )}
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={12}>
                    <Divider sx={{ my: 1 }} />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Numer faktury"
                      value={watch("invoiceNumber") || ""}
                      {...register("invoiceNumber", {
                        required: "Numer faktury jest wymagany",
                      })}
                      error={!!errors.invoiceNumber}
                      helperText={errors.invoiceNumber?.message}
                      fullWidth
                    />
                  </Grid>
                  <Grid size={{ xs: 12, sm: 4 }}>
                    <Controller
                      name="invoiceDate"
                      control={control}
                      rules={{ required: "Data faktury jest wymagana" }}
                      render={({ field }) => (
                        <DatePicker
                          label="Data faktury"
                          format="DD.MM.YYYY"
                          value={field.value ? dayjs(field.value) : null}
                          onChange={(date) =>
                            field.onChange(
                              date ? dayjs(date).format("YYYY-MM-DD") : null
                            )
                          }
                          slotProps={{
                            textField: {
                              fullWidth: true,
                              error: !!errors.invoiceDate,
                              helperText: errors.invoiceDate?.message,
                            },
                          }}
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <Controller
                      name="dueDate"
                      control={control}
                      rules={{ required: "Termin płatności jest wymagany" }}
                      render={({ field }) => (
                        <DatePicker
                          label="Data terminu płatności"
                          format="DD.MM.YYYY"
                          value={field.value ? dayjs(field.value) : null}
                          onChange={(date) =>
                            field.onChange(
                              date ? dayjs(date).format("YYYY-MM-DD") : null
                            )
                          }
                          slotProps={{
                            textField: {
                              fullWidth: true,
                              error: !!errors.invoiceDate,
                              helperText: errors.invoiceDate?.message,
                            },
                          }}
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Netto [zł]"
                      type="number"
                      value={watch("subTotal") || ""}
                      slotProps={{ htmlInput: { step: "0.01" } }}
                      {...register("subTotal", { valueAsNumber: true })}
                      error={!!errors.subTotal}
                      helperText={errors.subTotal?.message}
                      fullWidth
                    />
                  </Grid>
                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="VAT [zł]"
                      type="number"
                      value={watch("vatAmount") || ""}
                      slotProps={{ htmlInput: { step: "0.01" } }}
                      {...register("vatAmount", { valueAsNumber: true })}
                      error={!!errors.vatAmount}
                      helperText={errors.vatAmount?.message}
                      fullWidth
                    />
                  </Grid>
                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Brutto [zł]"
                      type="number"
                      value={watch("invoiceTotal") || ""}
                      slotProps={{ htmlInput: { step: "0.01" } }}
                      {...register("invoiceTotal", { valueAsNumber: true })}
                      error={!!errors.invoiceTotal}
                      helperText={errors.invoiceTotal?.message}
                      fullWidth
                    />
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => setCurrentIndex((prev) => Math.max(0, prev - 1))}
              disabled={currentIndex === 0}
            >
              Poprzedni
            </Button>
            <Button
              onClick={() =>
                setCurrentIndex((prev) =>
                  Math.min(draftSalesInvoices.length - 1, prev + 1)
                )
              }
              disabled={currentIndex === draftSalesInvoices.length - 1}
            >
              Następny
            </Button>
            <Button onClick={handleClose} color="secondary" variant="outlined">
              Anuluj
            </Button>
            <LoadingButton
              type="submit"
              variant="contained"
              color="primary"
              startIcon={<MdSave />}
              loading={loading}
            >
              Zapisz
            </LoadingButton>
          </DialogActions>
        </form>
      </AppDialog>

      <AppDialog
        open={previewOpen}
        onClose={() => setPreviewOpen(false)}
        maxWidth="xl"
        fullWidth
      >
        <DialogContent sx={{ p: 0 }}>
          {fileType === "pdf" ? (
            <iframe
              src={`${draftSalesInvoice.fileUrl ?? ""}#zoom=FitH`}
              title="PDF Fullscreen"
              width="100%"
              height="1000vh"
              style={{ border: "none" }}
            />
          ) : fileType === "image" ? (
            <img
              src={draftSalesInvoice.fileUrl ?? ""}
              alt="Image Fullscreen"
              style={{
                width: "100%",
                height: "auto",
                maxHeight: "90vh",
                objectFit: "contain",
              }}
            />
          ) : (
            <Typography sx={{ p: 2 }}>Nieobsługiwany format pliku</Typography>
          )}
        </DialogContent>
      </AppDialog>
    </>
  );
};

export default SaveSalesInvoicesModal;
