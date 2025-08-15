import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  Button,
} from "@mui/material";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { UsersService } from "../../../services/users-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import AppDialog from "../../common/app-dialog";
import LoadingButton from "../../common/loading-button";
import type { AddUserData } from "../../../models/users/users";

interface AddUserModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddUserModal: React.FC<AddUserModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddUserData>();

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: AddUserData) => {
    setLoading(true);
    await handleApiResponse(
      () => UsersService.addUser(data),
      () => {
        toast.success("Pomyślnie dodano użytkownika");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania użytkownika"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Dodaj nowego użytkownika</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Login"
                fullWidth
                error={!!errors.login}
                helperText={errors.login?.message}
                {...register("login", {
                  required: "Login jest wymagany",
                })}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Imię i nazwisko"
                fullWidth
                error={!!errors.name}
                helperText={errors.name?.message}
                {...register("name", {
                  required: "Imię i nazwisko jest wymagane",
                })}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Hasło tymczasowe"
                type="password"
                fullWidth
                error={!!errors.temporaryPassword}
                helperText={errors.temporaryPassword?.message}
                {...register("temporaryPassword", {
                  required: "Hasło jest wymagane",
                })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} variant="outlined" color="inherit">
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
    </AppDialog>
  );
};

export default AddUserModal;
