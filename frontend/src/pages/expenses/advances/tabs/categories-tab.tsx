import React, { useEffect, useState } from "react";
import {
  Box,
  Table,
  TableHead,
  TableBody,
  TableCell,
  TableRow,
  TextField,
  IconButton,
  Button,
  Paper,
  MenuItem,
} from "@mui/material";
import { MdDelete, MdSave } from "react-icons/md";
import LoadingButton from "../../../../components/common/loading-button";
import Loading from "../../../../components/loading/loading";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import {
  AdvanceType,
  type AddAdvanceCategory,
} from "../../../../models/expenses/advances/categories";
import { useAdvanceCategories } from "../../../../hooks/expenses/advances/useAdvanceCategories";
import { ExpensesAdvancesService } from "../../../../services/expenses-advances-service";

const AdvanceCategoriesTab: React.FC = () => {
  const { categories, loadingCategories, fetchCategories } =
    useAdvanceCategories();
  const [newCategories, setNewCategories] = useState<AddAdvanceCategory[]>([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  const handleAdd = () => {
    if (saving) return;
    setNewCategories((prev) => [
      ...prev,
      { name: "", type: AdvanceType.Expense },
    ]);
  };

  const handleNewFieldChange = (
    index: number,
    field: "name" | "type",
    value: string
  ) => {
    setNewCategories((prev) => {
      const copy = [...prev];
      copy[index][field] = value as AdvanceType;
      return copy;
    });
  };

  const handleRemove = async (id: string) => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => ExpensesAdvancesService.deleteAdvanceCategory(id),
      () => {
        fetchCategories();
      },
      () => {
        fetchCategories();
      },
      "Wystąpił błąd podczas usuwania kategorii"
    );
    setSaving(false);
  };

  const handleRemoveNewField = (index: number) => {
    if (saving) return;
    setNewCategories((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSave = async () => {
    if (saving) return;

    const categoriesToSave = newCategories.filter((c) => c.name.trim() !== "");
    if (categoriesToSave.length === 0) {
      alert("Nazwa kategorii nie może być pusta.");
      return;
    }

    setSaving(true);
    await handleApiResponse(
      () => ExpensesAdvancesService.addAdvanceCategories(categoriesToSave),
      async () => {
        fetchCategories();
        setNewCategories([]);
      },
      () => {
        fetchCategories();
      },
      "Wystąpił błąd podczas zapisywania kategorii"
    );
    setSaving(false);
  };

  if (loadingCategories) return <Loading size={30} />;

  return (
    <Box mt={3} maxWidth={700} display="flex" flexDirection="column">
      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Typ</TableCell>
              <TableCell>Nazwa kategorii</TableCell>
              <TableCell align="center" sx={{ width: "100px" }}>
                Akcje
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {categories.map((category) => (
              <TableRow key={category.id}>
                <TableCell>
                  {category.type === AdvanceType.Income
                    ? "Przychód"
                    : "Wydatek"}
                </TableCell>
                <TableCell>{category.name}</TableCell>
                <TableCell align="center">
                  <IconButton
                    color="error"
                    onClick={() => handleRemove(category.id)}
                    disabled={saving}
                  >
                    <MdDelete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}

            {newCategories.map((category, index) => (
              <TableRow key={`new-${index}`}>
                <TableCell>
                  <TextField
                    select
                    value={category.type}
                    onChange={(e) =>
                      handleNewFieldChange(index, "type", e.target.value)
                    }
                    fullWidth
                    size="small"
                    disabled={saving}
                  >
                    <MenuItem value={AdvanceType.Income}>Przychód</MenuItem>
                    <MenuItem value={AdvanceType.Expense}>Wydatek</MenuItem>
                  </TextField>
                </TableCell>
                <TableCell>
                  <TextField
                    value={category.name}
                    onChange={(e) =>
                      handleNewFieldChange(index, "name", e.target.value)
                    }
                    fullWidth
                    autoFocus={index === newCategories.length - 1}
                    placeholder="Wprowadź nazwę kategorii"
                    disabled={saving}
                    size="small"
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

      <Box
        display="flex"
        justifyContent="flex-start"
        alignItems="center"
        gap={2}
        mt={2}
      >
        <Button variant="outlined" onClick={handleAdd} disabled={saving}>
          Dodaj kategorię
        </Button>
        <LoadingButton
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          onClick={handleSave}
          loading={saving}
          disabled={newCategories.length === 0}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default AdvanceCategoriesTab;
