import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Button,
  CircularProgress,
} from "@mui/material";
import { toast } from "react-toastify";
import { UserService } from "../../../services/user-service";
import { useFarms } from "../../../hooks/useFarms";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { useAuth } from "../../../auth/useAuth";

const NotificationSettingsTab: React.FC = () => {
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { userData, fetchUserData } = useAuth();
  const [selectedFarms, setSelectedFarms] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  useEffect(() => {
    if (userData?.notificationFarms) {
      setSelectedFarms(new Set(userData.notificationFarms));
    }
  }, [userData?.notificationFarms]);

  const handleToggleFarm = (farmId: string) => {
    setSelectedFarms((prev) => {
      const newSelection = new Set(prev);
      if (newSelection.has(farmId)) {
        newSelection.delete(farmId);
      } else {
        newSelection.add(farmId);
      }
      return newSelection;
    });
  };

  const handleSave = async () => {
    setLoading(true);
    await handleApiResponse(
      () => UserService.updateNotificationFarms(Array.from(selectedFarms)),
      async () => {
        toast.success("Ustawienia powiadomień zostały zapisane.");
        await fetchUserData();
      },
      undefined,
      "Nie udało się zapisać ustawień."
    );
    setLoading(false);
  };

  if (loadingFarms) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Subskrypcje powiadomień
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Wybierz fermy, z których chcesz otrzymywać powiadomienia. Jeśli nic nie
        zaznaczysz, będziesz otrzymywać powiadomienia ze wszystkich ferm, do
        których masz dostęp.
      </Typography>
      <FormGroup>
        {farms.map((farm) => (
          <FormControlLabel
            key={farm.id}
            control={
              <Checkbox
                checked={selectedFarms.has(farm.id)}
                onChange={() => handleToggleFarm(farm.id)}
              />
            }
            label={farm.name}
          />
        ))}
      </FormGroup>
      <Box sx={{ mt: 2, display: "flex", justifyContent: "flex-end" }}>
        <Button variant="contained" onClick={handleSave} disabled={loading}>
          Zapisz zmiany
        </Button>
      </Box>
    </Box>
  );
};

export default NotificationSettingsTab;
