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
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import { FeedsService } from "../../../../services/feeds-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";

const FeedsNamesTab: React.FC = () => {
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const [newNames, setNewNames] = useState<string[]>([]);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchFeedsNames();
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
      () => FeedsService.deleteFeedName(id),
      async () => {
        await fetchFeedsNames();
      },
      undefined,
      "Wystąpił błąd podczas usuwania nazwy"
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
    await handleApiResponse(
      () => FeedsService.addFeedName(newNames),
      async () => {
        setNewNames([]);
        await fetchFeedsNames();
      },
      undefined,
      "Wystąpił błąd podczas zapisywania nazw"
    );
    setSaving(false);
  };

  if (loadingFeedsNames) return <Loading size={30} />;

  return (
    <Box p={4} maxWidth={700} display="flex" flexDirection="column">
      <Typography variant="h5" mb={2}>
        Nazwy pasz
      </Typography>

      <Paper variant="outlined" style={{ width: "100%" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Nazwa paszy</TableCell>
              <TableCell align="center">Akcje</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {feedsNames.map((f) => (
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

            {newNames.map((name, index) => (
              <TableRow key={"new-" + index}>
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

            {feedsNames.length === 0 && newNames.length === 0 && (
              <TableRow>
                <TableCell colSpan={2} align="center">
                  Brak nazw – dodaj nowe.
                </TableCell>
              </TableRow>
            )}
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

export default FeedsNamesTab;
