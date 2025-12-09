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
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  IconButton,
} from "@mui/material";
import { toast } from "react-toastify";
import { MdFileUpload, MdDelete } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import AppDialog from "../../common/app-dialog";
import { AccountingService } from "../../../services/accounting-service";
import {
  KSeFInvoiceType,
  KSeFInvoiceTypeLabels,
} from "../../../models/accounting/ksef-invoice";
import dayjs from "dayjs";

interface UploadInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const UploadInvoiceModal: React.FC<UploadInvoiceModalProps> = ({
  open,
  onClose,
  onSuccess,
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);
  const [invoiceType, setInvoiceType] = useState<KSeFInvoiceType>(
    KSeFInvoiceType.Purchase
  );
  const [invoiceNumber, setInvoiceNumber] = useState("");
  const [invoiceDate, setInvoiceDate] = useState(dayjs().format("YYYY-MM-DD"));

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const newFiles = Array.from(e.target.files);
      setSelectedFiles((prev) => [...prev, ...newFiles]);
    }
    // Reset input to allow selecting the same file again
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleRemoveFile = (index: number) => {
    setSelectedFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const handleUpload = async () => {
    if (selectedFiles.length === 0) {
      toast.warning("Wybierz przynajmniej jeden plik");
      return;
    }

    setLoading(true);
    try {
      await handleApiResponse(
        () =>
          AccountingService.uploadManualInvoice(selectedFiles, {
            invoiceType,
            invoiceNumber: invoiceNumber || undefined,
            invoiceDate: invoiceDate || undefined,
          }),
        () => {
          toast.success("Faktura została wgrana pomyślnie");
          onSuccess();
          handleClose();
        },
        undefined,
        "Błąd podczas wgrywania faktury"
      );
    } catch {
      toast.error("Błąd podczas wgrywania faktury");
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    setSelectedFiles([]);
    setInvoiceNumber("");
    setInvoiceDate(dayjs().format("YYYY-MM-DD"));
    setInvoiceType(KSeFInvoiceType.Purchase);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    onClose();
  };

  return (
    <AppDialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>Dodaj fakturę manualnie</DialogTitle>
      <DialogContent>
        <Box sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 2 }}>
          <FormControl fullWidth>
            <InputLabel>Typ faktury</InputLabel>
            <Select
              value={invoiceType}
              label="Typ faktury"
              onChange={(e) =>
                setInvoiceType(e.target.value as KSeFInvoiceType)
              }
            >
              {Object.entries(KSeFInvoiceTypeLabels).map(([key, label]) => (
                <MenuItem key={key} value={key}>
                  {label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <TextField
            label="Numer faktury"
            value={invoiceNumber}
            onChange={(e) => setInvoiceNumber(e.target.value)}
            placeholder="np. FV/2024/001"
            fullWidth
          />

          <TextField
            label="Data wystawienia"
            type="date"
            value={invoiceDate}
            onChange={(e) => setInvoiceDate(e.target.value)}
            fullWidth
            slotProps={{
              inputLabel: { shrink: true },
            }}
          />

          <Box>
            <input
              ref={fileInputRef}
              type="file"
              multiple
              accept=".pdf,.xml,.jpg,.jpeg,.png"
              onChange={handleFileChange}
              style={{ display: "none" }}
              id="upload-invoice-input"
            />
            <label htmlFor="upload-invoice-input">
              <Button
                variant="outlined"
                color="primary"
                component="span"
                fullWidth
                startIcon={<MdFileUpload />}
              >
                Wybierz pliki (PDF, XML, obrazy)
              </Button>
            </label>
          </Box>

          {selectedFiles.length > 0 && (
            <List dense sx={{ bgcolor: "background.paper", borderRadius: 1 }}>
              {selectedFiles.map((file, idx) => (
                <ListItem
                  key={idx}
                  secondaryAction={
                    <IconButton
                      edge="end"
                      aria-label="delete"
                      onClick={() => handleRemoveFile(idx)}
                      size="small"
                    >
                      <MdDelete />
                    </IconButton>
                  }
                >
                  <ListItemText
                    primary={file.name}
                    secondary={`${(file.size / 1024).toFixed(1)} KB`}
                  />
                </ListItem>
              ))}
            </List>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={loading}>
          Anuluj
        </Button>
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

export default UploadInvoiceModal;
