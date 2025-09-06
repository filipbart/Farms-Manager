import React, { useState } from "react";
import {
  Button,
  Stack,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";

interface ActionsCellProps {
  params: any;
  onDelete?: (id: string) => void;
  onEdit?: (row: any) => void;
  disabledEdit?: boolean;
  disabledDelete?: boolean;
}

const ActionsCell: React.FC<ActionsCellProps> = ({
  params,
  onEdit,
  onDelete,
  disabledEdit = false,
  disabledDelete = false,
}) => {
  const [open, setOpen] = useState(false);

  const handleEdit = () => {
    if (onEdit) {
      onEdit(params.row);
    }
  };

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const handleConfirmDelete = () => {
    if (onDelete) {
      onDelete(params.row.id);
    }
    handleClose();
  };

  return (
    <>
      <Stack direction="row" spacing={1}>
        {onEdit && (
          <Button
            variant="outlined"
            size="small"
            onClick={handleEdit}
            disabled={disabledEdit}
          >
            Edytuj
          </Button>
        )}
        {onDelete && (
          <Button
            variant="outlined"
            size="small"
            color="error"
            onClick={handleClickOpen}
            disabled={disabledDelete}
          >
            Usuń
          </Button>
        )}
      </Stack>

      <Dialog
        open={open}
        onClose={handleClose}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">
          {"Potwierdzenie usunięcia"}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            Czy na pewno chcesz usunąć ten element?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary" variant="outlined">
            Anuluj
          </Button>
          <Button
            onClick={handleConfirmDelete}
            color="error"
            variant="contained"
            autoFocus
          >
            Usuń
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default ActionsCell;
