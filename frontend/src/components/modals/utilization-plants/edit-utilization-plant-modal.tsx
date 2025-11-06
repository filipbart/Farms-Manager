import { Box, Button, Modal, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { UtilizationPlantsService } from "../../../services/utilization-plants-service";
import { isValidProducerNumber } from "../../../utils/validation";

export interface UtilizationPlantData {
  id: string;
  name: string;
  irzNumber: string;
  nip: string;
  address: string;
}

export type UtilizationPlantFormValues = {
  name: string;
  irzNumber: string;
  nip: string;
  address: string;
};

interface EditUtilizationPlantModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  utilizationPlantData: UtilizationPlantData | null;
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

const EditUtilizationPlantModal: React.FC<EditUtilizationPlantModalProps> = ({
  open,
  onClose,
  onSave,
  utilizationPlantData,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<UtilizationPlantFormValues>();

  useEffect(() => {
    if (utilizationPlantData) {
      reset(utilizationPlantData);
    }
  }, [utilizationPlantData, reset]);

  const onSubmit: SubmitHandler<UtilizationPlantFormValues> = async (data) => {
    if (!utilizationPlantData) return;

    await handleApiResponse(
      () =>
        UtilizationPlantsService.updateUtilizationPlantAsync(
          utilizationPlantData.id,
          data
        ),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych zakładu utylizacyjnego"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj zakład utylizacyjny
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
            label="Numer IRZ"
            {...register("irzNumber", {
              required: "To pole jest wymagane",
              validate: (value) =>
                isValidProducerNumber(value) || "Numer IRZ musi mieć format: liczba-liczba (np. 000111222-012)",
            })}
            error={!!errors.irzNumber}
            helperText={errors.irzNumber?.message}
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

export default EditUtilizationPlantModal;
