import { Box, Button, Modal, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { FarmsService } from "../../../services/farms-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";

export interface HenhouseData {
  id: string;
  name: string;
  code: string;
  area: number;
  description: string;
}

export type HenhouseFormValues = {
  name: string;
  code: string;
  area: number;
  description: string;
};

interface EditHenhouseModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  henhouseData: HenhouseData | null;
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

const EditHenhouseModal: React.FC<EditHenhouseModalProps> = ({
  open,
  onClose,
  onSave,
  henhouseData,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<HenhouseFormValues>();

  useEffect(() => {
    if (henhouseData) {
      reset(henhouseData);
    }
  }, [henhouseData, reset]);

  const onSubmit: SubmitHandler<HenhouseFormValues> = async (data) => {
    if (!henhouseData) return;

    // Zakładamy, że istnieje metoda do aktualizacji kurnika w serwisie
    await handleApiResponse(
      () => FarmsService.updateHenhouseAsync(henhouseData.id, data),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych kurnika"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj kurnik
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
            label="ID Budynku"
            {...register("code", { required: "To pole jest wymagane" })}
            error={!!errors.code}
            helperText={errors.code?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Powierzchnia (m²)"
            type="number"
            {...register("area", {
              required: "To pole jest wymagane",
              valueAsNumber: true,
              min: { value: 0, message: "Powierzchnia nie może być ujemna" },
            })}
            error={!!errors.area}
            helperText={errors.area?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Opis"
            multiline
            rows={3}
            {...register("description")}
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

export default EditHenhouseModal;
