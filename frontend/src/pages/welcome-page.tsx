import { Box, Container, Typography, Paper } from "@mui/material";
import { FaHome } from "react-icons/fa";
import { useAuth } from "../auth/useAuth";

const WelcomePage: React.FC = () => {
  const { userData } = useAuth();

  return (
    <Container maxWidth="md">
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          minHeight: "80vh",
          py: 4,
        }}
      >
        <Paper
          elevation={3}
          sx={{
            p: 6,
            textAlign: "center",
            borderRadius: 2,
            maxWidth: 600,
            width: "100%",
          }}
        >
          <Box
            sx={{
              mb: 3,
              display: "flex",
              justifyContent: "center",
            }}
          >
            <FaHome size={80} color="#1976d2" />
          </Box>

          <Typography variant="h3" component="h1" gutterBottom>
            Witamy w systemie
          </Typography>

          <Typography variant="h5" color="text.secondary" sx={{ mb: 3 }}>
            {userData?.name || "Użytkowniku"}
          </Typography>

          <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
            Zostałeś pomyślnie zalogowany do systemu zarządzania fermami.
          </Typography>

          <Typography variant="body2" color="text.secondary">
            Skontaktuj się z administratorem systemu, aby uzyskać dostęp do
            dodatkowych modułów i funkcji.
          </Typography>
        </Paper>
      </Box>
    </Container>
  );
};

export default WelcomePage;
