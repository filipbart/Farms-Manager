import {
  Autocomplete,
  Box,
  Button,
  Modal,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { FarmsService } from "../../../services/farms-service";
import { TaxBusinessEntitiesService } from "../../../services/tax-business-entities-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { MdSave } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { isValidProducerNumber } from "../../../utils/validation";
import type { TaxBusinessEntityRowModel } from "../../../models/data/tax-business-entity";

export interface FarmData {
  id: string;
  name: string;
  nip: string;
  producerNumber: string;
  address: string;
  taxBusinessEntityId: string | null;
}

export type FarmFormValues = {
  name: string;
  nip: string;
  producerNumber: string;
  address: string;
  taxBusinessEntityId: string | null;
};

interface EditFarmModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  farmData: FarmData | null;
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

const EditFarmModal: React.FC<EditFarmModalProps> = ({
  open,
  onClose,
  onSave,
  farmData,
}) => {
  const [taxBusinessEntities, setTaxBusinessEntities] = useState<
    TaxBusinessEntityRowModel[]
  >([]);
  const [selectedEntity, setSelectedEntity] =
    useState<TaxBusinessEntityRowModel | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<FarmFormValues>();

  // Load tax business entities
  useEffect(() => {
    handleApiResponse(
      () => TaxBusinessEntitiesService.getAllAsync(),
      (data) => setTaxBusinessEntities(data.responseData?.items ?? []),
      () => setTaxBusinessEntities([])
    );
  }, []);

  useEffect(() => {
    if (farmData) {
      reset(farmData);
      // Set selected entity based on farmData.taxBusinessEntityId
      if (farmData.taxBusinessEntityId && taxBusinessEntities.length > 0) {
        const entity = taxBusinessEntities.find(
          (e) => e.id === farmData.taxBusinessEntityId
        );
        setSelectedEntity(entity || null);
      } else {
        setSelectedEntity(null);
      }
    }
  }, [farmData, reset, taxBusinessEntities]);

  const onSubmit: SubmitHandler<FarmFormValues> = async (data) => {
    if (!farmData) return;

    await handleApiResponse(
      () => FarmsService.updateFarmAsync(farmData.id, data),
      () => {
        onSave();
      },
      undefined,
      "Nie udało się zaktualizować danych fermy"
    );
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box sx={style}>
        <Typography variant="h6" component="h2">
          Edytuj fermę
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
            {...register("nip", { required: "To pole jest wymagane" })}
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
                isValidProducerNumber(value) ||
                "Numer producenta musi mieć format: liczba-liczba (np. 000111222-012)",
            })}
            error={!!errors.producerNumber}
            helperText={errors.producerNumber?.message}
          />
          <TextField
            fullWidth
            margin="normal"
            label="Adres"
            {...register("address")}
          />

          <Autocomplete
            options={taxBusinessEntities}
            getOptionLabel={(option) =>
              `${option.name} (${option.nip}) - ${option.businessType}`
            }
            value={selectedEntity}
            onChange={(_, value) => {
              setSelectedEntity(value);
              setValue("taxBusinessEntityId", value?.id || null);
            }}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Podmiot gospodarczy"
                margin="normal"
                fullWidth
              />
            )}
            isOptionEqualToValue={(option, value) => option.id === value.id}
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

export default EditFarmModal;
