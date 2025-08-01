import {
  Box,
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
} from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useFarms } from "../../../hooks/useFarms";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import {
  InsertionsService,
  type AddCycleData,
} from "../../../services/insertions-service";
import { toast } from "react-toastify";
import LoadingButton from "../../common/loading-button";
import LoadingTextField from "../../common/loading-textfield";
import { MdSave } from "react-icons/md";
import { useLatestCycle } from "../../../hooks/useLatestCycle";
import AppDialog from "../../common/app-dialog";

interface SetCycleModalProps {
  open: boolean;
  onClose: () => void;
}

const SetCycleModal: React.FC<SetCycleModalProps> = ({ open, onClose }) => {
  const [loadingNewCycle, setLoadingNewCycle] = useState(false);
  const [chosenFarm, setChosenFarm] = useState<FarmRowModel>();
  const setChosenFarmCallback = useCallback(
    async (chosenFarm: FarmRowModel) => {
      setChosenFarm(chosenFarm);
      await getLatestCycle(chosenFarm.id);
    },
    []
  );

  const { loadLatestCycle, loadingCycle } = useLatestCycle();

  const [cycle, setCycle] = useState<AddCycleData>();
  const [cycleText, setCycleText] = useState<string>("");

  const getLatestCycle = async (farmId: string) => {
    const latest = await loadLatestCycle(farmId);
    const now = new Date();

    const identifier = latest ? latest.identifier + 1 : 1;
    const year =
      latest?.year !== now.getFullYear() ? now.getFullYear() : latest?.year;

    const newCycle = { farmId, identifier, year };
    setCycle(newCycle);
    setCycleText(`${newCycle.identifier}/${newCycle.year}`);
  };

  const handleClose = () => {
    setChosenFarm(undefined);
    setCycle(undefined);
    setCycleText("");
    onClose();
  };

  const handleSave = async () => {
    if (loadingNewCycle || !cycle) return;
    setLoadingNewCycle(true);
    await handleApiResponse(
      () => InsertionsService.addNewCycle(cycle),
      () => {
        toast.success("Dodano nowy cykl");
        setLoadingNewCycle(false);
        handleClose();
      },
      undefined,
      "Wystąpił błąd podczas dodawania cyklu"
    );

    if (!loadingNewCycle) setLoadingNewCycle(false);
  };

  const { farms, loadingFarms, fetchFarms } = useFarms();

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Ustaw nowy cykl</DialogTitle>
      <DialogContent sx={{ mt: 1 }}>
        <Box display="flex" flexDirection="column" gap={3}>
          <Box mt={1}>
            <LoadingTextField
              loading={loadingFarms}
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
              {farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Box>

          <Box>
            <LoadingTextField
              loading={loadingCycle}
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
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button
          onClick={handleClose}
          variant="outlined"
          color="inherit"
          disabled={loadingNewCycle}
        >
          Anuluj
        </Button>
        <LoadingButton
          onClick={handleSave}
          variant="contained"
          color="primary"
          loading={loadingNewCycle}
          disabled={loadingNewCycle}
          startIcon={<MdSave />}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default SetCycleModal;
