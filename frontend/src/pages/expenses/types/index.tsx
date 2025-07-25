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
import LoadingButton from "../../../components/common/loading-button";
import Loading from "../../../components/loading/loading";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { ExpensesService } from "../../../services/expenses-service";
import { useExpensesTypes } from "../../../hooks/expenses/useExpensesTypes";

const ExpensesTypesPage: React.FC = () => {
  const { expensesTypes, loadingExpensesTypes, fetchExpensesTypes } =
    useExpensesTypes();
  const [newTypes, setNewTypes] = useState<string[]>([]);

  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchExpensesTypes();
  }, []);

  const handleAdd = () => {
    if (saving) return;
    setNewTypes((prev) => [...prev, ""]);
  };

  const handleNewTypeChange = (index: number, value: string) => {
    setNewTypes((prev) => {
      const copy = [...prev];
      copy[index] = value;
      return copy;
    });
  };

  const handleRemove = async (id: string) => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => ExpensesService.deleteExpensesType(id),
      async () => {
        await fetchExpensesTypes();
      },
      undefined,
      "Wystąpił błąd podczas usuwania pola"
    );
    setSaving(false);
  };

  const handleRemoveNewType = (index: number) => {
    if (saving) return;
    setNewTypes((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSave = async () => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => ExpensesService.addExpensesType(newTypes),
      async () => {
        setNewTypes([]);
        await fetchExpensesTypes();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania pól"
    );
    setSaving(false);
  };

  if (loadingExpensesTypes) return <Loading size={30} />;

  return (
    <Box p={4} maxWidth={700} display="flex" flexDirection="column">
      <Typography variant="h5" mb={2}>
        Typy wydatków
      </Typography>

      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Nazwa typu</TableCell>
              <TableCell align="center">Akcje</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {expensesTypes.map((f) => (
              <TableRow key={f.id}>
                <TableCell>
                  <Typography>{f.name}</Typography>
                </TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemove(f.id)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}

            {newTypes.map((name, index) => (
              <TableRow key={"new-" + index}>
                <TableCell>
                  <TextField
                    value={name}
                    onChange={(e) => handleNewTypeChange(index, e.target.value)}
                    fullWidth
                    autoFocus
                    placeholder="Wprowadź nazwę typu"
                    disabled={saving}
                  />
                </TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemoveNewType(index)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}

            {expensesTypes.length === 0 && newTypes.length === 0 && (
              <TableRow>
                <TableCell colSpan={2} align="center">
                  Brak typów – dodaj nowe.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>

      <Box display="flex" justifyContent="flex-start" gap={2} mt={2}>
        <Button variant="outlined" onClick={handleAdd} disabled={saving}>
          Dodaj typ
        </Button>
        <LoadingButton
          height="40px"
          loadingSize={10}
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          onClick={handleSave}
          loading={saving}
          disabled={newTypes.length === 0}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default ExpensesTypesPage;
