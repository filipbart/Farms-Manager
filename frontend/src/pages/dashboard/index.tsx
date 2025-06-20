import { Box, Typography } from "@mui/material";
import React from "react";

const DashboardPage: React.FC = () => {
  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4">Dashboard</Typography>
      <p>Welcome to the dashboard!</p>
      {/* Add more components or features here */}
    </Box>
  );
};

export default DashboardPage;
