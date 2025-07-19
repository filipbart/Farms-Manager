import {
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  Grid,
  Divider,
  MenuItem,
  DialogActions,
  useTheme,
  useMediaQuery,
  TextField,
} from "@mui/material";
import { useState, useEffect } from "react";
import { MdSave, MdZoomIn } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { DraftFeedInvoice } from "../../../../models/feeds/deliveries/draft-feed-invoice";
import type { FeedInvoiceData } from "../../../../models/feeds/deliveries/feed-invoice";
import { getFileTypeFromUrl } from "../../../../utils/fileUtils";
import LoadingTextField from "../../../common/loading-textfield";
import { Controller, useForm } from "react-hook-form";
import LoadingButton from "../../../common/loading-button";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";

interface SaveInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  draftFeedInvoices: DraftFeedInvoice[];
  onSave: (saveFeedInvoiceData: DraftFeedInvoice) => void;
}

const SaveInvoiceModal: React.FC<SaveInvoiceModalProps> = ({
  open,
  onClose: handleClose,
  draftFeedInvoices,
  onSave,
}) => {
  const theme = useTheme();
  const isMd = useMediaQuery(theme.breakpoints.up("md"));
  const isLg = useMediaQuery(theme.breakpoints.up("lg"));

  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();

  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const { loadLatestCycle, loadingCycle } = useLatestCycle();

  const [draftFeed, setDraftFeed] = useState<DraftFeedInvoice>(
    draftFeedInvoices[0]
  );

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    setError,
    clearErrors,
    watch,
  } = useForm<FeedInvoiceData>({
    defaultValues: draftFeedInvoices[0].extractedFields,
  });

  const [previewOpen, setPreviewOpen] = useState(false);

  const handleFarmChange = async (farmId: string) => {
    setHenhouses(farms.find((f) => f.id === farmId)?.henhouses || []);
    const cycle = await loadLatestCycle(farmId);
    if (cycle) {
      setValue("cycleId", cycle.id);
      clearErrors("cycleId");
      setValue("identifierDisplay", `${cycle.identifier}/${cycle.year}`);
    } else {
      setError("cycleId", {
        type: "manual",
        message: "Brak aktywnego cyklu",
      });
    }
  };

  const handleSave = (formData: FeedInvoiceData) => {
    console.log("Zapisz:", formData);
    onSave(draftFeed);
    handleClose();
  };

  useEffect(() => {
    fetchFarms();
  }, []);

  const fileType = getFileTypeFromUrl(draftFeed.fileUrl ?? "");

  const renderPreview = () => {
    if (!draftFeed.fileUrl) return <Typography>Brak podglądu</Typography>;

    if (fileType === "pdf") {
      return (
        <>
          <iframe
            src={draftFeed.fileUrl}
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
            src={draftFeed.fileUrl}
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

  return (
    <>
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
        <DialogTitle>Podgląd faktury i dane</DialogTitle>

        <form onSubmit={handleSubmit(handleSave)}>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid size={{ md: 12, lg: 5, xl: 6 }}>
                <Typography variant="h6">Podgląd faktury</Typography>
                {renderPreview()}
              </Grid>

              <Grid size={{ md: 12, lg: 7, xl: 6 }}>
                <Grid container spacing={3} alignItems={"center"}>
                  <Grid size={12}>
                    <Typography variant="h6">Dane na fakturze</Typography>
                  </Grid>
                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <LoadingTextField
                      loading={loadingFarms}
                      select
                      label="Ferma"
                      fullWidth
                      error={!!errors.farmId}
                      helperText={errors.farmId?.message}
                      {...register("farmId", {
                        required: "Farma jest wymagana",
                        onChange: async (e: any) => {
                          const value = e.target.value;
                          await handleFarmChange(value);
                        },
                      })}
                    >
                      {farms.map((farm) => (
                        <MenuItem key={farm.id} value={farm.id}>
                          {farm.name}
                        </MenuItem>
                      ))}
                    </LoadingTextField>
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <LoadingTextField
                      loading={loadingCycle}
                      label="Cykl"
                      value={watch("identifierDisplay") || ""}
                      slotProps={{ input: { readOnly: true } }}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      select
                      label="Kurnik"
                      {...register("farmId", {
                        required: "Farma jest wymagana",
                      })}
                      fullWidth
                      disabled={!watch("farmId") || watch("farmId") === ""}
                    >
                      {henhouses.map((house) => (
                        <MenuItem key={house.id} value={house.id}>
                          {house.name}
                        </MenuItem>
                      ))}
                    </TextField>
                  </Grid>

                  <Grid size={{ xs: 12 }}>
                    <Divider />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Numer faktury"
                      error={!!errors.invoiceNumber}
                      helperText={errors.invoiceNumber?.message}
                      {...register("invoiceNumber", {
                        required: "Numer faktury jest wymagany",
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Numer konta bankowego"
                      error={!!errors.bankAccountNumber}
                      helperText={errors.bankAccountNumber?.message}
                      {...register("bankAccountNumber", {
                        required: "Numer konta bankowego jest wymagany",
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Nazwa paszy"
                      error={!!errors.itemName}
                      helperText={errors.itemName?.message}
                      {...register("itemName", {
                        required: "Nazwa paszy jest wymagana",
                      })}
                      fullWidth
                    />
                    {/*TODO: nazwa paszy zrobić listę wybieraną z backendu */}
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Sprzedawca"
                      error={!!errors.vendorName}
                      helperText={errors.vendorName?.message}
                      {...register("vendorName", {
                        required: "Sprzedawca jest wymagany",
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Ilość"
                      type="number"
                      inputMode="decimal"
                      slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                      error={!!errors.quantity}
                      helperText={errors.quantity?.message}
                      {...register("quantity", {
                        required: "Ilość jest wymagana",
                        valueAsNumber: true,
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Cena jednostkowa [zł]"
                      type="number"
                      inputMode="decimal"
                      slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                      error={!!errors.unitPrice}
                      helperText={errors.unitPrice?.message}
                      {...register("unitPrice", {
                        required: "Cena jednostkowa jest wymagana",
                        valueAsNumber: true,
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <Controller
                      name="invoiceDate"
                      control={control}
                      rules={{
                        required: "Data wystawienia faktury jest wymagana",
                      }}
                      render={({ field }) => (
                        <DatePicker
                          label="Data wystawienia faktury"
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

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <Controller
                      name="dueDate"
                      control={control}
                      rules={{
                        required: "Termin płatności faktury jest wymagany",
                      }}
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
                              error: !!errors.dueDate,
                              helperText: errors.dueDate?.message,
                            },
                          }}
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Wartość brutto [zł]"
                      type="number"
                      inputMode="decimal"
                      slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                      error={!!errors.invoiceTotal}
                      helperText={errors.invoiceTotal?.message}
                      {...register("invoiceTotal", {
                        required: "Wartość brutto jest wymagana",
                        valueAsNumber: true,
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Wartość netto [zł]"
                      type="number"
                      inputMode="decimal"
                      slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                      error={!!errors.subTotal}
                      helperText={errors.subTotal?.message}
                      {...register("subTotal", {
                        required: "Wartość netto jest wymagana",
                        valueAsNumber: true,
                      })}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="VAT [zł]"
                      type="number"
                      inputMode="decimal"
                      slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                      error={!!errors.vatAmount}
                      helperText={errors.vatAmount?.message}
                      {...register("vatAmount", {
                        required: "VAT jest wymagany",
                        valueAsNumber: true,
                      })}
                      fullWidth
                    />
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
          </DialogContent>
        </form>

        <DialogActions>
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
      </Dialog>

      {/* DIALOG PODGLĄDU */}
      <Dialog
        open={previewOpen}
        onClose={() => setPreviewOpen(false)}
        maxWidth="xl"
        fullWidth
      >
        <DialogContent sx={{ p: 0 }}>
          {fileType === "pdf" ? (
            <iframe
              src={`${draftFeed.fileUrl ?? ""}#zoom=FitH`}
              title="PDF Fullscreen"
              width="100%"
              height="1000vh"
              style={{ border: "none" }}
            />
          ) : fileType === "image" ? (
            <img
              src={draftFeed.fileUrl ?? ""}
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
      </Dialog>
    </>
  );
};

export default SaveInvoiceModal;
