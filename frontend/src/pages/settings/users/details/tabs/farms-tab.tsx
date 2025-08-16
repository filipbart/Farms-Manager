import {
  Box,
  Button,
  Grid,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  Chip,
} from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { useEffect } from "react";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { UsersService } from "../../../../../services/users-service";
import { useFarms } from "../../../../../hooks/useFarms";
import { handleApiResponse } from "../../../../../utils/axios/handle-api-response";
import type { UserDetailsModel } from "../../../../../models/users/users";

interface FarmsTabProps {
  user: UserDetailsModel;
  refetch: () => void;
}

interface FarmsFormState {
  farmIds: string[];
}

const FarmsTab: React.FC<FarmsTabProps> = ({ user, refetch }) => {
  const { farms, fetchFarms, loadingFarms } = useFarms();

  const {
    handleSubmit,
    control,
    reset,
    formState: { isDirty },
  } = useForm<FarmsFormState>({
    defaultValues: { farmIds: [] },
  });

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  useEffect(() => {
    if (user?.accessibleFarmIds) {
      reset({
        farmIds: user.accessibleFarmIds,
      });
    }
  }, [user, reset]);

  const onSubmit = async (data: FarmsFormState) => {
    if (!user) return;

    try {
      await handleApiResponse(
        () => UsersService.updateUserFarms(user.id, data.farmIds),
        () => {
          toast.success("Dostęp do ferm został zaktualizowany");
          refetch();
          reset(data);
        },
        undefined,
        "Nie udało się zapisać dostępu do ferm"
      );
    } catch {
      toast.error("Wystąpił błąd podczas zapisu");
    }
  };

  if (loadingFarms) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} mt={2}>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12 }}>
          <FormControl fullWidth>
            <InputLabel id="farm-select-label">Dostępne fermy</InputLabel>
            <Controller
              name="farmIds"
              control={control}
              render={({ field }) => (
                <Select
                  labelId="farm-select-label"
                  multiple
                  value={field.value}
                  onChange={field.onChange}
                  input={<OutlinedInput label="Dostępne fermy" />}
                  renderValue={(selected) => (
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                      {selected.map((value) => {
                        const farmName =
                          farms.find((f) => f.id === value)?.name ?? value;
                        return <Chip key={value} label={farmName} />;
                      })}
                    </Box>
                  )}
                >
                  {farms.map((farm) => (
                    <MenuItem key={farm.id} value={farm.id}>
                      {farm.name}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
          </FormControl>
        </Grid>
      </Grid>
      <Box display="flex" justifyContent="flex-end" mt={3}>
        <Button
          type="submit"
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          disabled={!isDirty}
        >
          Zapisz dostęp
        </Button>
      </Box>
    </Box>
  );
};

export default FarmsTab;
