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
  Autocomplete,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave, MdAttachFile } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import FilePreview from "../../../common/file-preview";
import AppDialog from "../../../common/app-dialog";
import { useFarms } from "../../../../hooks/useFarms";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { AddGasDeliveryData } from "../../../../models/gas/gas-deliveries";
import { GasService } from "../../../../services/gas-service";
import { useGasContractors } from "../../../../hooks/gas/useGasContractors";

interface AddGasDeliveryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddGasDeliveryModal: React.FC<AddGasDeliveryModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { gasContractors, loadingGasContractors, fetchGasContractors } =
    useGasContractors();

  const [selectedFile, setSelectedFile] = useState<File>();
  const [loading, setLoading] = useState(false);

  const filePreviewUrl = selectedFile ? URL.createObjectURL(selectedFile) : "";

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
  } = useForm<AddGasDeliveryData>();

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchGasContractors();
    }
  }, [open, fetchFarms, fetchGasContractors]);

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

  const handleClose = () => {
    setSelectedFile(undefined);
    reset();
    onClose();
  };

  const handleSave = async (data: AddGasDeliveryData) => {
    setLoading(true);
    await handleApiResponse(
      () => GasService.addGasDelivery(data),
      () => {
        toast.success("Pomyślnie dodano dostawę gazu");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania dostawy gazu"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="lg" fullWidth>
      <DialogTitle>Dodaj dostawę gazu</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 5 }}>
              <Box display="flex" flexDirection="column" gap={2}>
                <Button
                  variant="outlined"
                  component="label"
                  startIcon={<MdAttachFile />}
                  fullWidth
                >
                  Wybierz plik faktury
                  <input type="file" hidden onChange={handleFileChange} />
                </Button>
                {selectedFile && (
                  <>
                    <Typography variant="body2" noWrap>
                      Wybrano plik: {selectedFile.name}
                    </Typography>
                    <Box mt={1}>
                      <FilePreview file={selectedFile} maxHeight={700} />
                    </Box>
                  </>
                )}
              </Box>
            </Grid>

            <Grid size={{ xs: 12, md: 7 }}>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <LoadingTextField
                    label="Ferma"
                    select
                    fullWidth
                    loading={loadingFarms}
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
                          gasContractors.find(
                            (contractor) => contractor.id === field.value
                          ) || null
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

                <Grid size={{ xs: 12, sm: 6 }}>
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
                  <TextField
                    label="Cena jednostkowa [zł]"
                    type="number"
                    slotProps={{ htmlInput: { step: "any" } }}
                    {...register("unitPrice", {
                      required: "Cena jest wymagana",
                      valueAsNumber: true,
                      validate: (value) =>
                        value > 0 || "Cena musi być większa od 0",
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
                    slotProps={{ htmlInput: { step: "any" } }}
                    {...register("quantity", {
                      required: "Ilość jest wymagana",
                      valueAsNumber: true,
                      validate: (value) =>
                        value > 0 || "Ilość musi być większa od 0",
                    })}
                    error={!!errors.quantity}
                    helperText={errors.quantity?.message}
                    fullWidth
                  />
                </Grid>

                <Grid size={{ xs: 12 }}>
                  <TextField
                    label="Komentarz"
                    {...register("comment")}
                    multiline
                    rows={2}
                    fullWidth
                  />
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions>
          <Button onClick={handleClose}>Anuluj</Button>
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
  );
};

export default AddGasDeliveryModal;
