import { Button, Typography, IconButton, Box, Tooltip } from "@mui/material";
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
      field: "usedQuantity",
      headerName: "Ilość zużyta [l]",
      flex: 1,
      sortable: false,
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
      getActions: (params) => {
        const isUsed = params.row.usedQuantity > 0;

        const editButton = (
          <Button
            key="edit"
            variant="outlined"
            size="small"
            disabled={isUsed}
            onClick={() => {
              setSelectedGasDelivery(params.row);
              setIsEditModalOpen(true);
            }}
          >
            Edytuj
          </Button>
        );

        const deleteButton = (
          <Button
            key="delete"
            variant="outlined"
            size="small"
            color="error"
            disabled={isUsed}
            onClick={() => {
              deleteGasDelivery(params.row.id);
            }}
          >
            Usuń
          </Button>
        );

        if (isUsed) {
          return [
            <Tooltip
              key="edit-tooltip"
              title="Nie można edytować zużytej dostawy"
            >
              <span>{editButton}</span>
            </Tooltip>,
            <Tooltip
              key="delete-tooltip"
              title="Nie można usunąć zużytej dostawy"
            >
              <span>{deleteButton}</span>
            </Tooltip>,
          ];
        }

        return [editButton, deleteButton];
      },
    },
  ];
};
