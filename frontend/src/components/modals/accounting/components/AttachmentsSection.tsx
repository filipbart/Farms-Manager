import {
  Typography,
  Button,
  Box,
  Divider,
  CircularProgress,
} from "@mui/material";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import DownloadIcon from "@mui/icons-material/Download";
import DeleteIcon from "@mui/icons-material/Delete";
import dayjs from "dayjs";
import type { InvoiceAttachment } from "../../../../services/accounting-service";

interface AttachmentsSectionProps {
  attachments: InvoiceAttachment[];
  attachmentsLoading: boolean;
  uploadingAttachment: boolean;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  onUpload: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onDownload: (attachment: InvoiceAttachment) => void;
  onDelete: (attachmentId: string) => void;
}

const AttachmentsSection: React.FC<AttachmentsSectionProps> = ({
  attachments,
  attachmentsLoading,
  uploadingAttachment,
  fileInputRef,
  onUpload,
  onDownload,
  onDelete,
}) => {
  return (
    <Box sx={{ mt: 3 }}>
      <Divider sx={{ mb: 2 }}>
        <Typography variant="caption">
          Załączniki ({attachments.length})
        </Typography>
      </Divider>

      <Box sx={{ mb: 2 }}>
        <input
          type="file"
          ref={fileInputRef}
          style={{ display: "none" }}
          onChange={onUpload}
          accept=".pdf,.jpg,.jpeg,.png,.doc,.docx,.xls,.xlsx"
        />
        <Button
          variant="outlined"
          size="small"
          startIcon={<UploadFileIcon />}
          onClick={() => fileInputRef.current?.click()}
          disabled={uploadingAttachment}
        >
          {uploadingAttachment ? "Dodawanie..." : "Dodaj załącznik"}
        </Button>
      </Box>

      {attachmentsLoading ? (
        <CircularProgress size={24} />
      ) : attachments.length === 0 ? (
        <Typography variant="body2" color="text.secondary">
          Brak załączników
        </Typography>
      ) : (
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 1,
          }}
        >
          {attachments.map((attachment) => (
            <Box
              key={attachment.id}
              sx={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                p: 1,
                border: 1,
                borderColor: "divider",
                borderRadius: 1,
              }}
            >
              <Box>
                <Typography variant="body2" fontWeight={500}>
                  {attachment.fileName}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {(attachment.fileSize / 1024).toFixed(1)} KB •{" "}
                  {dayjs(attachment.uploadedAt).format("YYYY-MM-DD HH:mm")}
                </Typography>
              </Box>
              <Box>
                <Button size="small" onClick={() => onDownload(attachment)}>
                  <DownloadIcon fontSize="small" />
                </Button>
                <Button
                  size="small"
                  color="error"
                  onClick={() => onDelete(attachment.id)}
                >
                  <DeleteIcon fontSize="small" />
                </Button>
              </Box>
            </Box>
          ))}
        </Box>
      )}
    </Box>
  );
};

export default AttachmentsSection;
