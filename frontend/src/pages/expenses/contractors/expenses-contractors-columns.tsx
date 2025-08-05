import type { GridColDef } from "@mui/x-data-grid";
import ActionsCell from "../../../components/datagrid/actions-cell";

export const getExpensesContractorsColumns = ({
  setSelectedExpenseContractor,
  deleteExpenseContractor,
  setIsEditModalOpen,
}: {
  setSelectedExpenseContractor: (s: any) => void;
  deleteExpenseContractor: (id: string) => void;
  setIsEditModalOpen: (v: boolean) => void;
}): GridColDef[] => {
  return [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "expenseType", headerName: "Typ wydatku", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
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
            setSelectedExpenseContractor(row);
            setIsEditModalOpen(true);
          }}
          onDelete={deleteExpenseContractor}
        />,
      ],
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
