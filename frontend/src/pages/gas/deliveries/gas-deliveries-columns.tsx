import { Button, Typography, IconButton, Box } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { MdFileDownload } from "react-icons/md";
import Loading from "../../../components/loading/loading";
import type { GasDeliveryListModel } from "../../../models/gas/gas-deliveries";
import { CommentCell } from "../../../components/datagrid/comment-cell";

interface GetGasDeliveriesColumnsProps {
  setSelectedGasDelivery: (row: GasDeliveryListModel) => void;
  deleteGasDelivery: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  downloadGasDeliveryFile: (path: string) => void;
  downloadingFilePath: string | null;
}

export const getGasDeliveriesColumns = ({
  setSelectedGasDelivery,
  deleteGasDelivery,
  setIsEditModalOpen,
  downloadGasDeliveryFile,
  downloadingFilePath,
}: GetGasDeliveriesColumnsProps): GridColDef<GasDeliveryListModel>[] => {
  return [
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "contractorName",
      headerName: "Kontrahent",
      flex: 1,
    },
    {
      field: "invoiceDate",
      headerName: "Data wystawienia",
      flex: 1,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "";
      },
    },
    {
      field: "invoiceNumber",
      headerName: "Numer faktury",
      flex: 1,
    },
    {
      field: "unitPrice",
      headerName: "Cena jednostkowa [zł]",
      flex: 1,
    },
    {
      field: "quantity",
      headerName: "Ilość [l]",
      flex: 1,
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      sortable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
    {
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      flex: 0.5,
      renderCell: (params) => {
        const filePath = params.row.filePath;

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
            onClick={() => downloadGasDeliveryFile(filePath)}
            color="primary"
            disabled={isDownloading}
          >
            {isDownloading ? <Loading size={24} /> : <MdFileDownload />}
          </IconButton>
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedGasDelivery(params.row);
            setIsEditModalOpen(true);
          }}
        >
          Edytuj
        </Button>,
        <Button
          key="delete"
          variant="outlined"
          size="small"
          color="error"
          onClick={() => {
            deleteGasDelivery(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
