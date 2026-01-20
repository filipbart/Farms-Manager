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
  IconButton,
  TextField,
} from "@mui/material";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { MdFileUpload, MdDelete } from "react-icons/md";
import LoadingButton from "../../common/loading-button";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import AppDialog from "../../common/app-dialog";
import {
  AccountingService,
  type DraftAccountingInvoice,
} from "../../../services/accounting-service";
import {
  KSeFInvoiceType,
  KSeFInvoiceTypeLabels,
  KSeFPaymentStatus,
  KSeFPaymentStatusLabels,
  ModuleType,
  ModuleTypeLabels,
} from "../../../models/accounting/ksef-invoice";

interface UploadInvoiceModalProps {
  open: boolean;
  onClose: () => void;
  onUpload: (files: DraftAccountingInvoice[]) => void;
}

const UploadInvoiceModal: React.FC<UploadInvoiceModalProps> = ({
  open,
  onClose,
  onUpload,
}) => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const abortControllerRef = useRef<AbortController | null>(null);
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);
  const [invoiceType, setInvoiceType] = useState<KSeFInvoiceType>(
    KSeFInvoiceType.Purchase,
  );
  const [paymentStatus, setPaymentStatus] = useState<KSeFPaymentStatus>(
    KSeFPaymentStatus.Unpaid,
  );
  const [paymentDate, setPaymentDate] = useState<string>("");
  const [moduleType, setModuleType] = useState<ModuleType>(ModuleType.None);

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

  const handlePaymentStatusChange = (value: KSeFPaymentStatus) => {
    setPaymentStatus(value);

    // Auto-set payment date when status changes to paid
    const isPaidStatus =
      value === KSeFPaymentStatus.PaidCash ||
      value === KSeFPaymentStatus.PaidTransfer;
    if (isPaidStatus && !paymentDate) {
      setPaymentDate(dayjs().format("YYYY-MM-DD"));
    } else if (!isPaidStatus) {
      setPaymentDate("");
    }
  };

  const handleUpload = async () => {
    if (selectedFiles.length === 0) {
      toast.warning("Wybierz przynajmniej jeden plik");
      return;
    }

    setLoading(true);
    const controller = new AbortController();
    abortControllerRef.current = controller;

    // Rozdziel pliki na XML i pozostałe
    const xmlFiles = selectedFiles.filter((f) =>
      f.name.toLowerCase().endsWith(".xml"),
    );
    const otherFiles = selectedFiles.filter(
      (f) => !f.name.toLowerCase().endsWith(".xml"),
    );

    try {
      // Przetwórz pliki XML jako faktury KSeF (bezpośredni import)
      if (xmlFiles.length > 0) {
        await handleApiResponse(
          () =>
            AccountingService.uploadXmlInvoices(
              xmlFiles,
              invoiceType,
              paymentStatus,
              paymentDate || null,
              controller.signal,
            ),
          (data) => {
            if (data && data.responseData) {
              const { importedCount, skippedCount, errors } = data.responseData;
              if (importedCount > 0) {
                toast.success(
                  `Zaimportowano ${importedCount} faktur KSeF z XML`,
                );
              }
              if (skippedCount > 0) {
                toast.warning(`Pominięto ${skippedCount} faktur (duplikaty)`);
              }
              errors.forEach((err) => toast.error(err));
            }
          },
          undefined,
          "Błąd podczas importowania faktur XML",
        );
      }

      // Przetwórz pozostałe pliki przez AI
      if (otherFiles.length > 0) {
        await handleApiResponse(
          () =>
            AccountingService.uploadInvoices(
              otherFiles,
              invoiceType,
              paymentStatus,
              moduleType,
              paymentDate || null,
              controller.signal,
            ),
          (data) => {
            if (data && data.responseData) {
              onUpload(data.responseData.files);
            }
            toast.success("Faktury zostały wgrane pomyślnie");
          },
          undefined,
          "Błąd podczas wgrywania faktur",
        );
      } else if (xmlFiles.length > 0) {
        // Jeśli były tylko pliki XML, zamknij modal i odśwież listę
        handleClose();
        // Trigger parent refresh via empty callback
        onUpload([]);
      }
    } catch (error: any) {
      if (error.name !== "CanceledError") {
        toast.error("Błąd podczas wgrywania faktur");
      }
    } finally {
      setLoading(false);
      abortControllerRef.current = null;
      if (otherFiles.length > 0) {
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
    setInvoiceType(KSeFInvoiceType.Purchase);
    setPaymentStatus(KSeFPaymentStatus.Unpaid);
    setPaymentDate("");
    setModuleType(ModuleType.None);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
    setLoading(false);
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

          <FormControl fullWidth>
            <InputLabel>Status płatności</InputLabel>
            <Select
              value={paymentStatus}
              label="Status płatności"
              onChange={(e) =>
                handlePaymentStatusChange(e.target.value as KSeFPaymentStatus)
              }
            >
              {Object.entries(KSeFPaymentStatusLabels).map(([key, label]) => (
                <MenuItem key={key} value={key}>
                  {label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          {/* Payment Date Input - show only when status is paid */}
          {(paymentStatus === KSeFPaymentStatus.PaidCash ||
            paymentStatus === KSeFPaymentStatus.PaidTransfer) && (
            <TextField
              fullWidth
              type="date"
              label="Data płatności"
              value={paymentDate}
              onChange={(e) => setPaymentDate(e.target.value)}
              inputProps={{
                max: dayjs().format("YYYY-MM-DD"),
              }}
              InputLabelProps={{
                shrink: true,
              }}
            />
          )}

          <FormControl fullWidth>
            <InputLabel>Moduł docelowy</InputLabel>
            <Select
              value={moduleType}
              label="Moduł docelowy"
              onChange={(e) => setModuleType(e.target.value as ModuleType)}
            >
              {Object.entries(ModuleTypeLabels).map(([key, label]) => (
                <MenuItem key={key} value={key}>
                  {label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

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
