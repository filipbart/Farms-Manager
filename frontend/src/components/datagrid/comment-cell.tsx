import {
  Button,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from "@mui/material";
import { useState } from "react";
import AppDialog from "../common/app-dialog";

export const CommentCell: React.FC<{ value: string }> = ({ value }) => {
  const [open, setOpen] = useState(false);

  if (!value) return null;

  const preview = value.length > 20 ? `${value.slice(0, 20)}...` : value;

  return (
    <>
      <Button variant="text" onClick={() => setOpen(true)}>
        {preview}
      </Button>
      <AppDialog open={open} onClose={() => setOpen(false)}>
        <DialogTitle>Zawartość komentarza</DialogTitle>
        <DialogContent>
          <Typography>{value}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Zamknij</Button>
        </DialogActions>
      </AppDialog>
    </>
  );
};
