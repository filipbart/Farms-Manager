import React, { useEffect, useRef } from "react";
import type { FallbackProps } from "react-error-boundary";
import { useLocation, useNavigate } from "react-router-dom";
import Button from "@mui/material/Button"; // Importuj przycisk z MUI
import { Typography } from "@mui/material";

const ErrorFallback: React.FC<FallbackProps> = ({
  error,
  resetErrorBoundary,
}) => {
  const nav = useNavigate();
  const location = useLocation();
  const originalLocation = useRef(location.pathname);

  useEffect(() => {
    if (location.pathname == originalLocation.current) return;
    console.log(error);
    resetErrorBoundary();
  }, [location.pathname]);

  const handleClick = () => {
    nav("/");
  };

  return (
    <div style={{ textAlign: "center", padding: "50px" }}>
      <Typography variant="h5">Coś poszło nie tak!</Typography>
      <div className="mt-4">
        <Button
          variant="contained" // Ustawienie stylu na "contained"
          color="primary" // Ustawienie koloru na "primary"
          onClick={handleClick}
          style={{ padding: "10px 20px", fontSize: "16px" }}
        >
          Powrót do strony głównej
        </Button>
      </div>
    </div>
  );
};

export default ErrorFallback;
