import { Box, Button, Modal, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { HatcheriesService } from "../../../services/hatcheries-service";

export interface HatcheryData {
  id: string;
  name: string;
  producerNumber: string;
  fullName: string;
  nip: string;
  address: string;
}

export type HatcheryFormValues = {
  name: string;
  producerNumber: string;
  fullName: string;
  nip: string;
  address: string;
};

interface EditHatcheryModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  hatcheryData: HatcheryData | null;
}

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  bgcolor: "background.paper",
  boxShadow: 24,
  p: 4,
  borderRadius: 2,
};

const EditHatcheryModal: React.FC<EditHatcheryModalProps> = ({
  open,
  onClose,
  onSave,
  hatcheryData,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<HatcheryFormValues>();

  useEffect(() => {
    if (hatcheryData) {
      reset(hatcheryData);
    }
  }, [hatcheryData, reset]);

  const onSubmit: SubmitHandler<HatcheryFormValues> = async (data) => {
    if (!hatcheryData) return;

    await handleApiResponse(
      () => HatcheriesService.updateHatcheryAsync(hatcheryData.id, data),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych wylęgarni"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj wylęgarnię
        </Typography>
        <Box
          mt={2}
          component="form"
          noValidate
          autoComplete="off"
          onSubmit={handleSubmit(onSubmit)}
        >
          <TextField
            fullWidth
            margin="normal"
            label="Nazwa"
            {...register("name", { required: "To pole jest wymagane" })}
            error={!!errors.name}
            helperText={errors.name?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Pełna nazwa"
            {...register("fullName", { required: "To pole jest wymagane" })}
            error={!!errors.fullName}
            helperText={errors.fullName?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="NIP"
            {...register("nip", {
              required: "To pole jest wymagane",
              pattern: {
                value: /^[0-9]{10}$/,
                message: "NIP musi składać się z 10 cyfr",
              },
            })}
            error={!!errors.nip}
            helperText={errors.nip?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Numer producenta"
            {...register("producerNumber", {
              required: "To pole jest wymagane",
            })}
            error={!!errors.producerNumber}
            helperText={errors.producerNumber?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Adres"
            {...register("address")}
            error={!!errors.address}
            helperText={errors.address?.message}
          />

          <Box
            mt={3}
            display="flex"
            justifyContent="flex-end"
            alignItems="center"
            gap={2}
          >
            <Button onClick={onClose} color="secondary">
              Anuluj
            </Button>
            <LoadingButton
              type="submit"
              variant="contained"
              color="primary"
              startIcon={<MdSave />}
              disabled={isSubmitting}
              loading={isSubmitting}
            >
              Zapisz zmiany
            </LoadingButton>
          </Box>
        </Box>
      </Box>
    </Modal>
  );
};

export default EditHatcheryModal;
