import { useState, useEffect, useRef } from "react";
import {
  DialogTitle,
  DialogContent,
  Button,
  Typography,
  IconButton,
  Box,
} from "@mui/material";
import AppDialog from "./app-dialog";
import ZoomInIcon from "@mui/icons-material/ZoomIn";
import ZoomOutIcon from "@mui/icons-material/ZoomOut";
import RotateLeftIcon from "@mui/icons-material/RotateLeft";
import RotateRightIcon from "@mui/icons-material/RotateRight";
import RestartAltIcon from "@mui/icons-material/RestartAlt";

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
  const [scale, setScale] = useState<number>(1);
  const [rotation, setRotation] = useState<number>(0);
  const [position, setPosition] = useState({ x: 0, y: 0 });
  const [isDragging, setIsDragging] = useState(false);
  const [dragStart, setDragStart] = useState({ x: 0, y: 0 });
  const imgRef = useRef<HTMLImageElement>(null);

  const isFileObject = typeof file !== "string" && file instanceof File;
  const fileUrl = isFileObject ? objectUrl : (file as string);

  const fileType: "pdf" | "image" | "unknown" = (() => {
    if (!file) return "unknown";

    if (isFileObject) {
      const mime = file.type.toLowerCase();
      if (mime === "application/pdf") return "pdf";
      if (mime.startsWith("image/")) return "image";
      return "unknown";
    }

    try {
      const url = new URL(file);
      const pathname = url.pathname.toLowerCase();

      if (pathname.endsWith(".pdf")) return "pdf";
      if (pathname.match(/\.(jpe?g|png|webp|gif|bmp)$/)) return "image";

      if (url.pathname) {
        if (
          url.pathname.includes("/pdf/") ||
          url.searchParams.get("type") === "pdf"
        )
          return "pdf";
        if (
          url.pathname.includes("/image/") ||
          url.searchParams.get("type") === "image"
        )
          return "image";
      }
    } catch {
      const lowerPath = file.toLowerCase();
      if (lowerPath.endsWith(".pdf")) return "pdf";
      if (lowerPath.match(/\.(jpe?g|png|webp|gif|bmp)$/)) return "image";
    }

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

  const handleWheel = (e: React.WheelEvent) => {
    e.preventDefault();
    const delta = e.deltaY > 0 ? 0.9 : 1.1;
    setScale((prev) => Math.min(Math.max(0.5, prev * delta), 3));
  };

  const handleMouseDown = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
    setDragStart({
      x: e.clientX - position.x,
      y: e.clientY - position.y,
    });
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (!isDragging) return;
    e.preventDefault();
    e.stopPropagation();
    const newX = e.clientX - dragStart.x;
    const newY = e.clientY - dragStart.y;

    // Ograniczenie przesuwania, aby nie wyjść poza widoczne granice
    const img = imgRef.current;
    if (img) {
      const container = img.parentElement?.parentElement;
      if (container) {
        const containerRect = container.getBoundingClientRect();
        const imgRect = img.getBoundingClientRect();

        const maxX = Math.max(
          0,
          (imgRect.width * scale - containerRect.width) / 2
        );
        const maxY = Math.max(
          0,
          (imgRect.height * scale - containerRect.height) / 2
        );

        setPosition({
          x: Math.max(-maxX, Math.min(maxX, newX)),
          y: Math.max(-maxY, Math.min(maxY, newY)),
        });
        return;
      }
    }

    setPosition({
      x: newX,
      y: newY,
    });
  };

  const handleMouseUp = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleMouseLeave = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

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
          title="Podgląd PDF"
        />
      );
    }

    if (fileType === "image") {
      return (
        <Box
          sx={{
            position: "relative",
            overflow: "hidden",
            maxHeight,
            border: "1px solid #ccc",
          }}
        >
          <Box
            sx={{
              transform: `scale(${scale}) rotate(${rotation}deg)`,
              transformOrigin: "center",
              transition: isDragging ? "none" : "transform 0.1s",
              width: "100%",
              height: "100%",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
              cursor: isDragging ? "grabbing" : scale > 1 ? "move" : "default",
            }}
            onWheel={handleWheel}
            onMouseDown={handleMouseDown}
            onMouseMove={handleMouseMove}
            onMouseUp={handleMouseUp}
            onMouseLeave={handleMouseUp}
          >
            <img
              ref={imgRef}
              src={fileUrl}
              alt="Preview"
              style={{
                maxWidth: "100%",
                maxHeight: "100%",
                objectFit: "contain",
                transform: `translate(${position.x}px, ${position.y}px)`,
              }}
            />
          </Box>
          <Box
            sx={{
              position: "absolute",
              bottom: 8,
              right: 8,
              display: "flex",
              gap: 1,
              bgcolor: "rgba(255, 255, 255, 0.8)",
              p: 0.5,
              borderRadius: 1,
            }}
          >
            <IconButton
              size="small"
              onClick={() => setScale((prev) => Math.min(prev + 0.1, 3))}
              title="Powiększ"
            >
              <ZoomInIcon fontSize="small" />
            </IconButton>
            <IconButton
              size="small"
              onClick={() => setScale((prev) => Math.max(prev - 0.1, 0.5))}
              title="Pomniejsz"
            >
              <ZoomOutIcon fontSize="small" />
            </IconButton>
            <IconButton
              size="small"
              onClick={() => setRotation((prev) => (prev - 90) % 360)}
              title="Obróć w lewo"
            >
              <RotateLeftIcon fontSize="small" />
            </IconButton>
            <IconButton
              size="small"
              onClick={() => setRotation((prev) => (prev + 90) % 360)}
              title="Obróć w prawo"
            >
              <RotateRightIcon fontSize="small" />
            </IconButton>
            <IconButton
              size="small"
              onClick={() => {
                setScale(1);
                setRotation(0);
                setPosition({ x: 0, y: 0 });
              }}
              title="Resetuj widok"
            >
              <RestartAltIcon fontSize="small" />
            </IconButton>
          </Box>
        </Box>
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
        <DialogContent
          sx={{
            display: "flex",
            flexDirection: "column",
            height: "80vh",
            p: 0,
            overflow: "hidden",
          }}
        >
          {fileType === "pdf" && (
            <iframe
              src={fileUrl + "#toolbar=0"}
              style={{ width: "100%", height: "100%", border: "none" }}
              title="Pełny podgląd PDF"
            />
          )}
          {fileType === "image" && (
            <Box
              sx={{
                position: "relative",
                width: "100%",
                height: "100%",
                overflow: "hidden",
              }}
            >
              <Box
                sx={{
                  transform: `scale(${scale}) rotate(${rotation}deg)`,
                  transformOrigin: "center",
                  transition: isDragging ? "none" : "transform 0.1s",
                  width: "100%",
                  height: "100%",
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  cursor: isDragging
                    ? "grabbing"
                    : scale > 1
                    ? "move"
                    : "default",
                }}
                onWheel={handleWheel}
                onMouseDown={handleMouseDown}
                onMouseMove={handleMouseMove}
                onMouseUp={handleMouseUp}
                onMouseLeave={handleMouseLeave}
              >
                <img
                  src={fileUrl}
                  alt="Preview"
                  style={{
                    maxWidth: "100%",
                    maxHeight: "100%",
                    objectFit: "contain",
                    transform: `translate(${position.x}px, ${position.y}px)`,
                  }}
                />
              </Box>
              <Box
                sx={{
                  position: "absolute",
                  bottom: 16,
                  right: 16,
                  display: "flex",
                  gap: 1,
                  bgcolor: "rgba(255, 255, 255, 0.8)",
                  p: 1,
                  borderRadius: 1,
                }}
              >
                <IconButton
                  size="small"
                  onClick={() => setScale((prev) => Math.min(prev + 0.1, 3))}
                  title="Powiększ"
                >
                  <ZoomInIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => setScale((prev) => Math.max(prev - 0.1, 0.5))}
                  title="Pomniejsz"
                >
                  <ZoomOutIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => setRotation((prev) => (prev - 90) % 360)}
                  title="Obróć w lewo"
                >
                  <RotateLeftIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => setRotation((prev) => (prev + 90) % 360)}
                  title="Obróć w prawo"
                >
                  <RotateRightIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => {
                    setScale(1);
                    setRotation(0);
                    setPosition({ x: 0, y: 0 });
                  }}
                  title="Resetuj widok"
                >
                  <RestartAltIcon fontSize="small" />
                </IconButton>
              </Box>
            </Box>
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
