import { Paper, Typography, Box } from "@mui/material";
import { PieChart } from "@mui/x-charts/PieChart";
import type { ExpensesPieChartDataPoint } from "../../models/dashboard/dashboard";

interface ExpensesPieChartProps {
  data?: ExpensesPieChartDataPoint[];
}

export function ExpensesPieChart({ data }: ExpensesPieChartProps) {
  const hasData = data && data.length > 0;

  return (
    <Paper
      sx={{ p: 2, height: "100%", display: "flex", flexDirection: "column" }}
    >
      <Typography variant="h6" mb={1}>
        Struktura wydatków
      </Typography>

      {hasData ? (
        <PieChart
          series={[
            {
              data: data,
              arcLabel: (item) => `${item.value}%`,
              valueFormatter: (item) => `${item.value}%`,
              innerRadius: 30,
              outerRadius: 100,
              paddingAngle: 2,
              cornerRadius: 5,
              highlightScope: { fade: "global", highlight: "item" },
              faded: { innerRadius: 30, additionalRadius: -10, color: "gray" },
            },
          ]}
          slotProps={{
            legend: {
              direction: "vertical",
              position: { vertical: "middle", horizontal: "end" },
              sx: {
                "& .MuiChartsLegend-label": {
                  fontSize: "0.9rem",
                },
              },
            },
          }}
          height={250}
        />
      ) : (
        <Box
          sx={{
            flexGrow: 1,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Typography color="text.secondary">
            Brak danych o wydatkach
          </Typography>
        </Box>
      )}
    </Paper>
  );
}
