import { Box, Typography, Grid } from "@mui/material";
import { GridFooter } from "@mui/x-data-grid";
import { useMemo } from "react";
import type { EmployeePayslipListModel } from "../../models/employees/employees-payslips";

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: "PLN",
  }).format(value);
};

interface CustomFooterProps {
  rows: EmployeePayslipListModel[];
}

export const CustomFooter: React.FC<CustomFooterProps> = ({ rows }) => {
  const sums = useMemo(() => {
    return {
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
    };
  }, [rows]);

  return (
    <Box>
      <Grid
        container
        spacing={2}
        sx={{
          p: 1.5,
          borderTop: 1,
          borderColor: "divider",
          borderBottom: "none",
          bgcolor: "action.hover",
          alignItems: "center",
          justifyContent: { xs: "flex-start", md: "center" },
        }}
      >
        <Grid>
          <Typography variant="body2" sx={{ fontWeight: "bold" }}>
            SUMY:
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Wypłata: <strong>{formatCurrency(sums.baseSalary)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Na konto: <strong>{formatCurrency(sums.bankTransferAmount)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Premia: <strong>{formatCurrency(sums.bonusAmount)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Nadgodziny: <strong>{formatCurrency(sums.overtimePay)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Nadgodziny (h): <strong>{sums.overtimeHours.toFixed(2)} h</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Potrącenia: <strong>{formatCurrency(sums.deductions)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Inne dodatki:{" "}
            <strong>{formatCurrency(sums.otherAllowances)}</strong>
          </Typography>
        </Grid>
        <Grid>
          <Typography variant="body2">
            Do wypłaty: <strong>{formatCurrency(sums.netPay)}</strong>
          </Typography>
        </Grid>
      </Grid>

      <GridFooter />
    </Box>
  );
};
