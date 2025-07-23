import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  TextField,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { useState, useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import type { UpdateCorrectionData } from "../../../../models/feeds/corrections/correction";
import { FeedsService } from "../../../../services/feeds-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import LoadingButton from "../../../common/loading-button";
import type { FeedDeliveryListModel } from "../../../../models/feeds/deliveries/feed-invoice";

interface EditCorrectionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  correction: FeedDeliveryListModel | null;
}

const EditCorrectionModal: React.FC<EditCorrectionModalProps> = ({
  open,
  onClose,
  onSave,
  correction,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors },
  } = useForm<UpdateCorrectionData>({
    defaultValues: {
      id: "",
      invoiceNumber: "",
      subTotal: 0,
      vatAmount: 0,
      invoiceTotal: 0,
      invoiceDate: "",
    },
  });

  useEffect(() => {
    if (correction) {
      reset({
        id: correction.id,
        invoiceNumber: correction.invoiceNumber,
        subTotal: correction.subTotal,
        vatAmount: correction.vatAmount,
        invoiceTotal: correction.invoiceTotal,
        invoiceDate: correction.invoiceDate,
      });
    }
  }, [correction]);

  const handleUpdate = async (data: UpdateCorrectionData) => {
    if (!correction) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        FeedsService.updateFeedCorrection({
          id: correction.id,
          invoiceNumber: data.invoiceNumber,
          subTotal: data.subTotal,
          vatAmount: data.vatAmount,
          invoiceTotal: data.invoiceTotal,
          invoiceDate: data.invoiceDate,
        }),
      () => {
        toast.success("Faktura korekty została zaktualizowana");
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas aktualizacji faktury korekty"
    );
    setLoading(false);
  };

  return (
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason !== "backdropClick") onClose();
      }}
      maxWidth="sm"
      fullWidth
    >
      <DialogTitle>Edytuj fakturę korekty</DialogTitle>

      <form onSubmit={handleSubmit(handleUpdate)}>
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
              <Controller
                name="invoiceDate"
                control={control}
                rules={{ required: "Data korekty jest wymagana" }}
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

            <Grid size={{ xs: 4 }}>
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

            <Grid size={{ xs: 4 }}>
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

            <Grid size={{ xs: 4 }}>
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

export default EditCorrectionModal;
