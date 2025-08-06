import React from "react";
import {
  Table,
  TableHead,
  TableBody,
  TableCell,
  TableRow,
  TextField,
  IconButton,
  Button,
  MenuItem,
  Typography,
} from "@mui/material";
import { MdDelete, MdAttachFile } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import type { AdvanceCategoryModel } from "../../../../models/expenses/advances/categories";
import type { ExpenseAdvanceEntry } from "../../../../models/expenses/advances/expenses-advances";

interface AdvanceEntriesTableProps {
  entries: ExpenseAdvanceEntry[];
  categories: AdvanceCategoryModel[];
  errors: { [index: number]: any } | undefined;
  dispatch: React.Dispatch<any>;
}

const AdvanceEntriesTable: React.FC<AdvanceEntriesTableProps> = ({
  entries,
  categories,
  errors,
  dispatch,
}) => {
  const handleFieldChange = (
    index: number,
    name: keyof ExpenseAdvanceEntry,
    value: any
  ) => {
    dispatch({ type: "UPDATE_ENTRY", index, name, value });
  };

  const handleFileChange = (
    index: number,
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    if (e.target.files?.[0]) {
      dispatch({
        type: "UPDATE_ENTRY",
        index,
        name: "file",
        value: e.target.files[0],
      });
    }
  };

  return (
    <Table size="small" sx={{ mt: 2 }}>
      <TableHead>
        <TableRow>
          <TableCell>Data</TableCell>
          <TableCell>Nazwa</TableCell>
          <TableCell>Kategoria</TableCell>
          <TableCell>Kwota [zł]</TableCell>
          <TableCell>Komentarz</TableCell>
          <TableCell>Plik</TableCell>
          <TableCell align="center" sx={{ width: "60px" }}>
            Akcje
          </TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {entries.map((entry, index) => {
          const entryErrors = errors?.[index];
          return (
            <TableRow key={index}>
              <TableCell sx={{ minWidth: 180 }}>
                <DatePicker
                  value={entry.date ? dayjs(entry.date) : null}
                  onChange={(date) =>
                    handleFieldChange(
                      index,
                      "date",
                      date ? dayjs(date).format("YYYY-MM-DD") : ""
                    )
                  }
                  format="DD.MM.YYYY"
                  slotProps={{
                    textField: {
                      size: "small",
                      fullWidth: true,
                      error: !!entryErrors?.date,
                      helperText: entryErrors?.date,
                    },
                  }}
                />
              </TableCell>
              <TableCell sx={{ minWidth: 200 }}>
                <TextField
                  value={entry.name || ""}
                  onChange={(e) =>
                    handleFieldChange(index, "name", e.target.value)
                  }
                  error={!!entryErrors?.name}
                  helperText={entryErrors?.name}
                  fullWidth
                  size="small"
                />
              </TableCell>
              <TableCell sx={{ minWidth: 200 }}>
                <TextField
                  select
                  value={entry.categoryName}
                  onChange={(e) =>
                    handleFieldChange(index, "categoryName", e.target.value)
                  }
                  error={!!entryErrors?.categoryName}
                  helperText={entryErrors?.categoryName}
                  fullWidth
                  size="small"
                >
                  {categories.map((cat) => (
                    <MenuItem key={cat.id} value={cat.name}>
                      {cat.name}
                    </MenuItem>
                  ))}
                </TextField>
              </TableCell>
              <TableCell align="right" sx={{ minWidth: 150 }}>
                <TextField
                  type="number"
                  value={entry.amount}
                  onChange={(e) =>
                    handleFieldChange(index, "amount", e.target.value)
                  }
                  error={!!entryErrors?.amount}
                  helperText={entryErrors?.amount}
                  size="small"
                  slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                />
              </TableCell>
              <TableCell sx={{ minWidth: 200 }}>
                <TextField
                  value={entry.comment || ""}
                  onChange={(e) =>
                    handleFieldChange(index, "comment", e.target.value)
                  }
                  fullWidth
                  size="small"
                />
              </TableCell>
              <TableCell>
                <Button
                  component="label"
                  size="small"
                  variant="outlined"
                  startIcon={<MdAttachFile />}
                >
                  {entry.file ? "Zmień" : "Dodaj"}
                  <input
                    type="file"
                    hidden
                    onChange={(e) => handleFileChange(index, e)}
                  />
                </Button>
                {entry.file && (
                  <Typography
                    variant="body2"
                    noWrap
                    sx={{ maxWidth: 120, mt: 1 }}
                  >
                    {entry.file.name}
                  </Typography>
                )}
              </TableCell>
              <TableCell align="center">
                <IconButton
                  color="error"
                  onClick={() => dispatch({ type: "REMOVE_ENTRY", index })}
                >
                  <MdDelete />
                </IconButton>
              </TableCell>
            </TableRow>
          );
        })}
      </TableBody>
    </Table>
  );
};

export default AdvanceEntriesTable;
