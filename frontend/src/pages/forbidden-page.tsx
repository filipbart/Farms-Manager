import React from "react";
import { Box, Typography, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";
import SentimentVeryDissatisfiedIcon from "@mui/icons-material/SentimentVeryDissatisfied";

const ForbiddenPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        textAlign: "center",
        minHeight: "80vh",
        p: 3,
      }}
    >
      <SentimentVeryDissatisfiedIcon
        sx={{
          fontSize: 100,
          color: "text.secondary",
          mb: 2,
        }}
      />
      <Typography variant="h4" component="h1" gutterBottom>
        403 - Brak dostępu
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Przepraszamy, nie masz uprawnień do wyświetlenia tych zasobów.
      </Typography>
      <Button variant="contained" color="primary" onClick={() => navigate("/")}>
        Wróć na stronę główną
      </Button>
    </Box>
  );
};

export default ForbiddenPage;
