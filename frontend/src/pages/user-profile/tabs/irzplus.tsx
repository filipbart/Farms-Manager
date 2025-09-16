import React, { useEffect, useState } from "react";
import {
  TextField,
  Box,
  Typography,
  IconButton,
  InputAdornment,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from "@mui/material";
import LoadingButton from "../../../components/common/loading-button";
import { useForm } from "react-hook-form";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { SettingsService } from "../../../services/settings-service";
import { toast } from "react-toastify";
import type UserDetails from "../../../models/user/user-details";
import { MdSave, MdVisibility, MdVisibilityOff } from "react-icons/md";
import { useFarms } from "../../../hooks/useFarms";

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

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [selectedFarmId, setSelectedFarmId] = useState<string>("");

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm();

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  useEffect(() => {
    if (farms.length > 0 && !selectedFarmId) {
      setSelectedFarmId(farms[0].id);
    }
  }, [farms, selectedFarmId]);

  useEffect(() => {
    if (!userDetails || !selectedFarmId) {
      reset({ login: "", password: "" });
      return;
    }

    const credentialsForFarm = userDetails.irzplusCredentials?.find(
      (cred: any) => cred.farmId === selectedFarmId
    );

    reset({
      login: credentialsForFarm?.login || "",
      password: credentialsForFarm?.password || "",
    });
  }, [userDetails, selectedFarmId, reset]);

  const handleSave = async (val: any) => {
    if (loading || !selectedFarmId) return;

    setLoading(true);
    try {
      await handleApiResponse(
        () =>
          SettingsService.saveIrzPlusCredentials({
            farmId: selectedFarmId,
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
    } catch {
      toast.error("Błąd podczas zapisywania danych logowania");
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

        <FormControl fullWidth margin="normal" sx={{ maxWidth: "350px" }}>
          <InputLabel>Wybrana ferma</InputLabel>
          <Select
            label="Wybrana ferma"
            value={selectedFarmId}
            onChange={(e) => setSelectedFarmId(e.target.value as string)}
            disabled={loadingFarms}
          >
            {loadingFarms && <MenuItem disabled>Ładowanie ferm...</MenuItem>}
            {farms.map((farm) => (
              <MenuItem key={farm.id} value={farm.id}>
                {farm.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <TextField
          sx={{ width: "250px" }}
          label="Login"
          variant="outlined"
          error={!!errors?.login}
          autoComplete="off"
          helperText={errors.login ? (errors.login.message as string) : ""}
          {...register("login")}
          margin="normal"
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label="Hasło"
          sx={{ width: "250px" }}
          variant="outlined"
          type={showPassword ? "text" : "password"}
          autoComplete="off"
          error={!!errors?.password}
          helperText={
            errors.password ? (errors.password.message as string) : ""
          }
          {...register("password")}
          margin="normal"
          slotProps={{
            inputLabel: { shrink: true },
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
        />
        <LoadingButton
          height="20"
          startIcon={<MdSave />}
          variant="contained"
          color="primary"
          type="submit"
          loading={loading}
          disabled={!selectedFarmId}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </form>
  );
};

export default IrzPlusSettingsTab;
