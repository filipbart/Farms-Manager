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
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { SalesSettingsService } from "../../services/sales-settings-service";
import { useSaleFieldsExtra } from "../../hooks/sales/useSaleFieldsExtra";

const SaleFieldsSettingsPage: React.FC = () => {
  const { saleFieldsExtra, loadingSaleFieldsExtra, fetchSaleFieldsExtra } =
    useSaleFieldsExtra();
  const [newFields, setNewFields] = useState<string[]>([]);

  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchSaleFieldsExtra();
  }, []);

  const handleAdd = () => {
    if (saving) return;
    setNewFields((prev) => [...prev, ""]);
  };

  const handleNewFieldChange = (index: number, value: string) => {
    setNewFields((prev) => {
      const copy = [...prev];
      copy[index] = value;
      return copy;
    });
  };

  const handleRemove = async (id: string) => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => SalesSettingsService.deleteSaleFieldExtra(id),
      async () => {
        await fetchSaleFieldsExtra();
      },
      undefined,
      "Wystąpił błąd podczas usuwania pola"
    );
    setSaving(false);
  };

  const handleRemoveNewField = (index: number) => {
    if (saving) return;
    setNewFields((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSave = async () => {
    if (saving) return;
    setSaving(true);
    await handleApiResponse(
      () => SalesSettingsService.addSaleFieldExtra(newFields),
      async () => {
        setNewFields([]);
        await fetchSaleFieldsExtra();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania pól"
    );
    setSaving(false);
  };

  if (loadingSaleFieldsExtra) return <Loading size={30} />;

  return (
    <Box p={4} maxWidth={700} display="flex" flexDirection="column">
      <Typography variant="h5" mb={2}>
        Ustawienia pól dodatkowych sprzedaży
      </Typography>

      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Nazwa pola dodatkowego</TableCell>
              <TableCell align="center">Akcje</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {saleFieldsExtra.map((f) => (
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

            {newFields.map((name, index) => (
              <TableRow key={"new-" + index}>
                <TableCell>
                  <TextField
                    value={name}
                    onChange={(e) =>
                      handleNewFieldChange(index, e.target.value)
                    }
                    fullWidth
                    autoFocus
                    placeholder="Wprowadź nazwę pola"
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

            {saleFieldsExtra.length === 0 && newFields.length === 0 && (
              <TableRow>
                <TableCell colSpan={2} align="center">
                  Brak pól – dodaj nowe.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>

      <Box display="flex" justifyContent="flex-start" gap={2} mt={2}>
        <Button variant="outlined" onClick={handleAdd} disabled={saving}>
          Dodaj pole
        </Button>
        <LoadingButton
          height="40px"
          loadingSize={10}
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          onClick={handleSave}
          loading={saving}
          disabled={newFields.length === 0}
        >
          Zapisz
        </LoadingButton>
      </Box>
    </Box>
  );
};

export default SaleFieldsSettingsPage;
