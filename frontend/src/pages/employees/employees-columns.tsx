import { Box, Button, Tooltip, Typography } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { CommentCell } from "../../components/datagrid/comment-cell";
import type {
  EmployeeListModel,
  EmployeeFileModel,
} from "../../models/employees/employees";

interface GetEmployeesColumnsProps {
  deleteEmployee: (id: string) => void;
}

export const getEmployeesColumns = ({
  deleteEmployee,
}: GetEmployeesColumnsProps): GridColDef<EmployeeListModel>[] => {
  return [
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "fullName",
      headerName: "Imię i nazwisko",
      flex: 1.5,
    },
    {
      field: "position",
      headerName: "Stanowisko",
      flex: 1,
    },
    {
      field: "contractType",
      headerName: "Rodzaj umowy",
      flex: 1,
    },
    {
      field: "salary",
      headerName: "Wynagrodzenie",
      flex: 1,
    },
    {
      field: "startDate",
      headerName: "Data rozpoczęcia",
      width: 150,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "";
      },
    },
    {
      field: "endDate",
      headerName: "Data zakończenia",
      width: 150,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "Czas nieokreślony";
      },
    },
    {
      field: "statusDesc",
      headerName: "Status",
      width: 120,
    },
    {
      field: "comment",
      headerName: "Uwagi",
      flex: 1,
      sortable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
    },
    {
      field: "files",
      headerName: "Pliki",
      flex: 1.5,
      sortable: false,
      renderCell: (params) => {
        const files = params.value as EmployeeFileModel[];
        if (!files || files.length === 0) {
          return (
            <Box display="flex" alignItems="center" height="100%">
              <Typography variant="body2" noWrap>
                Brak
              </Typography>
            </Box>
          );
        }

        const fileNames = files.map((file) => file.fileName).join(", ");

        return (
          <Box display="flex" alignItems="center" height="100%">
            <Tooltip title={fileNames}>
              <Typography variant="body2" noWrap>
                {fileNames}
              </Typography>
            </Tooltip>
          </Box>
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 100,
      getActions: (params) => [
        <Button
          key="delete"
          variant="outlined"
          size="small"
          color="error"
          onClick={(e) => {
            e.stopPropagation();
            deleteEmployee(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
