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
  FormHelperText,
} from "@mui/material";
import { FaFloppyDisk, FaPenToSquare, FaXmark } from "react-icons/fa6";
import { FaTrash } from "react-icons/fa";
import type { SaleEntry } from "../../../../../models/sales/sales-entry";
import type { SaleEntriesTableProps } from "./table-types";
import { validateEntry } from "../validation/validate-entry";

const inputWidth = 150;
const numberInputWidth = 120;

const SaleEntriesTable: React.FC<SaleEntriesTableProps> = ({
  entries,
  henhouses,
  errors,
  dispatch,
  setEntryErrors,
  farmId,
}) => {
  const handleFieldChange = (
    index: number,
    name: keyof SaleEntry,
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
          <TableCell>Ubojnia</TableCell>
          <TableCell>Sztuki</TableCell>
          <TableCell>Waga (kg)</TableCell>
          <TableCell>Konfiskata – Sztuki</TableCell>
          <TableCell>Konfiskata – Waga (kg)</TableCell>
          <TableCell>Martwe – Sztuki</TableCell>
          <TableCell>Martwe – Waga (kg)</TableCell>
          <TableCell>Waga hodowcy (kg)</TableCell>
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
                      disabled={!farmId}
                      error={!!errors?.[index]?.henhouseId}
                      sx={{ minWidth: inputWidth }}
                    >
                      {availableHenhouses.map((house) => (
                        <MenuItem key={house.id} value={house.id}>
                          {house.name}
                        </MenuItem>
                      ))}
                    </Select>
                    <FormHelperText
                      error={!!errors?.[index]?.henhouseId}
                      sx={{ minHeight: 24 }}
                    >
                      {errors?.[index]?.henhouseId || "\u00A0"}
                    </FormHelperText>
                  </>
                ) : (
                  henhouses.find((h) => h.id === entry.henhouseId)?.name || "—"
                )}
              </TableCell>

              <TableCell>
                {entry.isEditing ? (
                  <>
                    <Select
                      value={entry.slaughterhouseId}
                      onChange={({ target }) =>
                        handleFieldChange(
                          index,
                          "slaughterhouseId",
                          target.value
                        )
                      }
                      fullWidth
                      disabled={loadingSlaughterhouses}
                      error={!!errors?.[index]?.slaughterhouseId}
                      sx={{ minWidth: inputWidth }}
                    >
                      {slaughterhouses.map((slaughterhouse) => (
                        <MenuItem
                          key={slaughterhouse.id}
                          value={slaughterhouse.id}
                        >
                          {slaughterhouse.name}
                        </MenuItem>
                      ))}
                    </Select>
                    <FormHelperText
                      error={!!errors?.[index]?.slaughterhouseId}
                      sx={{ minHeight: 24 }}
                    >
                      {errors?.[index]?.slaughterhouseId || "\u00A0"}
                    </FormHelperText>
                  </>
                ) : (
                  slaughterhouses.find((s) => s.id === entry.slaughterhouseId)
                    ?.name || "—"
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
                    inputProps={{ min: 1, step: 1 }}
                    sx={{ minWidth: numberInputWidth, width: numberInputWidth }}
                    error={!!errors?.[index]?.quantity}
                    helperText={errors?.[index]?.quantity || "\u00A0"}
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
                    value={entry.weight}
                    onChange={(e) =>
                      handleFieldChange(index, "weight", e.target.value)
                    }
                    inputProps={{ min: 0, step: 0.01 }}
                    sx={{ minWidth: numberInputWidth, width: numberInputWidth }}
                    error={!!errors?.[index]?.weight}
                    helperText={errors?.[index]?.weight || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.weight || "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.confiscatedCount}
                    onChange={(e) =>
                      handleFieldChange(
                        index,
                        "confiscatedCount",
                        e.target.value
                      )
                    }
                    inputProps={{ min: 0, step: 1 }}
                    sx={{ minWidth: numberInputWidth }}
                    error={!!errors?.[index]?.confiscatedCount}
                    helperText={errors?.[index]?.confiscatedCount || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.confiscatedCount ?? "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.confiscatedWeight}
                    onChange={(e) =>
                      handleFieldChange(
                        index,
                        "confiscatedWeight",
                        e.target.value
                      )
                    }
                    inputProps={{ min: 0, step: 0.01 }}
                    sx={{ minWidth: numberInputWidth }}
                    error={!!errors?.[index]?.confiscatedWeight}
                    helperText={errors?.[index]?.confiscatedWeight || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.confiscatedWeight ?? "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.deadCount}
                    onChange={(e) =>
                      handleFieldChange(index, "deadCount", e.target.value)
                    }
                    inputProps={{ min: 0, step: 1 }}
                    sx={{ minWidth: numberInputWidth }}
                    error={!!errors?.[index]?.deadCount}
                    helperText={errors?.[index]?.deadCount || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.deadCount ?? "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.deadWeight}
                    onChange={(e) =>
                      handleFieldChange(index, "deadWeight", e.target.value)
                    }
                    inputProps={{ min: 0, step: 0.01 }}
                    sx={{ minWidth: numberInputWidth }}
                    error={!!errors?.[index]?.deadWeight}
                    helperText={errors?.[index]?.deadWeight || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.deadWeight ?? "—"
                )}
              </TableCell>

              <TableCell align="right">
                {entry.isEditing ? (
                  <TextField
                    type="number"
                    value={entry.farmerWeight}
                    onChange={(e) =>
                      handleFieldChange(index, "farmerWeight", e.target.value)
                    }
                    inputProps={{ min: 0, step: 0.01 }}
                    sx={{ minWidth: numberInputWidth }}
                    error={!!errors?.[index]?.farmerWeight}
                    helperText={errors?.[index]?.farmerWeight || "\u00A0"}
                    fullWidth
                  />
                ) : (
                  entry.farmerWeight ?? "—"
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
                          !entry.slaughterhouseId &&
                          !entry.quantity &&
                          !entry.weight
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

export default SaleEntriesTable;
