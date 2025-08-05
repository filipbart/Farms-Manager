import React from "react";
import { Box, IconButton, Typography } from "@mui/material";
import { MdFileDownload } from "react-icons/md";
import Loading from "../loading/loading";

interface FileDownloadCellProps {
  filePath?: string;
  downloadingFilePath?: string | null;
  onDownload: (filePath: string) => void;
}

const FileDownloadCell: React.FC<FileDownloadCellProps> = ({
  filePath,
  downloadingFilePath,
  onDownload,
}) => {
  if (!filePath) {
    return (
      <Box
        sx={{
          width: "100%",
          height: "100%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Typography variant="body2" color="text.secondary">
          Brak
        </Typography>
      </Box>
    );
  }

  const isDownloading = downloadingFilePath === filePath;

  return (
    <IconButton
      onClick={() => onDownload(filePath)}
      color="primary"
      disabled={isDownloading}
    >
      {isDownloading ? <Loading size={10} /> : <MdFileDownload />}
    </IconButton>
  );
};

export default FileDownloadCell;
