import React, { useEffect, useState } from "react";
import {
  TextField,
  Box,
  Typography,
  IconButton,
  InputAdornment,
} from "@mui/material";
import LoadingButton from "../../../components/common/loading-button";
import { useForm } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { SettingsService } from "../../../services/settings-service";
import { toast } from "react-toastify";
import type UserDetails from "../../../models/user/user-details";
import { MdSave, MdVisibility, MdVisibilityOff } from "react-icons/md";

interface IrzPlusSettingsTabProps {
  userDetails?: UserDetails;
  onReload: () => void;
}

const IrzPlusSettingsTab: React.FC<IrzPlusSettingsTabProps> = ({
  userDetails,
  onReload,
}) => {
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm();

  const setCredentials = () => {
    reset({
      login: userDetails?.irzplusCredentials?.login || "",
      password: userDetails?.irzplusCredentials?.password || "",
    });
  };

  useEffect(() => {
    setCredentials();
  }, []);

  const handleSave = async (val: any) => {
    if (loading) return;

    setLoading(true);
    try {
      await handleApiResponse(
        () =>
          SettingsService.saveIrzPlusCredentials({
            login: val.login,
            password: val.password,
          }),
        () => {
          onReload();
          toast.success("Dane logowania zostały zapisane pomyślnie");
        },
        undefined,
        "Błąd podczas zapisywania danych logowania do IRZplus"
      );
    } catch (error) {
      toast.error("Błąd podczas pobierania danych logowania");
    }
    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit(handleSave)}>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "flex-start",
          gap: 0.5,
          p: 1,
          m: 1,
        }}
      >
        <Typography variant="h6">Dane logowania do systemu IRZplus:</Typography>

        <TextField
          sx={{ width: "250px" }}
          label="Login"
          variant="outlined"
          error={!!errors?.login}
          helperText={errors.login ? (errors.login.message as string) : ""}
          {...register("login")}
          margin="normal"
        />
        <TextField
          label="Hasło"
          sx={{ width: "250px" }}
          variant="outlined"
          type={showPassword ? "text" : "password"}
          slotProps={{
            input: {
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword((prev) => !prev)}
                    edge="end"
                  >
                    {showPassword ? <MdVisibilityOff /> : <MdVisibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          error={!!errors?.password}
          helperText={
            errors.password ? (errors.password.message as string) : ""
          }
          {...register("password")}
          margin="normal"
        />
        <LoadingButton
          startIcon={<MdSave />}
          variant="contained"
          color="primary"
          type="submit"
          loading={loading}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </form>
  );
};

export default IrzPlusSettingsTab;
