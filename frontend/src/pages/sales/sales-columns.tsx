import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { OtherExtrasCell } from "../../models/sales/sale-other-extras-cell";
import { CommentCell } from "../../components/datagrid/comment-cell";
import SaleSendToIrzCell from "../../components/datagrid/sale-send-to-irz-cell";
import ActionsCell from "../../components/datagrid/actions-cell";
import FileDownloadCell from "../../components/datagrid/file-download-cell";

export const getSalesColumns = ({
  setSelectedSale,
  deleteSale,
  setIsEditModalOpen,
  downloadSaleDirectory,
  downloadDirectoryPath,
  dispatch,
  filters,
}: {
  setSelectedSale: (s: any) => void;
  deleteSale: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  downloadSaleDirectory: (path: string) => void;
  downloadDirectoryPath: string | null;
  dispatch: any;
  filters: any;
}): GridColDef[] => {
  return [
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    { field: "slaughterhouseName", headerName: "Ubojnia", flex: 1 },
    {
      field: "saleDate",
      headerName: "Data sprzedaży",
      flex: 1,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    { field: "typeDesc", headerName: "Typ sprzedaży", flex: 1 },
    {
      field: "weight",
      headerName: "Waga ubojni [kg]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "quantity",
      headerName: "Ilość sztuk ubojnia [szt]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "confiscatedWeight",
      headerName: "Konfiskaty [kg]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "confiscatedCount",
      headerName: "Konfiskaty [szt]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "deadWeight",
      headerName: "Kurczęta martwe [kg]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "deadCount",
      headerName: "Kurczęta martwe [szt]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "farmerWeight",
      headerName: "Waga producenta [kg]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "basePrice",
      headerName: "Cena bazowa [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "priceWithExtras",
      headerName: "Cena z dodatkami [zł]",
      flex: 1,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "otherExtras",
      headerName: "Inne dodatki",
      flex: 1,
      renderCell: (params) => <OtherExtrasCell value={params.value} />,
    },
    {
      field: "comment",
      headerName: "Komentarz",
      flex: 1,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
    {
      field: "sendToIrz",
      headerName: "Wyślij do IRZplus",
      flex: 1,
      minWidth: 200,
      type: "actions",
      renderCell: (params) => (
        <SaleSendToIrzCell
          dispatch={dispatch}
          row={params.row}
          filters={filters}
        />
      ),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onEdit={(row) => {
            setSelectedSale(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteSale}
        />,
      ],
    },
    {
      field: "fileDownload",
      headerName: "Zawarte dokumenty",
      align: "center",
      headerAlign: "center",
      sortable: false,
      width: 100,
      renderCell: (params) => {
        const directoryPath = params.row.directoryPath;

        return (
          <FileDownloadCell
            filePath={directoryPath}
            downloadingFilePath={downloadDirectoryPath}
            onDownload={downloadSaleDirectory}
          />
        );
      },
    },

    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      flex: 1,
      renderCell: (params) => (params.value ? params.value : "Brak numeru"),
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
    },
  ];
};
