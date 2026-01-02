import { Box, Button, Modal, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import type {
  TaxBusinessEntityRowModel,
  UpdateTaxBusinessEntityFormData,
} from "../../../models/data/tax-business-entity";
import { TaxBusinessEntitiesService } from "../../../services/tax-business-entities-service";

interface EditTaxBusinessEntityModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  entityData: TaxBusinessEntityRowModel | null;
}

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 500,
  bgcolor: "background.paper",
  boxShadow: 24,
  p: 4,
  borderRadius: 2,
};

const EditTaxBusinessEntityModal: React.FC<EditTaxBusinessEntityModalProps> = ({
  open,
  onClose,
  onSave,
  entityData,
}) => {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<UpdateTaxBusinessEntityFormData>();

  useEffect(() => {
    if (entityData) {
      reset({
        name: entityData.name,
        businessType: entityData.businessType,
        description: entityData.description || "",
      });
    }
  }, [entityData, reset]);

  const onSubmit: SubmitHandler<UpdateTaxBusinessEntityFormData> = async (
    data
  ) => {
    if (!entityData) return;

    await handleApiResponse(
      () => TaxBusinessEntitiesService.updateAsync(entityData.id, data),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych podmiotu"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj podmiot
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          NIP: {entityData?.nip}
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
            label="Typ działalności"
            {...register("businessType", { required: "To pole jest wymagane" })}
            error={!!errors.businessType}
            helperText={errors.businessType?.message}
            placeholder="np. Ferma Drobiu, Gospodarstwo Rolne, Najem"
          />
          <TextField
            fullWidth
            margin="normal"
            label="Opis (opcjonalnie)"
            {...register("description")}
            multiline
            rows={3}
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

export default EditTaxBusinessEntityModal;
