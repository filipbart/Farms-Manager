import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Grid,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
} from "@mui/material";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { FallenStockService } from "../../../../services/production-data/fallen-stocks-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import AppDialog from "../../../common/app-dialog";
import LoadingButton from "../../../common/loading-button";
import Loading from "../../../loading/loading";
import type {
  FallenStockEditableEntry,
  GetFallenStockEditData,
} from "../../../../models/fallen-stocks/fallen-stocks";

interface EditFallenStocksModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  internalGroupId: string | null;
}

const EditFallenStocksModal: React.FC<EditFallenStocksModalProps> = ({
  open,
  onClose,
  onSave,
  internalGroupId,
}) => {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<Omit<
    GetFallenStockEditData,
    "entries"
  > | null>(null);
  const [entries, setEntries] = useState<FallenStockEditableEntry[]>([]);
  const [errors, setErrors] = useState<{
    [index: number]: { quantity?: string };
  }>({});

  useEffect(() => {
    const fetchFallenStockData = async () => {
      if (!internalGroupId) return;

      setLoading(true);
      await handleApiResponse(
        () => FallenStockService.getFallenStocksDataForEdit(internalGroupId),
        (data) => {
          const responseData = data.responseData;
          if (responseData) {
            setFormData({
              farmName: responseData.farmName,
              cycleDisplay: responseData.cycleDisplay,
              typeDesc: responseData.typeDesc,
              utilizationPlantName: responseData.utilizationPlantName,
              date: new Date(responseData.date).toLocaleDateString("pl-PL"),
            });
            setEntries(
              responseData.entries.map((e) => ({
                ...e,
                quantity: String(e.quantity),
              }))
            );
          }
        },
        undefined,
        "Nie udało się pobrać danych do edycji"
      );
      setLoading(false);
    };

    if (open) {
      fetchFallenStockData();
    }
  }, [open, internalGroupId]);

  const handleQuantityChange = (index: number, value: string) => {
    const updatedEntries = [...entries];
    updatedEntries[index].quantity = value;
    setEntries(updatedEntries);

    setErrors((prev) => {
      const updated = { ...prev };
      delete updated[index];
      return updated;
    });
  };

  const validate = (): boolean => {
    const newErrors: { [index: number]: { quantity?: string } } = {};
    let isValid = true;

    entries.forEach((entry, index) => {
      const quantityNum = Number(entry.quantity);
      if (entry.quantity === "" || isNaN(quantityNum) || quantityNum <= 0) {
        newErrors[index] = { quantity: "Ilość musi być > 0" };
        isValid = false;
      }
    });

    setErrors(newErrors);
    return isValid;
  };

  const handleSave = async () => {
    if (!validate() || !internalGroupId) return;

    setLoading(true);
    await handleApiResponse(
      () => FallenStockService.updateFallenStocks(internalGroupId, entries),
      () => {
        toast.success("Pomyślnie zaktualizowano zgłoszenie.");
        onSave();
        handleClose();
      },
      undefined,
      "Nie udało się zaktualizować zgłoszenia"
    );
    setLoading(false);
  };

  const handleClose = () => {
    setFormData(null);
    setEntries([]);
    setErrors({});
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="md">
      <DialogTitle>Edytuj zgłoszenie sztuk padłych</DialogTitle>
      <DialogContent>
        {loading && !formData ? (
          <Loading />
        ) : (
          formData && (
            <Box display="flex" flexDirection="column" gap={2} mt={1}>
              {/* Sekcja z danymi tylko do odczytu */}
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Ferma"
                    value={formData.farmName}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Cykl"
                    value={formData.cycleDisplay}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Typ zgłoszenia"
                    value={formData.typeDesc}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Zakład utylizacyjny"
                    value={formData.utilizationPlantName}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6 }}>
                  <TextField
                    label="Data zgłoszenia"
                    value={formData.date}
                    slotProps={{ input: { readOnly: true } }}
                    fullWidth
                  />
                </Grid>
              </Grid>

              <Table size="small" sx={{ mt: 2 }}>
                <TableHead>
                  <TableRow>
                    <TableCell>Kurnik</TableCell>
                    <TableCell align="right">Sztuki</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {entries.map((entry, index) => (
                    <TableRow key={entry.henhouseId}>
                      <TableCell component="th" scope="row">
                        {entry.henhouseName}
                      </TableCell>
                      <TableCell align="right">
                        <TextField
                          type="number"
                          value={entry.quantity}
                          onChange={(e) =>
                            handleQuantityChange(index, e.target.value)
                          }
                          variant="outlined"
                          size="small"
                          error={!!errors[index]?.quantity}
                          helperText={errors[index]?.quantity}
                          sx={{ width: "120px" }}
                        />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Box>
          )
        )}
      </DialogContent>
      <DialogActions>
        <Button disabled={loading} onClick={handleClose}>
          Anuluj
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          startIcon={<MdSave />}
          disabled={!formData}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditFallenStocksModal;
