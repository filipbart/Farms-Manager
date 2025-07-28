import React, { useRef, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  List,
  ListItem,
  ListItemText,
  Box,
} from "@mui/material";
import { toast } from "react-toastify";
import { MdFileUpload } from "react-icons/md";
import LoadingButton from "../../../common/loading-button";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import type { DraftExpenseInvoice } from "../../../../models/expenses/production/expenses-productions";
import { ExpensesService } from "../../../../services/expenses-service";
import AppDialog from "../../../common/app-dialog";
interface UploadExpenseInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  onUpload: (files: DraftExpenseInvoice[]) => void;
}

const UploadExpenseInvoicesModal: React.FC<UploadExpenseInvoicesModalProps> = ({
  open,
  onClose,
  onUpload,
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setSelectedFiles(Array.from(e.target.files));
    }
  };

  const handleUpload = async () => {
    if (selectedFiles.length > 0) {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ExpensesService.uploadInvoices(selectedFiles),
          (data) => {
            if (data && data.responseData) {
              onUpload(data.responseData.files);
            }
            toast.success(
              "Faktury kosztów produkcyjnych zostały wgrane pomyślnie"
            );
          },
          undefined,
          "Błąd podczas wgrywania faktur kosztów produkcyjnych"
        );
      } catch {
        toast.error("Błąd podczas wgrywania faktur kosztów produkcyjnych");
      } finally {
        setLoading(false);
        handleClose();
      }
    }
  };

  const handleClose = () => {
    setSelectedFiles([]);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>Wgraj faktury kosztów produkcyjnych</DialogTitle>
      <DialogContent>
        <Box my={2}>
          <input
            ref={fileInputRef}
            type="file"
            multiple
            onChange={handleFileChange}
            style={{ display: "none" }}
            id="upload-expense-invoices-input"
          />
          <label htmlFor="upload-expense-invoices-input">
            <Button
              variant="outlined"
              color="primary"
              component="span"
              fullWidth
              sx={{ mb: 2 }}
            >
              Wybierz pliki
            </Button>
          </label>
          {selectedFiles.length > 0 && (
            <List dense>
              {selectedFiles.map((file, idx) => (
                <ListItem key={idx}>
                  <ListItemText primary={file.name} />
                </ListItem>
              ))}
            </List>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Zamknij</Button>
        <LoadingButton
          onClick={handleUpload}
          disabled={selectedFiles.length === 0}
          variant="contained"
          color="primary"
          loading={loading}
          startIcon={<MdFileUpload />}
        >
          Prześlij
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default UploadExpenseInvoicesModal;
