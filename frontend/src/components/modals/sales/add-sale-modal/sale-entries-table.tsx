import React from "react";
import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
} from "@mui/material";
import { MdDelete, MdEdit } from "react-icons/md";
import type { SaleEntry } from "../../../../models/sales/sales-entry";

interface SaleEntriesTableProps {
  entries: SaleEntry[];
  indexes: number[];
  onRemove: (index: number) => void;
  onEdit: (index: number) => void;
}

const SaleEntriesTable: React.FC<SaleEntriesTableProps> = ({
  entries,
  indexes,
  onRemove,
  onEdit,
}) => {
  return (
    <Table size="small">
      <TableHead>
        <TableRow>
          <TableCell>Kurnik</TableCell>
          <TableCell>Ilość</TableCell>
          <TableCell>Waga</TableCell>
          <TableCell>Cena z dodatkami</TableCell>
          <TableCell>Akcje</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {indexes.map((i) => {
          const entry = entries[i];
          if (!entry) return null;
          return (
            <TableRow key={i}>
              <TableCell>{entry.henhouseName}</TableCell>
              <TableCell>{entry.quantity}</TableCell>
              <TableCell>{entry.weight}</TableCell>
              <TableCell>{entry.priceWithExtras}</TableCell>
              <TableCell>
                <IconButton
                  size="small"
                  color="primary"
                  onClick={() => onEdit(i)}
                  aria-label="edytuj"
                >
                  <MdEdit />
                </IconButton>

                <IconButton
                  size="small"
                  color="error"
                  onClick={() => onRemove(i)}
                  aria-label="usuń"
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

export default SaleEntriesTable;
