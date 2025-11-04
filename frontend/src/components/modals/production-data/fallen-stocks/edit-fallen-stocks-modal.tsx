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
  MenuItem,
  TableRow,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs, { type Dayjs } from "dayjs";
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
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";
import LoadingTextField from "../../../common/loading-textfield";

interface EditFallenStocksModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  internalGroupId: string | null;
}

type GetFallenStockEditDataExtended = GetFallenStockEditData & {
  farmId: string;
  cycleId: string;
};

const EditFallenStocksModal: React.FC<EditFallenStocksModalProps> = ({
  open,
  onClose,
  onSave,
  internalGroupId,
}) => {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<Omit<
    GetFallenStockEditDataExtended,
    "entries"
  > | null>(null);
  const [entries, setEntries] = useState<FallenStockEditableEntry[]>([]);
  const [errors, setErrors] = useState<{
    [index: number]: { quantity?: string };
    cycleId?: string;
    date?: string;
  }>({});
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);
  const [selectedCycleId, setSelectedCycleId] = useState("");
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(null);

  useEffect(() => {
    const fetchFallenStockData = async () => {
      if (!internalGroupId) return;

      setLoading(true);
      await handleApiResponse(
        () => FallenStockService.getFallenStocksDataForEdit(internalGroupId),
        (data) => {
          const responseData = data.responseData as
            | GetFallenStockEditDataExtended
            | undefined;
          if (responseData) {
            setFormData({
              farmId: responseData.farmId,
              farmName: responseData.farmName,
              cycleId: responseData.cycleId,
              cycleDisplay: responseData.cycleDisplay,
              typeDesc: responseData.typeDesc,
              utilizationPlantName: responseData.utilizationPlantName,
              date: new Date(responseData.date).toLocaleDateString("pl-PL"),
            });
            setSelectedDate(dayjs(responseData.date));
            setEntries(
              responseData.entries.map((e) => ({
                ...e,
                quantity: String(e.quantity),
              }))
            );
            setSelectedCycleId(responseData.cycleId);

            const fetchCycles = async (farmId: string) => {
              setLoadingCycles(true);
              await handleApiResponse(
                () => FarmsService.getFarmCycles(farmId),
                (cycleData) => setCycles(cycleData.responseData ?? []),
                () => setCycles([]),
                "Nie udało się pobrać listy cykli."
              );
              setLoadingCycles(false);
            };

            if (responseData.farmId) fetchCycles(responseData.farmId);
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
    const newErrors: { [index: number]: { quantity?: string }; date?: string; cycleId?: string } = {};
    let isValid = true;

    if (!selectedCycleId) {
      newErrors.cycleId = "Cykl jest wymagany";
      isValid = false;
    }
    
    if (!selectedDate || !selectedDate.isValid()) {
      newErrors.date = "Data jest wymagana";
      isValid = false;
    }
    entries.forEach((entry, index) => {
      const quantityNum = Number(entry.quantity);
      if (entry.quantity === "" || isNaN(quantityNum) || quantityNum <= 0) {
        newErrors[index] = { quantity: "Ilość musi być > 0" };
        isValid = false;
      }
    });

    setErrors(newErrors as any);
    return isValid;
  };

  const handleSave = async () => {
    if (!validate() || !internalGroupId) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        FallenStockService.updateFallenStocks(internalGroupId, {
          cycleId: selectedCycleId,
          date: selectedDate!.format("YYYY-MM-DD"),
          entries,
        }),
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
    setCycles([]);
    setErrors({});
    setSelectedDate(null);
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
                  <LoadingTextField
                    loading={loadingCycles}
                    label="Cykl"
                    select
                    fullWidth
                    disabled={loadingCycles || cycles.length === 0}
                    value={selectedCycleId}
                    onChange={(e) => {
                      setSelectedCycleId(e.target.value);
                      setErrors((prev) => ({ ...prev, cycleId: undefined }));
                    }}
                    error={!!errors.cycleId}
                    helperText={errors.cycleId}
                  >
                    {cycles.map((cycle) => (
                      <MenuItem key={cycle.id} value={cycle.id}>
                        {`${cycle.identifier}/${cycle.year}`}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
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
                  <DatePicker
                    label="Data zgłoszenia"
                    value={selectedDate}
                    onChange={(newValue) => {
                      setSelectedDate(newValue);
                      setErrors((prev) => ({ ...prev, date: undefined }));
                    }}
                    format="DD.MM.YYYY"
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.date,
                        helperText: errors.date,
                      },
                    }}
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
