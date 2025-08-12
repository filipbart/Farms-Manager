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
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", width: 180 },
    { field: "henhouseName", headerName: "Kurnik", width: 150 },
    { field: "slaughterhouseName", headerName: "Ubojnia", width: 180 },
    {
      field: "saleDate",
      headerName: "Data sprzedaży",
      width: 150,
      type: "string",
      valueGetter: (date: any) => {
        return date ? dayjs(date).format("YYYY-MM-DD") : "";
      },
    },
    { field: "typeDesc", headerName: "Typ sprzedaży", width: 150 },
    {
      field: "weight",
      headerName: "Waga ubojni [kg]",
      width: 160,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "quantity",
      headerName: "Ilość sztuk ubojnia [szt]",
      width: 190,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "confiscatedWeight",
      headerName: "Konfiskaty [kg]",
      width: 150,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "confiscatedCount",
      headerName: "Konfiskaty [szt]",
      width: 150,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "deadWeight",
      headerName: "Kurczęta martwe [kg]",
      width: 180,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "deadCount",
      headerName: "Kurczęta martwe [szt]",
      width: 180,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "farmerWeight",
      headerName: "Waga producenta [kg]",
      width: 180,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "basePrice",
      headerName: "Cena bazowa [zł]",
      width: 150,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "priceWithExtras",
      headerName: "Cena z dodatkami [zł]",
      width: 130,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
    {
      field: "otherExtras",
      headerName: "Inne dodatki",
      width: 150,
      renderCell: (params) => <OtherExtrasCell value={params.value} />,
    },
    {
      field: "comment",
      headerName: "Komentarz",
      width: 250,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
    {
      field: "sendToIrz",
      headerName: "Wyślij do IRZplus",
      width: 180,
      sortable: false,
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
      sortable: false,
      width: 180,
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
      width: 180,
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
      renderCell: (params) => (params.value ? params.value : "Brak numeru"),
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
    },
  ];
};
