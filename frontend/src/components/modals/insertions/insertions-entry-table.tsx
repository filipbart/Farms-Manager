import React from "react";
import {
  Table,
  TableHead,
  TableBody,
  TableCell,
  TableRow,
  Select,
  MenuItem,
  TextField,
  IconButton,
  Typography,
} from "@mui/material";
import { FaFloppyDisk, FaPenToSquare, FaXmark } from "react-icons/fa6";
import { FaTrash } from "react-icons/fa";
import type { HouseRowModel } from "../../../models/farms/house-row-model";
import type {
  InsertionEntry,
  InsertionEntryErrors,
} from "../../../models/insertions/insertion-entry";

interface InsertionEntriesTableProps {
  entries: InsertionEntry[];
  henhouses: HouseRowModel[];
  hatcheries: { id: string; name: string }[];
  errors: { [index: number]: InsertionEntryErrors } | undefined;
  dispatch: React.Dispatch<any>;
  setEntryErrors: (index: number, errors: InsertionEntryErrors) => void;
  validateEntry: (entry: InsertionEntry) => InsertionEntryErrors;
  loadingHenhouses: boolean;
  loadingHatcheries: boolean;
  farmId?: string;
}

const inputWidth = 150;
const numberInputWidth = 120;

const InsertionEntriesTable: React.FC<InsertionEntriesTableProps> = ({
  entries,
  henhouses,
  hatcheries,
  errors,
  dispatch,
  setEntryErrors,
  validateEntry,
  loadingHenhouses,
  loadingHatcheries,
  farmId,
}) => {
  const handleFieldChange = (
    index: number,
    name: keyof InsertionEntry,
    value: string
  ) => {
    dispatch({
      type: "UPDATE_ENTRY",
      index,
      name,
      value,
    });
    setEntryErrors(index, { ...(errors?.[index] || {}), [name]: undefined });
  };

  return (
    <Table
      size="small"
      sx={{ mt: 2, "& td, & th": { verticalAlign: "top" }, "& td": { pt: 2 } }}
    >
      <TableHead>
        <TableRow>
          <TableCell>Kurnik</TableCell>
          <TableCell>Wylęgarnia</TableCell>
          <TableCell align="right">Sztuki</TableCell>
          <TableCell align="right">Śr. masa ciała</TableCell>
          <TableCell align="center">Akcje</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {entries.map((entry, index) => {
          const availableHenhouses = henhouses.filter(
            (h) =>
              !entries.some(
                (en, idx) => idx !== index && en.henhouseId === h.id
              )
          );

          return (
            <TableRow key={index}>
              <TableCell>
                {entry.isEditing ? (
                  <>
                    <Select
                      value={entry.henhouseId}
                      onChange={({ target }) =>
                        handleFieldChange(index, "henhouseId", target.value)
                      }
                      fullWidth
                      disabled={loadingHenhouses || !farmId}
                      error={!!errors?.[index]?.henhouseId}
                      sx={{ minWidth: inputWidth }}
                    >
                      {availableHenhouses.map((house) => (
                        <MenuItem key={house.id} value={house.id}>
                          {house.name}
                        </MenuItem>
                      ))}
                    </Select>
                    {errors?.[index]?.henhouseId && (
                      <Typography variant="caption" color="error">
                        {errors[index].henhouseId}
                      </Typography>
                    )}
                  </>
                ) : (
                  henhouses.find((h) => h.id === entry.henhouseId)?.name || "—"
                )}
              </TableCell>

              <TableCell>
                {entry.isEditing ? (
                  <>
                    <Select
                      value={entry.hatcheryId}
                      onChange={({ target }) =>
                        handleFieldChange(index, "hatcheryId", target.value)
                      }
                      fullWidth
                      disabled={loadingHatcheries}
                      error={!!errors?.[index]?.hatcheryId}
                      sx={{ minWidth: inputWidth }}
                    >
                      {hatcheries.map((hatchery) => (
                        <MenuItem key={hatchery.id} value={hatchery.id}>
                          {hatchery.name}
                        </MenuItem>
                      ))}
                    </Select>
                    {errors?.[index]?.hatcheryId && (
                      <Typography variant="caption" color="error">
                        {errors[index].hatcheryId}
                      </Typography>
                    )}
                  </>
                ) : (
                  hatcheries.find((h) => h.id === entry.hatcheryId)?.name || "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.quantity}
                    onChange={(e) =>
                      handleFieldChange(index, "quantity", e.target.value)
                    }
                    slotProps={{ htmlInput: { min: 1, step: 1 } }}
                    sx={{ minWidth: numberInputWidth, width: numberInputWidth }}
                    error={!!errors?.[index]?.quantity}
                    helperText={errors?.[index]?.quantity}
                    fullWidth
                  />
                ) : (
                  entry.quantity || "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.bodyWeight}
                    onChange={(e) =>
                      handleFieldChange(index, "bodyWeight", e.target.value)
                    }
                    slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                    sx={{ minWidth: numberInputWidth, width: numberInputWidth }}
                    error={!!errors?.[index]?.bodyWeight}
                    helperText={errors?.[index]?.bodyWeight}
                    fullWidth
                  />
                ) : (
                  entry.bodyWeight || "—"
                )}
              </TableCell>

              <TableCell align="center">
                {entry.isEditing ? (
                  <>
                    <IconButton
                      onClick={() => {
                        const entryErrors = validateEntry(entry);
                        if (Object.keys(entryErrors).length === 0) {
                          dispatch({
                            type: "UPDATE_ENTRY",
                            index,
                            name: "isEditing",
                            value: false,
                          });
                          setEntryErrors(index, {});
                        } else {
                          setEntryErrors(index, entryErrors);
                        }
                      }}
                      color="primary"
                      size="small"
                      title="Zapisz"
                    >
                      <FaFloppyDisk />
                    </IconButton>
                    <IconButton
                      onClick={() => {
                        if (
                          !entry.henhouseId &&
                          !entry.hatcheryId &&
                          !entry.quantity &&
                          !entry.bodyWeight
                        ) {
                          dispatch({ type: "REMOVE_ENTRY", index });
                          setEntryErrors(index, {});
                        } else {
                          dispatch({
                            type: "UPDATE_ENTRY",
                            index,
                            name: "isEditing",
                            value: false,
                          });
                          setEntryErrors(index, {});
                        }
                      }}
                      color="inherit"
                      size="small"
                      title="Anuluj"
                    >
                      <FaXmark />
                    </IconButton>
                  </>
                ) : (
                  <>
                    <IconButton
                      onClick={() =>
                        dispatch({
                          type: "UPDATE_ENTRY",
                          index,
                          name: "isEditing",
                          value: true,
                        })
                      }
                      size="small"
                      title="Edytuj"
                    >
                      <FaPenToSquare />
                    </IconButton>
                    <IconButton
                      onClick={() => {
                        dispatch({ type: "REMOVE_ENTRY", index });
                        setEntryErrors(index, {});
                      }}
                      color="error"
                      size="small"
                      title="Usuń"
                    >
                      <FaTrash />
                    </IconButton>
                  </>
                )}
              </TableCell>
            </TableRow>
          );
        })}
      </TableBody>
    </Table>
  );
};

export default InsertionEntriesTable;
