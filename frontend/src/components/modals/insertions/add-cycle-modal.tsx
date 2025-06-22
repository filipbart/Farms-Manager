import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  TextField,
  Typography,
} from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useFarms } from "../../../hooks/useFarms";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type LatestCycle from "../../../models/farms/latest-cycle";
import Loading from "../../loading/loading";
import {
  FarmsService,
  type AddCycleData,
} from "../../../services/farms-service";

interface SetCycleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (data: { farmId: string; cycle: string }) => void;
}

const SetCycleModal: React.FC<SetCycleModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [loading, setLoading] = useState(false);
  const [chosenFarm, setChosenFarm] = useState<FarmRowModel>();
  const setChosenFarmCallback = useCallback(
    async (chosenFarm: FarmRowModel) => {
      setChosenFarm(chosenFarm);
      await getLatestCycle(chosenFarm.id);
    },
    []
  );

  const [cycle, setCycle] = useState<AddCycleData>();
  const [cycleText, setCycleText] = useState<string>("");

  const getLatestCycle = async (farmId: string) => {
    setLoading(true);

    handleApiResponse<LatestCycle>(
      () => FarmsService.getLatestCycle(farmId),
      (data) => {
        const now = new Date();
        const latest = data.responseData;
        const identifier = latest ? latest.identifier + 1 : 1;
        const year =
          latest?.year !== now.getFullYear() ? now.getFullYear() : latest?.year;

        const newCycle = { farmId, identifier, year };
        setCycle(newCycle);
        setCycleText(`${newCycle.identifier}/${newCycle.year}`);
      },
      undefined,
      "Nie udało się pobrać ostatniego cyklu"
    );
    setCycleText(cycle ? `${cycle.identifier}/${cycle.year}` : "");
    setLoading(false);
  };

  const handleClose = () => {
    setChosenFarm(undefined);
    setCycle(undefined);
    setCycleText("");
    onClose();
  };

  const handleSave = () => {
    onClose();
  };

  const { farms, loadingFarms, fetchFarms } = useFarms();

  useEffect(() => {
    fetchFarms();
  }, []);

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Ustaw nowy cykl</DialogTitle>
      <DialogContent sx={{ mt: 1 }}>
        <Box display="flex" flexDirection="column" gap={3}>
          <Box mt={1}>
            {loadingFarms ? (
              <Typography>Ładowanie farm...</Typography>
            ) : (
              <TextField
                select
                name="farm"
                label="Wybierz fermę"
                value={chosenFarm?.id || ""}
                onChange={(e) => {
                  const farmId = e.target.value;
                  const selectedFarm = farms.find((farm) => farm.id === farmId);
                  if (selectedFarm) {
                    setChosenFarmCallback(selectedFarm);
                  }
                }}
                fullWidth
              >
                <MenuItem disabled value="">
                  Wybierz Fermę
                </MenuItem>
                {farms.map((farm) => (
                  <MenuItem key={farm.id} value={farm.id}>
                    {farm.name}
                  </MenuItem>
                ))}
              </TextField>
            )}
          </Box>

          <Box>
            <>
              {loading ? (
                <Loading height="0" size={10} />
              ) : (
                <TextField
                  name="identifier"
                  label="Nowy cykl"
                  value={cycleText}
                  slotProps={{
                    input: {
                      readOnly: true,
                    },
                  }}
                  fullWidth
                />
              )}
            </>
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} variant="outlined" color="inherit">
          Anuluj
        </Button>
        <Button onClick={handleSave} variant="contained" color="primary">
          Zapisz
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default SetCycleModal;
