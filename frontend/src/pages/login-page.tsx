import React, { useState } from "react";
import logo from "../assets/logo.png";
import {
  TextField,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { useAuth } from "../auth/useAuth";
import { AuthService } from "../services/auth-service";
import { toast } from "react-toastify";

const LoginPage: React.FC = () => {
  const auth = useAuth();
  const [loading, setLoading] = useState(false);
  const [showPasswordWarning, setShowPasswordWarning] = useState(false);
  const nav = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const handleLogin = async (val: any) => {
    if (loading) return;
    setLoading(true);

    try {
      const response = await AuthService.login(val.login, val.password);

      if (!response.success) {
        toast.error(
          response.domainException?.errorDescription ||
            "Nie udało się zalogować. Sprawdź dane logowania."
        );
        return;
      }

      auth.setAuthToken(response.responseData?.accessToken || "");
      await auth.fetchUserData();

      if (response.responseData?.mustChangePassword) {
        setShowPasswordWarning(true);
      } else {
        toast.success("Zalogowano pomyślnie!");
        nav("/");
      }
    } catch {
      toast.error("Wystąpił nieoczekiwany błąd logowania.");
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmPasswordChange = () => {
    setShowPasswordWarning(false);
    nav("/user-profile");
  };

  return (
    <div
      className="flex items-center justify-center h-screen"
      style={{ backgroundColor: "#E0E1DD" }}
    >
      <div className="bg-white p-8 rounded-lg shadow-md w-96">
        <img src={logo} alt="Logo" className="mb-4 mx-auto w-32 h-32" />
        <h2 className="text-2xl font-bold mb-6 text-center">Logowanie</h2>
        <form onSubmit={handleSubmit(handleLogin)}>
          <div className="mb-4">
            <TextField
              label="Login"
              variant="outlined"
              fullWidth
              className="mb-4"
              placeholder="Wprowadź login"
              error={!!errors?.login}
              helperText={errors.login ? (errors.login.message as string) : ""}
              {...register("login", {
                required: "Login jest wymagany",
              })}
            />
          </div>
          <div className="mb-6">
            <TextField
              label="Hasło"
              type="password"
              variant="outlined"
              fullWidth
              className="mb-4"
              placeholder="Wprowadź hasło"
              error={!!errors?.password}
              helperText={
                errors.password ? (errors.password.message as string) : ""
              }
              {...register("password", {
                required: "Hasło jest wymagane",
              })}
            />
          </div>
          <Button
            disabled={loading}
            type="submit"
            variant="contained"
            color="primary"
            className="w-full"
          >
            {loading ? "Logowanie..." : "Zaloguj"}
          </Button>
        </form>
      </div>

      <Dialog
        open={showPasswordWarning}
        onClose={handleConfirmPasswordChange}
        aria-labelledby="password-change-dialog-title"
      >
        <DialogTitle id="password-change-dialog-title">
          Wymagana zmiana hasła
        </DialogTitle>
        <DialogContent>
          <Typography>
            Twoje konto wymaga natychmiastowej zmiany hasła ze względów
            bezpieczeństwa. Kliknij "OK", aby kontynuować.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={handleConfirmPasswordChange}
            variant="contained"
            autoFocus
          >
            OK
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default LoginPage;
