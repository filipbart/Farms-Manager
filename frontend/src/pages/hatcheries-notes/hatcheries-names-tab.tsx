import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Table,
  TableHead,
  TableBody,
  TableCell,
  TableRow,
  TextField,
  IconButton,
  Button,
  Paper,
} from "@mui/material";
import { MdDelete, MdSave } from "react-icons/md";
import LoadingButton from "../../components/common/loading-button";
import Loading from "../../components/loading/loading";
import { HatcheriesService } from "../../services/hatcheries-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { HatcheryName } from "../../models/hatcheries/hatcheries-prices";

const HatcheriesNamesTab: React.FC = () => {
  const [hatcheries, setHatcheries] = useState<HatcheryName[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [newNames, setNewNames] = useState<string[]>([]);

  const fetchHatcheries = async () => {
    setLoading(true);
    await handleApiResponse(
      () => HatcheriesService.getPricesNames(),
      (data) => {
        setHatcheries(data.responseData?.hatcheries ?? []);
      },
      undefined,
      "Błąd podczas pobierania wylęgarni"
    );
    setLoading(false);
  };

  useEffect(() => {
    fetchHatcheries();
  }, []);

  const handleAdd = () => {
    if (saving) return;
    setNewNames((prev) => [...prev, ""]);
  };

  const handleNewFieldChange = (index: number, value: string) => {
    setNewNames((prev) => {
      const copy = [...prev];
      copy[index] = value;
      return copy;
    });
  };

  const handleRemove = async (id: string) => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => HatcheriesService.deleteHatcheryName(id),
      async () => {
        await fetchHatcheries();
      },
      undefined,
      "Wystąpił błąd podczas usuwania nazwy wylęgarni"
    );
    setSaving(false);
  };

  const handleRemoveNewField = (index: number) => {
    if (saving) return;
    setNewNames((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSave = async () => {
    if (saving) return;
    setSaving(true);
    for (const name of newNames) {
      if (name.trim()) {
        await handleApiResponse(
          () => HatcheriesService.addHatcheryName(name.trim()),
          undefined,
          undefined,
          `Wystąpił błąd podczas zapisywania nazwy: ${name}`
        );
      }
    }
    setNewNames([]);
    await fetchHatcheries();
    setSaving(false);
  };

  if (loading) return <Loading size={30} />;

  return (
    <Box p={4} maxWidth={700} display="flex" flexDirection="column">
      <Typography variant="h5" mb={2}>
        Nazwy wylęgarni
      </Typography>

      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Nazwa wylęgarni</TableCell>
              <TableCell align="center">Akcje</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {hatcheries.map((h) => (
              <TableRow key={h.id}>
                <TableCell>
                  <Typography>{h.name}</Typography>
                </TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemove(h.id)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
            {newNames.map((name, index) => (
              <TableRow key={`new-${index}`}>
                <TableCell>
                  <TextField
                    value={name}
                    onChange={(e) =>
                      handleNewFieldChange(index, e.target.value)
                    }
                    fullWidth
                    autoFocus
                    placeholder="Wprowadź nazwę"
                    disabled={saving}
                  />
                </TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemoveNewField(index)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>

      <Box display="flex" justifyContent="flex-start" gap={2} mt={2}>
        <Button variant="outlined" onClick={handleAdd} disabled={saving}>
          Dodaj nazwę
        </Button>
        <LoadingButton
          height="40px"
          loadingSize={10}
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          onClick={handleSave}
          loading={saving}
          disabled={newNames.length === 0}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default HatcheriesNamesTab;
