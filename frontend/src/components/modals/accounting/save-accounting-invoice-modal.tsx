import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  Divider,
  DialogActions,
  useTheme,
  useMediaQuery,
  TextField,
} from "@mui/material";
import { useState, useEffect } from "react";
import { MdSave } from "react-icons/md";
import type { DraftAccountingInvoice } from "../../../services/accounting-service";
import { AccountingService } from "../../../services/accounting-service";
import FilePreview from "../../common/file-preview";
import { Controller, useForm } from "react-hook-form";
import LoadingButton from "../../common/loading-button";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { toast } from "react-toastify";
import AppDialog from "../../common/app-dialog";

interface SaveAccountingInvoiceFormData {
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  sellerName: string;
  sellerNip: string;
  buyerName: string;
  buyerNip: string;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  comment: string;
}

interface SaveAccountingInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  draftInvoices: DraftAccountingInvoice[];
  onSave: (savedInvoice: DraftAccountingInvoice) => void;
}

const SaveAccountingInvoiceModal: React.FC<SaveAccountingInvoiceModalProps> = ({
  open,
  onClose: handleClose,
  draftInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const [currentIndex, setCurrentIndex] = useState(0);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
  } = useForm<SaveAccountingInvoiceFormData>();

  const draftInvoice = draftInvoices[currentIndex];

  useEffect(() => {
    if (draftInvoices.length === 0 && open) {
      handleClose();
      return;
    }

    const newIndex = Math.min(currentIndex, draftInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }

    const currentDraft = draftInvoices[newIndex];
    if (currentDraft) {
      reset({
        invoiceNumber: currentDraft.extractedFields.invoiceNumber || "",
        invoiceDate: currentDraft.extractedFields.invoiceDate || "",
        dueDate: currentDraft.extractedFields.dueDate || "",
        sellerName: currentDraft.extractedFields.sellerName || "",
        sellerNip: currentDraft.extractedFields.sellerNip || "",
        buyerName: currentDraft.extractedFields.buyerName || "",
        buyerNip: currentDraft.extractedFields.buyerNip || "",
        grossAmount: currentDraft.extractedFields.grossAmount || 0,
        netAmount: currentDraft.extractedFields.netAmount || 0,
        vatAmount: currentDraft.extractedFields.vatAmount || 0,
        comment: "",
      });
    }
  }, [currentIndex, draftInvoices, reset, open, handleClose]);

  const handleSave = async (formData: SaveAccountingInvoiceFormData) => {
    if (!draftInvoice) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        AccountingService.saveInvoice({
          draftId: draftInvoice.draftId,
          filePath: draftInvoice.filePath,
          invoiceNumber: formData.invoiceNumber,
          invoiceDate: formData.invoiceDate,
          dueDate: formData.dueDate || undefined,
          sellerName: formData.sellerName,
          sellerNip: formData.sellerNip,
          buyerName: formData.buyerName,
          buyerNip: formData.buyerNip,
          grossAmount: formData.grossAmount,
          netAmount: formData.netAmount,
          vatAmount: formData.vatAmount,
          invoiceType: draftInvoice.extractedFields.invoiceType,
          comment: formData.comment || undefined,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftInvoice);
      },
      undefined,
      "Wystąpił błąd podczas zapisywania faktury"
    );
    setLoading(false);
  };

  const renderPreview = () => {
    if (!draftInvoice?.fileUrl) return <Typography>Brak podglądu</Typography>;

    return (
      <FilePreview
        file={draftInvoice.fileUrl}
        maxHeight={isLg ? 900 : isMd ? 700 : 500}
        showPreviewButton={true}
      />
    );
  };

  if (!draftInvoice) {
    return null;
  }

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
      <DialogTitle>
        Podgląd faktury i dane ({currentIndex + 1} / {draftInvoices.length})
      </DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid size={{ md: 12, lg: 5, xl: 6 }}>
              <Typography variant="h6">Podgląd faktury</Typography>
              {renderPreview()}
            </Grid>

            <Grid size={{ md: 12, lg: 7, xl: 6 }}>
              <Grid container spacing={3} alignItems={"top"}>
                <Grid size={12}>
                  <Typography variant="h6">Dane zaczytane z faktury</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Sprawdź i popraw dane, jeśli AI zaczytało je niepoprawnie
                  </Typography>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Numer faktury"
                    error={!!errors.invoiceNumber}
                    helperText={errors.invoiceNumber?.message}
                    {...register("invoiceNumber", {
                      required: "Numer faktury jest wymagany",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <Controller
                    name="invoiceDate"
                    control={control}
                    rules={{
                      required: "Data wystawienia faktury jest wymagana",
                    }}
                    render={({ field }) => (
                      <DatePicker
                        label="Data wystawienia"
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
                            error: !!errors.invoiceDate,
                            helperText: errors.invoiceDate?.message,
                          },
                        }}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <Controller
                    name="dueDate"
                    control={control}
                    render={({ field }) => (
                      <DatePicker
                        label="Termin płatności"
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
                          },
                        }}
                      />
                    )}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Sprzedawca</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Nazwa sprzedawcy"
                    error={!!errors.sellerName}
                    helperText={errors.sellerName?.message}
                    {...register("sellerName", {
                      required: "Nazwa sprzedawcy jest wymagana",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="NIP sprzedawcy"
                    error={!!errors.sellerNip}
                    helperText={errors.sellerNip?.message}
                    {...register("sellerNip")}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Nabywca</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Nazwa nabywcy"
                    error={!!errors.buyerName}
                    helperText={errors.buyerName?.message}
                    {...register("buyerName", {
                      required: "Nazwa nabywcy jest wymagana",
                    })}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="NIP nabywcy"
                    error={!!errors.buyerNip}
                    helperText={errors.buyerNip?.message}
                    {...register("buyerNip")}
                    fullWidth
                    slotProps={{ inputLabel: { shrink: true } }}
                  />
                </Grid>

                <Grid size={12}>
                  <Divider>
                    <Typography variant="caption">Kwoty</Typography>
                  </Divider>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota netto [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.netAmount}
                    helperText={errors.netAmount?.message}
                    {...register("netAmount", {
                      required: "Kwota netto jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota VAT [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.vatAmount}
                    helperText={errors.vatAmount?.message}
                    {...register("vatAmount", {
                      required: "Kwota VAT jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <TextField
                    label="Kwota brutto [zł]"
                    type="number"
                    slotProps={{
                      htmlInput: { min: 0, step: "0.01" },
                      inputLabel: { shrink: true },
                    }}
                    error={!!errors.grossAmount}
                    helperText={errors.grossAmount?.message}
                    {...register("grossAmount", {
                      required: "Kwota brutto jest wymagana",
                      valueAsNumber: true,
                    })}
                    fullWidth
                  />
                </Grid>

                <Grid size={12}>
                  <TextField
                    label="Komentarz"
                    error={!!errors.comment}
                    helperText={errors.comment?.message}
                    {...register("comment")}
                    fullWidth
                    multiline
                    rows={2}
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
                Math.min(draftInvoices.length - 1, prev + 1)
              )
            }
            disabled={currentIndex === draftInvoices.length - 1}
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

export default SaveAccountingInvoiceModal;
