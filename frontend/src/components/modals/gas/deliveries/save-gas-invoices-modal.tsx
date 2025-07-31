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
import { getFileTypeFromUrl } from "../../../../utils/fileUtils";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { useGasContractors } from "../../../../hooks/gas/useGasContractors";
import type {
  DraftGasInvoice,
  GasInvoiceData,
} from "../../../../models/gas/gas-deliveries";
import { GasService } from "../../../../services/gas-service";

interface SaveGasInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  draftGasInvoices: DraftGasInvoice[];
  onSave: (savedDraft: DraftGasInvoice) => void;
}

const SaveGasInvoicesModal: React.FC<SaveGasInvoicesModalProps> = ({
  open,
  onClose: handleClose,
  draftGasInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [previewOpen, setPreviewOpen] = useState(false);

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { gasContractors, loadingGasContractors, fetchGasContractors } =
    useGasContractors();

  const [draftGasInvoice, setDraftGasInvoice] = useState<DraftGasInvoice>(
    draftGasInvoices[currentIndex]
  );

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<GasInvoiceData>();

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchGasContractors();
    }
  }, [open, fetchFarms, fetchGasContractors]);

  useEffect(() => {
    const contractorName = draftGasInvoice?.extractedFields.contractorName;
    const contractorIdFromBackend =
      draftGasInvoice?.extractedFields.contractorId;

    if (contractorIdFromBackend) {
      setValue("contractorId", contractorIdFromBackend);
      return;
    }

    if (contractorName && gasContractors.length > 0) {
      const matchedContractor = gasContractors.find(
        (c) => c.name.toLowerCase() === contractorName.toLowerCase()
      );
      if (matchedContractor) {
        setValue("contractorId", matchedContractor.id);
      }
    }
  }, [gasContractors, draftGasInvoice, setValue]);

  useEffect(() => {
    if (draftGasInvoices.length === 0 && open) {
      handleClose();
      return;
    }
    const newIndex = Math.min(currentIndex, draftGasInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }

    const currentDraft = draftGasInvoices[newIndex];
    setDraftGasInvoice(currentDraft);
    reset({ ...currentDraft.extractedFields });
  }, [currentIndex, draftGasInvoices, reset, open, handleClose]);

  const handleSave = async (formData: GasInvoiceData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        GasService.saveGasInvoice({
          draftId: draftGasInvoice.draftId,
          filePath: draftGasInvoice.filePath,
          data: formData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftGasInvoice);
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

  const fileType = getFileTypeFromUrl(draftGasInvoice?.fileUrl ?? "");

  const renderPreview = () => {
    if (!draftGasInvoice?.fileUrl)
      return <Typography>Brak podglądu</Typography>;

    if (fileType === "pdf") {
      return (
        <>
          <iframe
            src={draftGasInvoice.fileUrl}
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
            src={draftGasInvoice.fileUrl}
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

  if (!draftGasInvoice) return null;

  return (
    <>
      <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
        <DialogTitle>Podgląd i weryfikacja faktury za gaz</DialogTitle>
        <form onSubmit={handleSubmit(handleSave)}>
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
                      label="Ferma"
                      select
                      fullWidth
                      loading={loadingFarms}
                      value={watch("farmId") || ""}
                      error={!!errors.farmId}
                      helperText={errors.farmId?.message}
                      {...register("farmId", {
                        required: "Farma jest wymagana",
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
                    <Controller
                      name="contractorId"
                      control={control}
                      rules={{ required: "Kontrahent jest wymagany" }}
                      render={({ field, fieldState: { error } }) => (
                        <Autocomplete
                          options={gasContractors}
                          getOptionLabel={(option) => option.name || ""}
                          isOptionEqualToValue={(option, value) =>
                            option.id === value.id
                          }
                          loading={loadingGasContractors}
                          value={
                            gasContractors.find((c) => c.id === field.value) ||
                            null
                          }
                          onChange={(_, newValue) => {
                            field.onChange(newValue ? newValue.id : "");
                          }}
                          renderInput={(params) => (
                            <TextField
                              {...params}
                              label="Kontrahent"
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

                  <Grid size={{ xs: 12, sm: 6 }}>
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
                  <Grid size={{ xs: 12, sm: 6 }}>
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

                  <Grid size={{ xs: 12, sm: 6 }}>
                    <TextField
                      label="Cena jednostkowa [zł]"
                      type="number"
                      value={watch("unitPrice") || ""}
                      slotProps={{ htmlInput: { step: "0.01" } }}
                      {...register("unitPrice", {
                        required: "Cena jest wymagana",
                        valueAsNumber: true,
                      })}
                      error={!!errors.unitPrice}
                      helperText={errors.unitPrice?.message}
                      fullWidth
                    />
                  </Grid>
                  <Grid size={{ xs: 12, sm: 6 }}>
                    <TextField
                      label="Ilość [l]"
                      type="number"
                      value={watch("quantity") || ""}
                      slotProps={{ htmlInput: { step: "0.01" } }}
                      {...register("quantity", {
                        required: "Ilość jest wymagana",
                        valueAsNumber: true,
                      })}
                      error={!!errors.quantity}
                      helperText={errors.quantity?.message}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={12}>
                    <TextField
                      label="Komentarz"
                      value={watch("comment") || ""}
                      {...register("comment")}
                      multiline
                      rows={4}
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
                  Math.min(draftGasInvoices.length - 1, prev + 1)
                )
              }
              disabled={currentIndex === draftGasInvoices.length - 1}
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
              src={`${draftGasInvoice.fileUrl ?? ""}#zoom=FitH`}
              title="PDF Fullscreen"
              width="100%"
              height="1000vh"
              style={{ border: "none" }}
            />
          ) : fileType === "image" ? (
            <img
              src={draftGasInvoice.fileUrl ?? ""}
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

export default SaveGasInvoicesModal;
