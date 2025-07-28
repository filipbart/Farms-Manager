import { useState } from "react";
import {
  Button,
  DialogTitle,
  DialogContent,
  DialogActions,
  Typography,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
} from "@mui/material";
import type { SaleOtherExtras } from "../../models/sales/sales";
import AppDialog from "../../components/common/app-dialog";

export const OtherExtrasCell = ({ value }: { value: SaleOtherExtras }) => {
  const [open, setOpen] = useState(false);

  if (!Array.isArray(value) || value.length === 0) {
    return (
      <Typography variant="body2" color="textSecondary">
        Brak
      </Typography>
    );
  }

  return (
    <>
      <Button variant="text" onClick={() => setOpen(true)}>
        Pokaż dodatki
      </Button>
      <AppDialog
        open={open}
        onClose={() => setOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Inne dodatki</DialogTitle>
        <DialogContent dividers>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>
                  <strong>Nazwa</strong>
                </TableCell>
                <TableCell>
                  <strong>Wartość</strong>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {value.map((item, index) => (
                <TableRow key={index}>
                  <TableCell>{item.name}</TableCell>
                  <TableCell>{item.value.toFixed(2)} zł</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Zamknij</Button>
        </DialogActions>
      </AppDialog>
    </>
  );
};
