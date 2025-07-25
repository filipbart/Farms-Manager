import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";

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
    { field: "id", headerName: "Id", width: 70 },
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "expenseType", headerName: "Typ wydatku", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedExpenseContractor(params.row);
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
            deleteExpenseContractor(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];
};
