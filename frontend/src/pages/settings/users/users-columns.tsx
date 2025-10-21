import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { UserListModel } from "../../../models/users/users";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { getAuditColumns } from "../../../utils/audit-columns-helper";

interface GetUsersColumnsProps {
  deleteUser: (id: string) => void;
  isAdmin?: boolean;
}

export const getUsersColumns = ({
  deleteUser,
  isAdmin = false,
}: GetUsersColumnsProps): GridColDef<UserListModel>[] => {
  const baseColumns: GridColDef<UserListModel>[] = [
    {
      field: "login",
      headerName: "Login",
      flex: 1.5,
    },
    {
      field: "name",
      headerName: "Imię i nazwisko",
      flex: 2,
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia",
      width: 180,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "";
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 150,
      getActions: (params) => {
        if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
          return [];
        }
        return [
          <ActionsCell key="actions" params={params} onDelete={deleteUser} />,
        ];
      },
    },
  ];
  
  const auditColumns = getAuditColumns<UserListModel>(isAdmin);
  return [...baseColumns, ...auditColumns];
};
