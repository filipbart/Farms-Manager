import { Box, IconButton } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import { MdFileDownload } from "react-icons/md";
import Loading from "../../../components/loading/loading";

export const getFeedsCorrectionsColumns = ({
  downloadCorrectionFile,
  downloadFilePath,
}: {
  downloadCorrectionFile: (filePath: string) => void;
  downloadFilePath: string | null;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "invoiceNumber", headerName: "Numer faktury", flex: 1 },
    {
      field: "invoiceFile",
      headerName: "Plik faktury",
      flex: 1,
      renderCell: (params) => (
        <Box
          display="flex"
          justifyContent="center"
          alignItems="center"
          width="100%"
          height="100%"
        >
          <IconButton
            onClick={() => downloadCorrectionFile(params.row.filePath)}
            color="primary"
            disabled={downloadFilePath === params.row.filePath}
          >
            {downloadFilePath === params.row.filePath ? (
              <Loading size={10} />
            ) : (
              <MdFileDownload />
            )}
          </IconButton>
        </Box>
      ),
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
