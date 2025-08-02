import React from "react";
import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Select,
  MenuItem,
  TextField,
  IconButton,
} from "@mui/material";
import { MdDelete } from "react-icons/md";
import type { AddEmployeePayslipEntry } from "../../../models/employees/employees-payslips";

interface Employee {
  id: string;
  fullName: string;
  salary: number;
}

interface PayslipEntriesTableProps {
  entries: AddEmployeePayslipEntry[];
  employees: Employee[];
  errors:
    | {
        [index: number]: Partial<Record<keyof AddEmployeePayslipEntry, string>>;
      }
    | undefined;
  dispatch: React.Dispatch<any>;
  farmId: string;
  loadingEmployees: boolean;
}

const PayslipEntriesTable: React.FC<PayslipEntriesTableProps> = ({
  entries,
  employees,
  errors,
  dispatch,
  farmId,
  loadingEmployees,
}) => {
  const handleEntryChange = (
    index: number,
    name: keyof AddEmployeePayslipEntry,
    value: string
  ) => {
    dispatch({ type: "UPDATE_ENTRY", index, name, value });
  };

  const handleEmployeeChange = (index: number, employeeId: string) => {
    const selectedEmployee = employees.find((e) => e.id === employeeId);
    handleEntryChange(index, "employeeId", employeeId);
    if (selectedEmployee) {
      handleEntryChange(
        index,
        "baseSalary",
        selectedEmployee.salary.toString()
      );
    }
  };

  const calculateNetPay = (entry: AddEmployeePayslipEntry): string => {
    const baseSalary = Number(entry.baseSalary) || 0;
    const bankTransferAmount = Number(entry.bankTransferAmount) || 0;
    const bonusAmount = Number(entry.bonusAmount) || 0;
    const overtimePay = Number(entry.overtimePay) || 0;
    const deductions = Number(entry.deductions) || 0;
    const otherAllowances = Number(entry.otherAllowances) || 0;

    const netPay =
      baseSalary -
      bankTransferAmount +
      bonusAmount +
      overtimePay -
      deductions +
      otherAllowances;
    return netPay.toFixed(2);
  };

  const availableEmployees = employees.filter(
    (emp) => !entries.some((en) => en.employeeId === emp.id)
  );

  return (
    <Table size="small">
      <TableHead>
        <TableRow>
          <TableCell sx={{ width: "15%" }}>Pracownik</TableCell>
          <TableCell align="right">Pensja podst. [zł]</TableCell>
          <TableCell align="right">Konto [zł]</TableCell>
          <TableCell align="right">Premia [zł]</TableCell>
          <TableCell align="right">Nadgodziny [zł]</TableCell>
          <TableCell align="right">Nadgodziny [h]</TableCell>
          <TableCell align="right">Potrącenia [zł]</TableCell>
          <TableCell align="right">Inne dodatki [zł]</TableCell>
          <TableCell align="right" sx={{ fontWeight: "bold" }}>
            Do wypłaty [zł]
          </TableCell>
          <TableCell>Komentarz</TableCell>
          <TableCell align="center">Akcje</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {entries.map((entry, index) => (
          <TableRow key={index}>
            <TableCell>
              <Select
                value={entry.employeeId}
                onChange={(e) => handleEmployeeChange(index, e.target.value)}
                fullWidth
                variant="standard"
                disabled={!farmId || loadingEmployees}
                error={!!errors?.[index]?.employeeId}
              >
                {employees.find((e) => e.id === entry.employeeId) && (
                  <MenuItem key={entry.employeeId} value={entry.employeeId}>
                    {employees.find((e) => e.id === entry.employeeId)?.fullName}
                  </MenuItem>
                )}
                {availableEmployees.map((emp) => (
                  <MenuItem key={emp.id} value={emp.id}>
                    {emp.fullName}
                  </MenuItem>
                ))}
              </Select>
            </TableCell>

            {/* Renderowanie wszystkich pól numerycznych */}
            {(
              [
                "baseSalary",
                "bankTransferAmount",
                "bonusAmount",
                "overtimePay",
                "overtimeHours",
                "deductions",
                "otherAllowances",
              ] as const
            ).map((key) => (
              <TableCell key={key} align="right">
                <TextField
                  type="number"
                  variant="standard"
                  value={entry[key]}
                  onChange={(e) =>
                    handleEntryChange(index, key, e.target.value)
                  }
                  error={!!errors?.[index]?.[key]}
                  fullWidth
                  slotProps={{ htmlInput: { min: 0, step: "0.01" } }}
                />
              </TableCell>
            ))}

            {/* Kolumna "Do wypłaty" */}
            <TableCell align="right">
              <TextField
                variant="standard"
                value={calculateNetPay(entry)}
                fullWidth
                sx={{ fontWeight: "bold" }}
                slotProps={{
                  htmlInput: { readOnly: true, style: { textAlign: "right" } },
                }}
              />
            </TableCell>

            {/* Kolumna "Komentarz" */}
            <TableCell>
              <TextField
                variant="standard"
                value={entry.comment || ""}
                onChange={(e) =>
                  handleEntryChange(index, "comment", e.target.value)
                }
                fullWidth
              />
            </TableCell>

            <TableCell align="center">
              <IconButton
                onClick={() => dispatch({ type: "REMOVE_ENTRY", index })}
                color="error"
              >
                <MdDelete />
              </IconButton>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default PayslipEntriesTable;
