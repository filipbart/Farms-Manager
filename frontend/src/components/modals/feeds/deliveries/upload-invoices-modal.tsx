import React, { useRef, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  List,
  ListItem,
  ListItemText,
  Box,
} from "@mui/material";
import LoadingButton from "../../../common/loading-button";
import { toast } from "react-toastify";
import { FeedsService } from "../../../../services/feeds-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { MdFileUpload } from "react-icons/md";
import type { DraftFeedInvoice } from "../../../../models/feeds/deliveries/draft-feed-invoice";

interface UploadInvoicesModalProps {
  open: boolean;
  onClose: () => void;
  onUpload: (files: DraftFeedInvoice[]) => void;
}

const UploadInvoicesModal: React.FC<UploadInvoicesModalProps> = ({
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
      try {
        setLoading(true);
        await handleApiResponse(
          () => FeedsService.uploadInvoices(selectedFiles),
          (data) => {
            if (data && data.responseData) {
              onUpload(data.responseData.files);
            }
            toast.success("Faktury zostały wgrane pomyślnie");
          },
          undefined,
          "Błąd podczas wgrywania faktur"
        );
      } catch {
        toast.error("Błąd podczas wgrywania faktur");
      } finally {
        setLoading(false);
      }

      setSelectedFiles([]);
      onClose();
    }
  };

  const handleClose = () => {
    setSelectedFiles([]);
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>Wgraj faktury</DialogTitle>
      <DialogContent>
        <Box my={2}>
          <input
            ref={fileInputRef}
            type="file"
            multiple
            onChange={handleFileChange}
            style={{ display: "none" }}
            id="upload-invoices-input"
          />
          <label htmlFor="upload-invoices-input">
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
    </Dialog>
  );
};

export default UploadInvoicesModal;
