import { useState, useEffect } from "react";
import { DialogTitle, DialogContent, Button, Typography } from "@mui/material";
import AppDialog from "./app-dialog";

interface FilePreviewProps {
  file: File | string | undefined;
  showPreviewButton?: boolean;
  maxHeight?: number;
}

const FilePreview: React.FC<FilePreviewProps> = ({
  file,
  showPreviewButton = true,
  maxHeight = 300,
}) => {
  const [previewOpen, setPreviewOpen] = useState(false);
  const [objectUrl, setObjectUrl] = useState<string>("");

  const isFileObject = typeof file !== "string" && file instanceof File;
  const fileUrl = isFileObject ? objectUrl : (file as string);

  const fileType: "pdf" | "image" | "unknown" = (() => {
    if (!file) return "unknown";

    if (isFileObject) {
      const mime = file.type;
      if (mime === "application/pdf") return "pdf";
      if (mime.startsWith("image/")) return "image";
      return "unknown";
    }

    const lowerUrl = file.toLowerCase();
    if (lowerUrl.endsWith(".pdf")) return "pdf";
    if (lowerUrl.match(/\.(jpeg|jpg|png|webp)$/)) return "image";
    return "unknown";
  })();

  useEffect(() => {
    if (isFileObject && file) {
      const url = URL.createObjectURL(file);
      setObjectUrl(url);
      return () => URL.revokeObjectURL(url);
    }
  }, [file, isFileObject]);

  if (!file) return null;
  if (isFileObject && !objectUrl)
    return <Typography variant="body2">Ładowanie podglądu...</Typography>;

  const previewContent = () => {
    if (fileType === "pdf") {
      return (
        <iframe
          src={fileUrl + "#toolbar=0"}
          style={{
            width: "100%",
            border: "1px solid #ccc",
            maxHeight,
            height: "100vh",
          }}
        />
      );
    }

    if (fileType === "image") {
      return (
        <img
          src={fileUrl}
          alt="Preview"
          style={{ maxWidth: "100%", maxHeight, border: "1px solid #ccc" }}
        />
      );
    }

    return <Typography>Nieobsługiwany typ pliku</Typography>;
  };

  return (
    <>
      {previewContent()}
      {showPreviewButton && (fileType === "pdf" || fileType === "image") && (
        <Button
          size="small"
          onClick={() => setPreviewOpen(true)}
          sx={{ mt: 1 }}
        >
          Powiększ
        </Button>
      )}

      <AppDialog
        open={previewOpen}
        onClose={() => setPreviewOpen(false)}
        maxWidth="lg"
        fullWidth
      >
        <DialogTitle>Podgląd pliku</DialogTitle>
        <DialogContent>
          {fileType === "pdf" && (
            <iframe
              src={fileUrl + "#toolbar=0"}
              style={{ width: "100%", height: "80vh", border: "none" }}
            />
          )}
          {fileType === "image" && (
            <img
              src={fileUrl}
              alt="Preview"
              style={{ width: "100%", maxHeight: "80vh", objectFit: "contain" }}
            />
          )}
          {fileType === "unknown" && (
            <Typography>Nieobsługiwany typ pliku</Typography>
          )}
        </DialogContent>
      </AppDialog>
    </>
  );
};

export default FilePreview;
