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
import { FaTrash } from "react-icons/fa";
import type { HouseRowModel } from "../../../../models/farms/house-row-model";
import type { FallenStockEntry } from "../../../../models/fallen-stocks/fallen-stocks";

interface FallenStockEntriesTableProps {
  entries: (FallenStockEntry & { isEditing: boolean })[];
  henhouses: HouseRowModel[];
  errors:
    | { [index: number]: { henhouseId?: string; quantity?: string } }
    | undefined;
  dispatch: React.Dispatch<any>;
  farmId?: string;
}

const inputWidth = 150;
const numberInputWidth = 120;

const FallenStockEntriesTable: React.FC<FallenStockEntriesTableProps> = ({
  entries,
  henhouses,
  errors,
  dispatch,
  farmId,
}) => {
  const handleFieldChange = (
    index: number,
    name: keyof FallenStockEntry,
    value: string
  ) => {
    dispatch({
      type: "UPDATE_ENTRY",
      index,
      name,
      value,
    });
  };

  return (
    <Table
      size="small"
      sx={{ mt: 2, "& td, & th": { verticalAlign: "top" }, "& td": { pt: 2 } }}
    >
      <TableHead>
        <TableRow>
          <TableCell>Kurnik</TableCell>
          <TableCell align="right">Sztuki</TableCell>
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
                  {errors?.[index]?.henhouseId && (
                    <Typography variant="caption" color="error">
                      {errors[index].henhouseId}
                    </Typography>
                  )}
                </>
              </TableCell>

              <TableCell align="right">
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
              </TableCell>

              <TableCell align="center">
                <IconButton
                  onClick={() => {
                    dispatch({ type: "REMOVE_ENTRY", index });
                  }}
                  color="error"
                  size="small"
                  title="Usuń"
                >
                  <FaTrash />
                </IconButton>
              </TableCell>
            </TableRow>
          );
        })}
      </TableBody>
    </Table>
  );
};

export default FallenStockEntriesTable;
