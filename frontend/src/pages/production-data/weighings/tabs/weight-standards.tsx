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

import LoadingButton from "../../../../components/common/loading-button";
import Loading from "../../../../components/loading/loading";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { useWeightStandards } from "../../../../hooks/useWeightStandards";
import { ProductionDataWeighingsService } from "../../../../services/production-data/production-data-weighings-service";

const WeightStandardsTab: React.FC = () => {
  const { standards, loadingStandards, fetchStandards } = useWeightStandards();
  const [newStandards, setNewStandards] = useState<
    { day: string; weight: string }[]
  >([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchStandards();
  }, [fetchStandards]);

  const handleAdd = () => {
    if (saving) return;
    setNewStandards((prev) => [...prev, { day: "", weight: "" }]);
  };

  const handleNewFieldChange = (
    index: number,
    field: "day" | "weight",
    value: string
  ) => {
    setNewStandards((prev) => {
      const copy = [...prev];
      copy[index][field] = value;
      return copy;
    });
  };

  const handleRemove = async (id: string) => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => ProductionDataWeighingsService.deleteStandard(id),
      async () => {
        await fetchStandards();
      },
      undefined,
      "Wystąpił błąd podczas usuwania normy"
    );
    setSaving(false);
  };

  const handleRemoveNewField = (index: number) => {
    if (saving) return;
    setNewStandards((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSave = async () => {
    if (saving) return;
    const standardsToSave = newStandards
      .map((s) => ({
        day: parseInt(s.day, 10),
        weight: parseInt(s.weight, 10),
      }))
      .filter(
        (s) => !isNaN(s.day) && s.day >= 0 && !isNaN(s.weight) && s.weight >= 0
      );

    if (standardsToSave.length !== newStandards.length) {
      alert("Wszystkie pola muszą być wypełnione poprawnymi liczbami.");
      return;
    }

    setSaving(true);
    await handleApiResponse(
      () => ProductionDataWeighingsService.addStandards(standardsToSave),
      async () => {
        setNewStandards([]);
        await fetchStandards();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania norm"
    );
    setSaving(false);
  };

  if (loadingStandards) return <Loading size={30} />;

  return (
    <Box p={4} maxWidth={700} display="flex" flexDirection="column">
      <Typography variant="h5" mb={2}>
        Normy mas ciała
      </Typography>

      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Doba</TableCell>
              <TableCell>Średnia masa ciała (g)</TableCell>
              <TableCell align="center" sx={{ width: "100px" }}>
                Akcje
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {standards.map((standard) => (
              <TableRow key={standard.id}>
                <TableCell>{standard.day}</TableCell>
                <TableCell>{standard.weight}</TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemove(standard.id)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}

            {newStandards.map((standard, index) => (
              <TableRow key={`new-${index}`}>
                <TableCell>
                  <TextField
                    type="number"
                    value={standard.day}
                    onChange={(e) =>
                      handleNewFieldChange(index, "day", e.target.value)
                    }
                    fullWidth
                    autoFocus={index === newStandards.length - 1}
                    placeholder="Wprowadź dobę"
                    disabled={saving}
                  />
                </TableCell>
                <TableCell>
                  <TextField
                    type="number"
                    value={standard.weight}
                    onChange={(e) =>
                      handleNewFieldChange(index, "weight", e.target.value)
                    }
                    fullWidth
                    placeholder="Wprowadź masę"
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
          Dodaj normę
        </Button>
        <LoadingButton
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          onClick={handleSave}
          loading={saving}
          disabled={newStandards.length === 0}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default WeightStandardsTab;
