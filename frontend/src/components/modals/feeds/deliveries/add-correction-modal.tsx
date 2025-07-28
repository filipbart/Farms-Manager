import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
  Box,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import { MdSave, MdAttachFile } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import type { CorrectionData } from "../../../../models/feeds/corrections/correction";
import type { GridRowId } from "@mui/x-data-grid";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { FeedsService } from "../../../../services/feeds-service";
import { toast } from "react-toastify";
import { useLatestCycle } from "../../../../hooks/useLatestCycle";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import FilePreview from "../../../common/file-preview";
import AppDialog from "../../../common/app-dialog";

interface AddCorrectionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  selectedDeliveries: Set<GridRowId>;
}

const AddCorrectionModal: React.FC<AddCorrectionModalProps> = ({
  open,
  onClose,
  onSave,
  selectedDeliveries,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [selectedFile, setSelectedFile] = useState<File>();
  const { loadLatestCycle, loadingCycle } = useLatestCycle();
  const [loading, setLoading] = useState(false);

  const filePreviewUrl = selectedFile ? URL.createObjectURL(selectedFile) : "";

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    clearErrors,
    setError,
    control,
    watch,
  } = useForm<CorrectionData>({
    defaultValues: {
      invoiceNumber: "",
      farmId: "",
      cycleId: "",
      identifierDisplay: "",
      subTotal: 0,
      vatAmount: 0,
      invoiceTotal: 0,
      invoiceDate: "",
    },
  });

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  useEffect(() => {
    return () => {
      if (filePreviewUrl) {
        URL.revokeObjectURL(filePreviewUrl);
      }
    };
  }, [filePreviewUrl]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      const file = e.target.files[0];
      setSelectedFile(file);
      setValue("file", file);
    }
  };

  const handleFarmChange = async (farmId: string) => {
    setValue("cycleId", "");
    setValue("identifierDisplay", "");
    clearErrors("cycleId");

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

  const handleSave = async (data: CorrectionData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        FeedsService.addFeedCorrection({
          farmId: data.farmId,
          cycleId: data.cycleId,
          invoiceNumber: data.invoiceNumber,
          subTotal: data.subTotal,
          vatAmount: data.vatAmount,
          invoiceTotal: data.invoiceTotal,
          invoiceDate: data.invoiceDate,
          file: data.file,
          feedInvoiceIds: Array.from(selectedDeliveries).map(String),
        }),
      () => {
        toast.success("Pomyślnie dodano fakturę korekty");
        setSelectedFile(undefined);
        reset();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania faktury korekty"
    );
    setLoading(false);
  };

  return (
    <>
      <AppDialog open={open} onClose={onClose} maxWidth="lg" fullWidth>
        <DialogTitle>Dodaj fakturę kosztową</DialogTitle>

        <form onSubmit={handleSubmit(handleSave)}>
          <DialogContent>
            <Grid container spacing={2}>
              <Grid
                container
                direction="column"
                spacing={2}
                size={{ xs: 12, md: 4 }}
              >
                <Grid size={{ xs: 12 }}>
                  <Button
                    variant="outlined"
                    component="label"
                    startIcon={<MdAttachFile />}
                    fullWidth
                  >
                    Wybierz plik
                    <input type="file" hidden onChange={handleFileChange} />
                  </Button>
                </Grid>

                {selectedFile && (
                  <>
                    <Grid size={{ xs: 12 }}>
                      <Typography variant="body2">
                        Wybrano plik: {selectedFile.name}
                      </Typography>
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                      <Box mt={1}>
                        <FilePreview file={selectedFile} maxHeight={500} />
                      </Box>
                    </Grid>
                  </>
                )}
              </Grid>

              <Grid size={{ xs: 12, md: 8 }}>
                <Grid container spacing={2}>
                  <Grid size={{ xs: 12 }}>
                    <TextField
                      label="Numer faktury"
                      {...register("invoiceNumber", {
                        required: "Numer faktury jest wymagany",
                      })}
                      error={!!errors.invoiceNumber}
                      helperText={errors.invoiceNumber?.message}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12 }}>
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
                        onChange: async (e) => {
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

                  <Grid size={{ xs: 12 }}>
                    <LoadingTextField
                      loading={loadingCycle}
                      label="Cykl"
                      value={watch("identifierDisplay") || ""}
                      slotProps={{ input: { readOnly: true } }}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12 }}>
                    <Controller
                      name="invoiceDate"
                      control={control}
                      rules={{
                        required: "Data korekty jest wymagana",
                      }}
                      render={({ field }) => (
                        <DatePicker
                          label="Data korekty"
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

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Netto [zł]"
                      type="number"
                      slotProps={{ htmlInput: { step: "any" } }}
                      {...register("subTotal", {
                        required: "Wartość netto jest wymagana",
                        valueAsNumber: true,
                      })}
                      error={!!errors.subTotal}
                      helperText={errors.subTotal?.message}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="VAT [zł]"
                      type="number"
                      slotProps={{ htmlInput: { step: "any" } }}
                      {...register("vatAmount", {
                        required: "VAT jest wymagany",
                        valueAsNumber: true,
                      })}
                      error={!!errors.vatAmount}
                      helperText={errors.vatAmount?.message}
                      fullWidth
                    />
                  </Grid>

                  <Grid size={{ xs: 12, sm: 4 }}>
                    <TextField
                      label="Brutto [zł]"
                      type="number"
                      slotProps={{ htmlInput: { step: "any" } }}
                      {...register("invoiceTotal", {
                        required: "Wartość brutto jest wymagana",
                        valueAsNumber: true,
                      })}
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
              onClick={() => {
                setSelectedFile(undefined);
                reset();
                onClose();
              }}
            >
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
    </>
  );
};

export default AddCorrectionModal;
