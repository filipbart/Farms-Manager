import {
  Box,
  Button,
  Grid,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Divider,
  Typography,
  Switch,
  FormControlLabel,
} from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { useEffect, useState } from "react";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import { UsersService } from "../../../../../services/users-service";
import { handleApiResponse } from "../../../../../utils/axios/handle-api-response";
import type {
  UpdateUserData,
  UserDetailsModel,
} from "../../../../../models/users/users";

interface UserInfoTabProps {
  user: UserDetailsModel;
  refetch: () => void;
}

interface UserFormState {
  name: string;
  isAdmin: boolean;
  newPassword?: string;
  confirmPassword?: string;
}

const UserInfoTab: React.FC<UserInfoTabProps> = ({ user, refetch }) => {
  const [isPasswordConfirmOpen, setPasswordConfirmOpen] = useState(false);
  const [isAdminConfirmOpen, setAdminConfirmOpen] = useState(false);

  const {
    handleSubmit,
    register,
    reset,
    watch,
    control,
    formState: { errors, isDirty },
    getValues,
  } = useForm<UserFormState>({
    defaultValues: {
      name: "",
      isAdmin: false,
      newPassword: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    if (user) {
      reset({
        name: user.name,
        isAdmin: user.isAdmin,
        newPassword: "",
        confirmPassword: "",
      });
    }
  }, [user, reset]);

  const onSubmit = async (data: UserFormState) => {
    if (!user) return;

    const payload: UpdateUserData = {
      name: data.name,
      isAdmin: data.isAdmin,
      password: data.newPassword || undefined,
    };

    try {
      await handleApiResponse(
        () => UsersService.updateUser(user.id, payload),
        () => {
          toast.success("Dane użytkownika zostały zaktualizowane");
          refetch();
          reset({
            name: data.name,
            isAdmin: data.isAdmin,
            newPassword: "",
            confirmPassword: "",
          });
        },
        undefined,
        "Nie udało się zaktualizować danych"
      );
    } catch {
      toast.error("Wystąpił błąd podczas aktualizacji");
    } finally {
      setPasswordConfirmOpen(false);
      setAdminConfirmOpen(false);
    }
  };

  const handleSaveChangesClick = () => {
    const formValues = getValues();
    const isBecomingAdmin = formValues.isAdmin && !user.isAdmin;

    if (isBecomingAdmin) {
      setAdminConfirmOpen(true);
    } else {
      handleSubmit(onSubmit)();
    }
  };

  const handlePasswordChangeClick = () => {
    setPasswordConfirmOpen(true);
  };

  return (
    <>
      <Box component="form" onSubmit={handleSubmit(onSubmit)} mt={2}>
        <Grid container spacing={2.5} alignItems="center">
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Login"
              fullWidth
              value={user.login || ""}
              InputProps={{ readOnly: true }}
              variant="filled"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <TextField
              label="Imię i nazwisko"
              fullWidth
              error={!!errors.name}
              helperText={errors.name?.message}
              {...register("name", { required: "To pole jest wymagane" })}
            />
          </Grid>
          <Grid size={{ xs: 12 }}>
            <FormControlLabel
              control={
                <Controller
                  name="isAdmin"
                  control={control}
                  render={({ field }) => (
                    <Switch
                      {...field}
                      checked={field.value}
                      onChange={(e) => field.onChange(e.target.checked)}
                    />
                  )}
                />
              }
              label="Administrator"
            />
          </Grid>
        </Grid>

        <Box display="flex" justifyContent="flex-end" mt={3}>
          <Button
            type="button"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            disabled={!isDirty}
            onClick={handleSaveChangesClick}
          >
            Zapisz zmiany
          </Button>
        </Box>

        <Divider sx={{ my: 4 }} />

        <Typography variant="h6">Zmień hasło</Typography>
        <Grid container spacing={2.5} mt={0.5}>
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
        <Box display="flex" justifyContent="flex-end" mt={3}>
          <Button
            variant="contained"
            color="warning"
            onClick={handlePasswordChangeClick}
            disabled={
              !watch("newPassword") ||
              !!errors.newPassword ||
              !!errors.confirmPassword
            }
          >
            Zmień hasło
          </Button>
        </Box>
      </Box>

      {/* Okno dialogowe do potwierdzenia zmiany hasła */}
      <Dialog
        open={isPasswordConfirmOpen}
        onClose={() => setPasswordConfirmOpen(false)}
      >
        <DialogTitle>Potwierdzenie zmiany hasła</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Czy na pewno chcesz zmienić hasło dla użytkownika "{user.name}"?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPasswordConfirmOpen(false)} color="inherit">
            Anuluj
          </Button>
          <Button onClick={handleSubmit(onSubmit)} color="primary" autoFocus>
            Potwierdź
          </Button>
        </DialogActions>
      </Dialog>

      {/* okno dialogowe do potwierdzenia nadania uprawnień admina */}
      <Dialog
        open={isAdminConfirmOpen}
        onClose={() => setAdminConfirmOpen(false)}
      >
        <DialogTitle>Potwierdzenie nadania uprawnień</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Czy na pewno chcesz nadać pełne uprawnienia administratora
            użytkownikowi "{user.name}"? Ta akcja da mu dostęp do wszystkich
            sekcji systemu.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAdminConfirmOpen(false)} color="inherit">
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

export default UserInfoTab;
