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
import AppDialog from "../../../common/app-dialog";
import type { DraftGasInvoice } from "../../../../models/gas/gas-deliveries";
import { GasService } from "../../../../services/gas-service";

interface UploadGasInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  onUpload: (files: DraftGasInvoice[]) => void;
}

const UploadGasInvoicesModal: React.FC<UploadGasInvoicesModalProps> = ({
  open,
  onClose,
  onUpload,
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);
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
      const controller = new AbortController();
      abortControllerRef.current = controller;

      try {
        await handleApiResponse(
          () => GasService.uploadInvoices(selectedFiles, controller.signal),
          (data) => {
            if (data && data.responseData) {
              onUpload(data.responseData.files);
            }
            toast.success("Faktury za gaz zostały wgrane pomyślnie");
          },
          undefined,
          "Błąd podczas wgrywania faktur za gaz"
        );
      } catch (error: any) {
        if (error.name !== "CanceledError") {
          toast.error("Błąd podczas wgrywania faktur za gaz");
        }
      } finally {
        setLoading(false);
        abortControllerRef.current = null;
        handleClose();
      }
    }
  };

  const handleClose = () => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
      abortControllerRef.current = null;
      toast.info("Przesyłanie zostało anulowane.");
    }

    setSelectedFiles([]);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>Wgraj faktury za gaz</DialogTitle>
      <DialogContent>
        <Box my={2}>
          <input
            ref={fileInputRef}
            type="file"
            multiple
            onChange={handleFileChange}
            style={{ display: "none" }}
            id="upload-gas-invoices-input"
          />
          <label htmlFor="upload-gas-invoices-input">
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

export default UploadGasInvoicesModal;
