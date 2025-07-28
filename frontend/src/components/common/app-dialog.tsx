import { Dialog, type DialogProps } from "@mui/material";
import React from "react";

interface AppDialogProps extends Omit<DialogProps, "onClose"> {
  onClose: () => void;
}

const AppDialog: React.FC<AppDialogProps> = ({
  children,
  onClose,
  ...props
}) => {
  const handleClose = (
    _event: object,
    reason: "backdropClick" | "escapeKeyDown"
  ) => {
    if (reason !== "backdropClick") {
      onClose();
    }
  };

  return (
    <Dialog onClose={handleClose} {...props}>
      {children}
    </Dialog>
  );
};

export default AppDialog;
