import { Box, Button, IconButton } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload } from "react-icons/md";
import Loading from "../../../components/loading/loading";

export const getFeedsPaymentsColumns = ({
  deleteFeedPayment,
  downloadPaymentFile,
  downloadFileName,
}: {
  deleteFeedPayment: (id: string) => void;
  downloadPaymentFile: (path: string) => void;
  downloadFileName: string | null;
}): GridColDef[] => {
  return [
    { field: "id", headerName: "Id", width: 70 },
    { field: "farmName", headerName: "Nazwa farmy", flex: 1 },
    { field: "fileName", headerName: "Nazwa pliku", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD, HH:mm:ss") : "";
      },
    },
    {
      field: "fileDownload",
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
            onClick={() => downloadPaymentFile(params.row.fileName)}
            color="primary"
            disabled={downloadFileName === params.row.fileName}
          >
            {downloadFileName === params.row.fileName ? (
              <Loading size={10} />
            ) : (
              <MdFileDownload />
            )}
          </IconButton>
        </Box>
      ),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Box key="delete" display="flex" justifyContent="center" width="100%">
          <Button
            variant="outlined"
            size="small"
            color="error"
            onClick={() => deleteFeedPayment(params.row.id)}
          >
            Usuń
          </Button>
        </Box>,
      ],
    },
  ];
};
