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
} from "@mui/material";
import { useState, useEffect } from "react";
import { MdSave } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { DraftFeedInvoice } from "../../../../models/feeds/deliveries/draft-feed-invoice";
import type { FeedInvoiceData } from "../../../../models/feeds/deliveries/feed-invoice";
import { getFileTypeFromUrl } from "../../../../utils/fileUtils";
import LoadingTextField from "../../../common/loading-textfield";
import FilePreview from "../../../common/file-preview";
import { Controller, useForm } from "react-hook-form";
import LoadingButton from "../../../common/loading-button";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { FeedsService } from "../../../../services/feeds-service";
import { toast } from "react-toastify";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import AppDialog from "../../../common/app-dialog";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";

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
  const [currentIndex, setCurrentIndex] = useState(0);

  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
    clearErrors,
    watch,
  } = useForm<FeedInvoiceData>();

  const [previewOpen, setPreviewOpen] = useState(false);
  const watchedFarmId = watch("farmId");
  const watchedUnitPrice = watch("unitPrice");
  const watchedQuantity = watch("quantity");

  const draftFeed = draftFeedInvoices[currentIndex];

  // Helper function to round to 2 decimal places (0.005 rounds up)
  const roundTo2Decimals = (value: number): number => {
    return Math.round(value * 100) / 100;
  };

  // Helper function to format number with thousand separators (spaces)
  const formatNumberWithSpaces = (value: string | number): string => {
    if (value === "" || value === null || value === undefined) return "";

    const numStr = String(value);
    const parts = numStr.split(".");
    const integerPart = parts[0].replace(/\s/g, "");
    const decimalPart = parts[1] || "";

    // Add spaces as thousand separators
    const formatted = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, " ");

    return decimalPart ? `${formatted}.${decimalPart}` : formatted;
  };

  // Helper function to parse formatted number back to plain number
  const parseFormattedNumber = (value: string): string => {
    if (!value) return "";
    return value.replace(/\s/g, "");
  };

  // Auto-calculate invoice values when unitPrice or quantity changes
  useEffect(() => {
    if (watchedUnitPrice && watchedQuantity) {
      const unitPrice = Number(watchedUnitPrice);
      const quantity = Number(watchedQuantity);

      if (
        !isNaN(unitPrice) &&
        !isNaN(quantity) &&
        unitPrice >= 0 &&
        quantity >= 0
      ) {
        const subTotal = roundTo2Decimals(unitPrice * quantity);
        const vatAmount = roundTo2Decimals(subTotal * 0.08);
        const invoiceTotal = roundTo2Decimals(subTotal + vatAmount);

        setValue("subTotal", subTotal);
        setValue("vatAmount", vatAmount);
        setValue("invoiceTotal", invoiceTotal);
      }
    }
  }, [watchedUnitPrice, watchedQuantity, setValue]);

  useEffect(() => {
    const fetchCyclesAndHenhouses = async (farmId: string) => {
      setValue("henhouseId", "");
      clearErrors("henhouseId");
      setHenhouses(farms.find((f) => f.id === farmId)?.henhouses || []);

      setLoadingCycles(true);
      const selectedFarm = farms.find((f) => f.id === farmId);
      if (selectedFarm?.activeCycle) {
        setValue("cycleId", selectedFarm.activeCycle.id);
      } else {
        setValue("cycleId", "");
      }

      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => {
          setCycles(data.responseData ?? []);
        },
        () => {
          setCycles([]);
        },
        "Nie udało się pobrać listy cykli.",
      );
      setLoadingCycles(false);
    };

    if (watchedFarmId && farms.length > 0) {
      fetchCyclesAndHenhouses(watchedFarmId);
    } else {
      setCycles([]);
      setHenhouses([]);
    }
  }, [watchedFarmId, farms, setValue, clearErrors]);

  useEffect(() => {
    const henhouseName = draftFeed?.extractedFields.henhouseName?.toLowerCase();
    const henhouseIdFromBackend = draftFeed?.extractedFields.henhouseId;

    if (henhouseIdFromBackend) {
      setValue("henhouseId", henhouseIdFromBackend);
      clearErrors("henhouseId");
      return;
    }

    if (henhouseName && henhouses.length > 0) {
      const matchedHenhouse = henhouses.find(
        (h) => h.name.toLowerCase() === henhouseName,
      );

      if (matchedHenhouse) {
        setValue("henhouseId", matchedHenhouse.id);
        clearErrors("henhouseId");
      }
    }
  }, [henhouses, draftFeed, setValue, clearErrors]);

  const handleSave = async (formData: FeedInvoiceData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        FeedsService.saveFeedInvoice({
          filePath: draftFeed.filePath,
          draftId: draftFeed.draftId,
          data: formData,
        }),
      () => {
        toast.success(`Pomyślnie zapisano fakturę: ${formData.invoiceNumber}`);
        onSave(draftFeed);
      },
      undefined,
      "Wystąpił błąd podczas zapisywania danych faktury",
    );
    setLoading(false);
  };

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchFeedsNames();
    }
  }, [open, fetchFarms, fetchFeedsNames]);

  const inferFarmId = (
    draft: DraftFeedInvoice,
    allFarms: typeof farms,
  ): string | undefined => {
    if (draft.extractedFields.farmId) return draft.extractedFields.farmId;

    const henhouseName = draft.extractedFields.henhouseName?.toLowerCase();
    const matchedFarms = allFarms.filter(
      (f) =>
        f.nip === draft.extractedFields.nip ||
        f.name?.toLowerCase() ===
          draft.extractedFields.customerName?.toLowerCase(),
    );

    if (matchedFarms.length === 1) return matchedFarms[0].id;

    if (matchedFarms.length > 1 && henhouseName) {
      return matchedFarms.find((farm) =>
        farm.henhouses?.some(
          (house) => house.name?.toLowerCase() === henhouseName,
        ),
      )?.id;
    }

    return undefined;
  };

  useEffect(() => {
    if (draftFeedInvoices.length === 0 && open) {
      handleClose();
      return;
    }

    const newIndex = Math.min(currentIndex, draftFeedInvoices.length - 1);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
      return;
    }

    if (farms.length === 0) return;

    const currentDraft = draftFeedInvoices[newIndex];
    const data = { ...currentDraft.extractedFields };

    if (!data.farmId) {
      data.farmId = inferFarmId(currentDraft, farms);
    }
    reset(data);
  }, [currentIndex, draftFeedInvoices, farms, reset, open, handleClose]);

  const fileType = getFileTypeFromUrl(draftFeed?.fileUrl ?? "");

  const renderPreview = () => {
    if (!draftFeed?.fileUrl) return <Typography>Brak podglądu</Typography>;

    return (
      <FilePreview
        file={draftFeed.fileUrl}
        maxHeight={isLg ? 900 : isMd ? 700 : 500}
        showPreviewButton={true}
      />
    );
  };

  if (!draftFeed) {
    return null;
  }

  return (
    <>
      <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="xl">
        <DialogTitle>Podgląd faktury i dane</DialogTitle>

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
                    <Typography variant="h6">Dane na fakturze</Typography>
                  </Grid>
                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <LoadingTextField
                      loading={loadingFarms}
                      value={watch("farmId") || ""}
                      select
                      label="Ferma"
                      fullWidth
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

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <LoadingTextField
                      loading={loadingCycles}
                      label="Cykl"
                      select
                      fullWidth
                      disabled={!watchedFarmId || cycles.length === 0}
                      value={watch("cycleId") || ""}
                      error={!!errors.cycleId}
                      helperText={errors.cycleId?.message}
                      {...register("cycleId", {
                        required: "Cykl jest wymagany",
                      })}
                    >
                      {cycles.map((cycle) => (
                        <MenuItem key={cycle.id} value={cycle.id}>
                          {`${cycle.identifier}/${cycle.year}`}
                        </MenuItem>
                      ))}
                    </LoadingTextField>
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      select
                      label="Kurnik"
                      error={!!errors.henhouseId}
                      helperText={errors.henhouseId?.message}
                      value={watch("henhouseId") || ""}
                      {...register("henhouseId", {
                        required: "Kurnik jest wymagany",
                      })}
                      fullWidth
                      disabled={!watch("farmId")}
                    >
                      {henhouses.map((house) => (
                        <MenuItem key={house.id} value={house.id}>
                          {house.name}
                        </MenuItem>
                      ))}
                    </TextField>
                  </Grid>

                  <Grid size={12}>
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
                      slotProps={{ inputLabel: { shrink: true } }}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Numer konta bankowego"
                      error={!!errors.bankAccountNumber}
                      helperText={errors.bankAccountNumber?.message}
                      {...register("bankAccountNumber")}
                      fullWidth
                      slotProps={{ inputLabel: { shrink: true } }}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <LoadingTextField
                      value={watch("itemName") || ""}
                      select
                      label="Typ (nazwa) paszy"
                      fullWidth
                      error={!!errors.itemName}
                      helperText={errors.itemName?.message}
                      {...register("itemName", {
                        required: "Nazwa paszy jest wymagana",
                      })}
                      loading={loadingFeedsNames}
                    >
                      {feedsNames.map((feedName) => (
                        <MenuItem key={feedName.id} value={feedName.name}>
                          {feedName.name}
                        </MenuItem>
                      ))}
                    </LoadingTextField>
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
                      slotProps={{ inputLabel: { shrink: true } }}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 12, md: 6 }}>
                    <TextField
                      label="Ilość"
                      type="number"
                      slotProps={{
                        htmlInput: { min: 0, step: "0.01" },
                        inputLabel: { shrink: true },
                      }}
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
                    <Controller
                      name="unitPrice"
                      control={control}
                      rules={{
                        required: "Cena jednostkowa jest wymagana",
                        validate: (value) => {
                          const num = Number(value);
                          return (
                            (!isNaN(num) && num >= 0) ||
                            "Wartość musi być liczbą większą lub równą 0"
                          );
                        },
                      }}
                      render={({ field }) => (
                        <TextField
                          label="Cena jednostkowa [zł]"
                          value={formatNumberWithSpaces(field.value || "")}
                          onChange={(e) => {
                            const parsed = parseFormattedNumber(e.target.value);
                            field.onChange(parsed ? Number(parsed) : "");
                          }}
                          slotProps={{
                            inputLabel: { shrink: true },
                          }}
                          error={!!errors.unitPrice}
                          helperText={errors.unitPrice?.message}
                          fullWidth
                        />
                      )}
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
                              date ? dayjs(date).format("YYYY-MM-DD") : "",
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
                              date ? dayjs(date).format("YYYY-MM-DD") : "",
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
                    <Controller
                      name="subTotal"
                      control={control}
                      rules={{
                        required: "Wartość netto jest wymagana",
                        validate: (value) => {
                          const num = Number(value);
                          return (
                            (!isNaN(num) && num >= 0) ||
                            "Wartość musi być liczbą większą lub równą 0"
                          );
                        },
                      }}
                      render={({ field }) => (
                        <TextField
                          label="Wartość netto [zł]"
                          value={formatNumberWithSpaces(field.value || "")}
                          onChange={(e) => {
                            const parsed = parseFormattedNumber(e.target.value);
                            field.onChange(parsed ? Number(parsed) : "");
                          }}
                          slotProps={{
                            inputLabel: { shrink: true },
                          }}
                          error={!!errors.subTotal}
                          helperText={errors.subTotal?.message}
                          fullWidth
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <Controller
                      name="vatAmount"
                      control={control}
                      rules={{
                        required: "VAT jest wymagany",
                        validate: (value) => {
                          const num = Number(value);
                          return (
                            (!isNaN(num) && num >= 0) ||
                            "Wartość musi być liczbą większą lub równą 0"
                          );
                        },
                      }}
                      render={({ field }) => (
                        <TextField
                          label="VAT [zł]"
                          value={formatNumberWithSpaces(field.value || "")}
                          onChange={(e) => {
                            const parsed = parseFormattedNumber(e.target.value);
                            field.onChange(parsed ? Number(parsed) : "");
                          }}
                          slotProps={{
                            inputLabel: { shrink: true },
                          }}
                          error={!!errors.vatAmount}
                          helperText={errors.vatAmount?.message}
                          fullWidth
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <Controller
                      name="invoiceTotal"
                      control={control}
                      rules={{
                        required: "Wartość brutto jest wymagana",
                        validate: (value) => {
                          const num = Number(value);
                          return (
                            (!isNaN(num) && num >= 0) ||
                            "Wartość musi być liczbą większą lub równą 0"
                          );
                        },
                      }}
                      render={({ field }) => (
                        <TextField
                          label="Wartość brutto [zł]"
                          value={formatNumberWithSpaces(field.value || "")}
                          onChange={(e) => {
                            const parsed = parseFormattedNumber(e.target.value);
                            field.onChange(parsed ? Number(parsed) : "");
                          }}
                          slotProps={{
                            inputLabel: { shrink: true },
                          }}
                          error={!!errors.invoiceTotal}
                          helperText={errors.invoiceTotal?.message}
                          fullWidth
                        />
                      )}
                    />
                  </Grid>

                  <Grid size={12}>
                    <TextField
                      label="Notatka"
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
                  Math.min(draftFeedInvoices.length - 1, prev + 1),
                )
              }
              disabled={currentIndex === draftFeedInvoices.length - 1}
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

      <AppDialog
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
      </AppDialog>
    </>
  );
};

export default SaveInvoiceModal;
