import { Box, Button, Modal, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { SlaughterhousesService } from "../../../services/slaughterhouses-service";
import { isValidProducerNumber } from "../../../utils/validation";

export interface SlaughterhouseData {
  id: string;
  name: string;
  producerNumber: string;
  nip: string;
  address: string;
}

export type SlaughterhouseFormValues = {
  name: string;
  producerNumber: string;
  nip: string;
  address: string;
};

interface EditSlaughterhouseModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  slaughterhouseData: SlaughterhouseData | null;
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

const EditSlaughterhouseModal: React.FC<EditSlaughterhouseModalProps> = ({
  open,
  onClose,
  onSave,
  slaughterhouseData,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<SlaughterhouseFormValues>();

  useEffect(() => {
    if (slaughterhouseData) {
      reset(slaughterhouseData);
    }
  }, [slaughterhouseData, reset]);

  const onSubmit: SubmitHandler<SlaughterhouseFormValues> = async (data) => {
    if (!slaughterhouseData) return;

    await handleApiResponse(
      () =>
        SlaughterhousesService.updateSlaughterhouseAsync(
          slaughterhouseData.id,
          data
        ),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych ubojni"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj ubojnię
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
              validate: (value) =>
                isValidProducerNumber(value) || "Numer producenta musi mieć format: liczba-liczba (np. 000111222-012)",
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

export default EditSlaughterhouseModal;
