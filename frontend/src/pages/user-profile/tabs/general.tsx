import {
  Typography,
  Button,
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  Box,
  Divider,
  Dialog,
  DialogContentText,
} from "@mui/material";
import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { useAuth } from "../../../auth/useAuth";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { UpdateMyData } from "../../../models/user/user-details";
import { UserService } from "../../../services/user-service";
import AvatarUploader from "../../../components/user/avatar-uploader";

interface ProfileFormState {
  name: string;
  newPassword?: string;
  confirmPassword?: string;
}

const GeneralSettingsTab: React.FC = () => {
  const { userData, logout, fetchUserData } = useAuth();
  const [isConfirmOpen, setConfirmOpen] = useState(false);

  const {
    handleSubmit,
    register,
    reset,
    watch,
    getValues,
    formState: { errors, isDirty },
  } = useForm<ProfileFormState>({
    defaultValues: {
      name: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    if (userData) {
      reset({
        name: userData.name,
        newPassword: "",
        confirmPassword: "",
      });
    }
  }, [userData, reset]);

  const onSubmit = async (data: ProfileFormState) => {
    if (!userData) return;

    const payload: UpdateMyData = {
      name: data.name,
      password: data.newPassword || undefined,
    };

    try {
      await handleApiResponse(
        () => UserService.updateData(payload),
        () => {
          if (payload.password) {
            toast.success("Hasło zostało zmienione. Zaloguj się ponownie.");
            logout();
          } else {
            toast.success("Twoje dane zostały zaktualizowane");
            fetchUserData();
          }
        },
        undefined,
        "Nie udało się zaktualizować danych"
      );
    } catch {
      toast.error("Wystąpił błąd podczas aktualizacji");
    } finally {
      setConfirmOpen(false);
    }
  };

  const handleSaveClick = () => {
    if (getValues("newPassword")) {
      setConfirmOpen(true);
    } else {
      handleSubmit(onSubmit)();
    }
  };

  return (
    <>
      <Box component="form" onSubmit={handleSubmit(onSubmit)}>
        <Typography variant="h5" mb={3}>
          Mój profil
        </Typography>

        <Grid container spacing={3} alignItems="center">
          <Grid size={{ xs: 12, md: 4 }} display="flex" justifyContent="center">
            <AvatarUploader />
          </Grid>
          <Grid size={{ xs: 12, md: 8 }}></Grid>
          <Grid size={{ xs: 12, md: 3 }}>
            <TextField
              label="Imię i nazwisko"
              fullWidth
              error={!!errors.name}
              helperText={errors.name?.message}
              {...register("name", { required: "To pole jest wymagane" })}
            />
          </Grid>
        </Grid>

        <Divider sx={{ my: 4 }} />

        <Typography variant="h5" mb={3}>
          Zmień hasło
        </Typography>
        <Grid container spacing={2.5}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Nowe hasło"
              type="password"
              fullWidth
              error={!!errors.newPassword}
              helperText={errors.newPassword?.message}
              {...register("newPassword", {
                minLength: {
                  value: 8,
                  message: "Hasło musi mieć co najmniej 8 znaków",
                },
              })}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Potwierdź nowe hasło"
              type="password"
              fullWidth
              error={!!errors.confirmPassword}
              helperText={errors.confirmPassword?.message}
              {...register("confirmPassword", {
                validate: (value) =>
                  value === watch("newPassword") ||
                  "Hasła muszą być takie same",
              })}
            />
          </Grid>
        </Grid>
        <Box display="flex" justifyContent="flex-end" mt={4}>
          <Button
            type="button"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            disabled={!isDirty}
            onClick={handleSaveClick}
          >
            Zapisz zmiany
          </Button>
        </Box>
      </Box>

      <Dialog open={isConfirmOpen} onClose={() => setConfirmOpen(false)}>
        <DialogTitle>Potwierdzenie zmiany hasła</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Czy na pewno chcesz zmienić swoje hasło? Po tej operacji zostaniesz
            wylogowany/a.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmOpen(false)} color="inherit">
            Anuluj
          </Button>
          <Button onClick={handleSubmit(onSubmit)} color="primary" autoFocus>
            Potwierdź
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default GeneralSettingsTab;
