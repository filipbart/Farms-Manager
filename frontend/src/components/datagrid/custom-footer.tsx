import { Box, Typography } from "@mui/material";
import { GridFooter } from "@mui/x-data-grid";
import { useMemo } from "react";
import type { EmployeePayslipListModel } from "../../models/employees/employees-payslips";

const formatCurrency = (value: number) =>
  new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: "PLN",
  }).format(value);

interface CustomFooterProps {
  rows: EmployeePayslipListModel[];
}

export const CustomFooter: React.FC<CustomFooterProps> = ({ rows }) => {
  const sums = useMemo(
    () => ({
      baseSalary: rows.reduce((acc, row) => acc + (row.baseSalary || 0), 0),
      bankTransferAmount: rows.reduce(
        (acc, row) => acc + (row.bankTransferAmount || 0),
        0
      ),
      bonusAmount: rows.reduce((acc, row) => acc + (row.bonusAmount || 0), 0),
      overtimePay: rows.reduce((acc, row) => acc + (row.overtimePay || 0), 0),
      overtimeHours: rows.reduce(
        (acc, row) => acc + (row.overtimeHours || 0),
        0
      ),
      deductions: rows.reduce((acc, row) => acc + (row.deductions || 0), 0),
      otherAllowances: rows.reduce(
        (acc, row) => acc + (row.otherAllowances || 0),
        0
      ),
      netPay: rows.reduce((acc, row) => acc + (row.netPay || 0), 0),
    }),
    [rows]
  );

  return (
    <Box>
      <Box
        sx={{
          p: 2,
          borderTop: 1,
          borderColor: "divider",
          bgcolor: "action.hover",
          overflowX: "auto",
        }}
      >
        <Box
          sx={{
            display: "flex",
            gap: 3,
            minWidth: "800px", // wymuszona szerokość, by ułatwić scroll
            flexWrap: { xs: "wrap", md: "nowrap" },
            alignItems: "center",
          }}
        >
          <Typography variant="subtitle2" fontWeight="bold">
            SUMY:
          </Typography>

          <Typography variant="body2">
            Wypłata: <strong>{formatCurrency(sums.baseSalary)}</strong>
          </Typography>

          <Typography variant="body2">
            Na konto: <strong>{formatCurrency(sums.bankTransferAmount)}</strong>
          </Typography>

          <Typography variant="body2">
            Premia: <strong>{formatCurrency(sums.bonusAmount)}</strong>
          </Typography>

          <Typography variant="body2">
            Nadgodziny: <strong>{formatCurrency(sums.overtimePay)}</strong>
          </Typography>

          <Typography variant="body2">
            Nadgodziny (h): <strong>{sums.overtimeHours.toFixed(2)} h</strong>
          </Typography>

          <Typography variant="body2">
            Potrącenia: <strong>{formatCurrency(sums.deductions)}</strong>
          </Typography>

          <Typography variant="body2">
            Inne dodatki:{" "}
            <strong>{formatCurrency(sums.otherAllowances)}</strong>
          </Typography>

          <Typography variant="body2">
            Do wypłaty: <strong>{formatCurrency(sums.netPay)}</strong>
          </Typography>
        </Box>
      </Box>

      <GridFooter />
    </Box>
  );
};
