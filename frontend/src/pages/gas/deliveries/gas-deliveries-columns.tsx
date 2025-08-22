import { Tooltip } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { GasDeliveryListModel } from "../../../models/gas/gas-deliveries";
import { CommentCell } from "../../../components/datagrid/comment-cell";
import FileDownloadCell from "../../../components/datagrid/file-download-cell";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";

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
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "quantity",
      headerName: "Ilość [l]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "usedQuantity",
      headerName: "Ilość zużyta [l]",
      flex: 1,
      sortable: false,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      sortable: false,
      aggregable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
    {
      field: "filePath",
      headerName: "Faktura",
      align: "center",
      headerAlign: "center",
      sortable: false,
      renderCell: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return (
          <FileDownloadCell
            filePath={params.row.filePath}
            downloadingFilePath={downloadingFilePath}
            onDownload={downloadGasDeliveryFile}
          />
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }

        const isUsed = params.row.usedQuantity > 0;

        // Callbacki przekazywane do ActionsCell:
        const handleEdit = (row: GasDeliveryListModel) => {
          setSelectedGasDelivery(row);
          setIsEditModalOpen(true);
        };
        const handleDelete = (id: string) => {
          deleteGasDelivery(id);
        };

        if (isUsed) {
          // Przyciski disabled z tooltipami
          return [
            <Tooltip
              key="edit-tooltip"
              title="Nie można edytować lub usunąć zużytej dostawy"
            >
              <span>
                <ActionsCell
                  params={params}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                  disabledEdit
                  disabledDelete
                />
              </span>
            </Tooltip>,
          ];
        }

        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />,
        ];
      },
    },
  ];
};
