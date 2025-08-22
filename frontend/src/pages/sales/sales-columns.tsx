import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { CommentCell } from "../../components/datagrid/comment-cell";
import SaleSendToIrzCell from "../../components/datagrid/sale-send-to-irz-cell";
import ActionsCell from "../../components/datagrid/actions-cell";
import FileDownloadCell from "../../components/datagrid/file-download-cell";

// Zaktualizowano interfejs propsów, aby przyjmował `uniqueExtraNames`
interface GetSalesColumnsProps {
  setSelectedSale: (s: any) => void;
  deleteSale: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
  downloadSaleDirectory: (path: string) => void;
  downloadDirectoryPath: string | null;
  dispatch: any;
  filters: any;
  uniqueExtraNames: string[]; // Nowa właściwość
}

export const getSalesColumns = ({
  setSelectedSale,
  deleteSale,
  setIsEditModalOpen,
  downloadSaleDirectory,
  downloadDirectoryPath,
  dispatch,
  filters,
  uniqueExtraNames, // Nowa właściwość
}: GetSalesColumnsProps): GridColDef[] => {
  // Kolumny, które zawsze pojawiają się na początku
  const staticColumns: GridColDef[] = [
    { field: "cycleText", headerName: "Identyfikator", width: 120 },
    { field: "farmName", headerName: "Ferma", width: 180 },
    { field: "henhouseName", headerName: "Kurnik", width: 150 },
    { field: "slaughterhouseName", headerName: "Ubojnia", width: 180 },
    {
      field: "saleDate",
      headerName: "Data sprzedaży",
      width: 150,
      type: "date",
      valueGetter: (value: string) => (value ? dayjs(value).toDate() : null),
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
      width: 180,
      type: "number",
      headerAlign: "left",
      align: "left",
    },
  ];

  // Dynamiczne tworzenie kolumn dla każdego "dodatku"
  const extraColumns: GridColDef[] = uniqueExtraNames.map((extraName) => ({
    field: extraName,
    headerName: `${extraName} [zł]`,
    width: 160,
    type: "number",
    headerAlign: "left",
    align: "left",
  }));

  // Kolumny, które zawsze pojawiają się na końcu
  const finalColumns: GridColDef[] = [
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
      width: 120,
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
      headerName: "Dokumenty",
      align: "center",
      headerAlign: "center",
      sortable: false,
      width: 130,
      renderCell: (params) => (
        <FileDownloadCell
          filePath={params.row.directoryPath}
          downloadingFilePath={downloadDirectoryPath}
          onDownload={downloadSaleDirectory}
        />
      ),
    },
    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      width: 220,
      renderCell: (params) => (params.value ? params.value : "Brak numeru"),
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia",
      type: "dateTime",
      width: 180,
      valueGetter: (value: string) => (value ? dayjs(value).toDate() : null),
    },
  ];

  // Połączenie wszystkich kolumn w jedną listę
  return [...staticColumns, ...extraColumns, ...finalColumns];
};
