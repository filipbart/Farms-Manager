import {
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
} from "@mui/material";
import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import { MdSave, MdAttachFile } from "react-icons/md";
import { useFarms } from "../../../../hooks/useFarms";
import type { CorrectionData } from "../../../../models/feeds/deliveries/correction";
import type { GridRowId } from "@mui/x-data-grid";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { FeedsService } from "../../../../services/feeds-service";
import { toast } from "react-toastify";

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
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
    setValue,
    watch,
  } = useForm<CorrectionData>({
    defaultValues: {
      invoiceNumber: "",
      farmId: "",
      subTotal: 0,
      vatAmount: 0,
      invoiceTotal: 0,
    },
  });

  useEffect(() => {
    fetchFarms();
  }, []);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      const file = e.target.files[0];
      setSelectedFile(file);
      setValue("file", file);
    }
  };

  const handleSave = async (data: CorrectionData) => {
    setLoading(true);
    await handleApiResponse(
      () =>
        FeedsService.addFeedCorrection({
          farmId: data.farmId,
          invoiceNumber: data.invoiceNumber,
          subTotal: data.subTotal,
          vatAmount: data.vatAmount,
          invoiceTotal: data.invoiceTotal,
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
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason !== "backdropClick") {
          onClose();
        }
      }}
      maxWidth="sm"
      fullWidth
    >
      <DialogTitle>Dodaj fakturę kosztową</DialogTitle>

      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
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
                {...register("farmId", { required: "Farma jest wymagana" })}
              >
                {farms.map((farm) => (
                  <MenuItem key={farm.id} value={farm.id}>
                    {farm.name}
                  </MenuItem>
                ))}
              </LoadingTextField>
            </Grid>

            <Grid size={{ xs: 4 }}>
              <TextField
                label="Netto [zł]"
                type="number"
                {...register("subTotal", {
                  required: "Wartość netto jest wymagana",
                  valueAsNumber: true,
                })}
                error={!!errors.subTotal}
                helperText={errors.subTotal?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 4 }}>
              <TextField
                label="VAT [zł]"
                type="number"
                {...register("vatAmount", {
                  required: "VAT jest wymagany",
                  valueAsNumber: true,
                })}
                error={!!errors.vatAmount}
                helperText={errors.vatAmount?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 4 }}>
              <TextField
                label="Brutto [zł]"
                type="number"
                {...register("invoiceTotal", {
                  required: "Wartość brutto jest wymagana",
                  valueAsNumber: true,
                })}
                error={!!errors.invoiceTotal}
                helperText={errors.invoiceTotal?.message}
                fullWidth
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Button
                variant="outlined"
                component="label"
                startIcon={<MdAttachFile />}
              >
                Wybierz plik
                <input type="file" hidden onChange={handleFileChange} />
              </Button>
              {selectedFile && (
                <Typography variant="body2" sx={{ mt: 1 }}>
                  Wybrano plik: {selectedFile.name}
                </Typography>
              )}
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions>
          <Button
            onClick={() => {
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
    </Dialog>
  );
};

export default AddCorrectionModal;
