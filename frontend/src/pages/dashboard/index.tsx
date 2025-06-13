import { Box, Button, Typography } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";

const DashboardPage: React.FC = () => {
  const nav = useNavigate();
  const handleLoginRedirect = () => {
    nav("/login");
  };
  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4">Dashboard</Typography>
      <p>Welcome to the dashboard!</p>
      {/* Add more components or features here */}

      <Button variant="contained" color="primary" onClick={handleLoginRedirect}>
        Strona logowania
      </Button>
    </Box>
  );
};

export default DashboardPage;
