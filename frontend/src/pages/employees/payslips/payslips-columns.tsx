import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import { CommentCell } from "../../../components/datagrid/comment-cell";
import type { EmployeePayslipListModel } from "../../../models/employees/employees-payslips";

interface GetEmployeePayslipColumnsProps {
  setSelectedPayslip: (payslip: EmployeePayslipListModel) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  deletePayslip: (id: string) => void;
}

export const getEmployeePayslipColumns = ({
  setSelectedPayslip,
  setIsEditModalOpen,
  deletePayslip,
}: GetEmployeePayslipColumnsProps): GridColDef<EmployeePayslipListModel>[] => {
  return [
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "cycleText",
      headerName: "Identyfikator (cykl)",
      flex: 1,
    },
    {
      field: "payrollPeriodDesc",
      headerName: "Okres rozliczeniowy",
      flex: 1,
    },
    {
      field: "employeeFullName",
      headerName: "Pracownik",
      flex: 1.5,
    },

    {
      field: "baseSalary",
      headerName: "Wypłata [zł]",
      flex: 1,
    },
    {
      field: "bankTransferAmount",
      headerName: "Konto [zł]",
      flex: 1,
    },
    {
      field: "bonusAmount",
      headerName: "Premia [zł]",
      flex: 1,
    },
    {
      field: "overtimePay",
      headerName: "Nadgodziny [zł]",
      flex: 1,
    },
    {
      field: "overtimeHours",
      headerName: "Nadgodziny [h]",
      flex: 1,
    },
    {
      field: "deductions",
      headerName: "Potrącenia [zł]",
      flex: 1,
    },
    {
      field: "otherAllowances",
      headerName: "Inne dodatki [zł]",
      flex: 1,
    },
    {
      field: "netPay",
      headerName: "Do wypłaty [zł]",
      flex: 1,
    },
    {
      field: "comment",
      headerName: "Uwagi",
      flex: 1,
      sortable: false,
      renderCell: (params) => <CommentCell value={params.value} />,
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
            setSelectedPayslip(params.row);
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
            deletePayslip(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
