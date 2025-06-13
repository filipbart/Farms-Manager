import React from "react";
import logo from "../assets/logo-full.png";

import { TextField, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";

const LoginPage: React.FC = () => {
  const nav = useNavigate();
  const handleLogin = () => {
    nav("/");
  };
  return (
    <div
      className="flex items-center justify-center h-screen"
      style={{ backgroundColor: "#E0E1DD" }}
    >
      <div className="bg-white p-8 rounded-lg shadow-md w-96">
        <img src={logo} alt="Logo" className="mb-4 mx-auto w-32 h-32" />
        <h2 className="text-2xl font-bold mb-6 text-center">Logowanie</h2>
        <form>
          <div className="mb-4">
            <TextField
              label="Login"
              variant="outlined"
              fullWidth
              className="mb-4"
              placeholder="Wprowadź login"
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
            />
          </div>
          <Button
            onClick={handleLogin}
            //type="submit"
            variant="contained"
            color="primary"
            className="w-full"
          >
            Zaloguj
          </Button>
        </form>
      </div>
    </div>
  );
};

export default LoginPage;
