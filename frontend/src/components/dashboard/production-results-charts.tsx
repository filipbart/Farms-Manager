import { useState, useMemo } from "react";
import {
  Paper,
  Typography,
  Box,
  FormControl,
  Select,
  MenuItem,
} from "@mui/material";
import { LineChart } from "@mui/x-charts/LineChart";
import type {
  DashboardFcrChart,
  DashboardEwwChart,
  DashboardGasConsumptionChart,
  DashboardFlockLossChart,
  ChartSeries,
} from "../../models/dashboard/dashboard"; // Popraw ścieżkę do interfejsów

type ChartType = "fcr" | "eww" | "gas" | "loss";

interface ProductionResultsChartProps {
  fcrData: DashboardFcrChart;
  ewwData: DashboardEwwChart;
  gasData: DashboardGasConsumptionChart;
  lossData: DashboardFlockLossChart;
}

const chartOptions = {
  fcr: { label: "FCR (Współczynnik konwersji paszy)" },
  eww: { label: "EWW (Europejski wskaźnik wydajności)" },
  gas: { label: "Zużycie gazu (m2)" },
  loss: { label: "Upadki i braki (%)" },
};

export function ProductionResultsChart({
  fcrData,
  ewwData,
  gasData,
  lossData,
}: ProductionResultsChartProps) {
  const [selectedChart, setSelectedChart] = useState<ChartType>("fcr");

  const { xAxisData, seriesData } = useMemo(() => {
    let sourceData: { series: ChartSeries[] } | undefined;
    switch (selectedChart) {
      case "fcr":
        sourceData = fcrData;
        break;
      case "eww":
        sourceData = ewwData;
        break;
      case "gas":
        sourceData = gasData;
        break;
      case "loss":
        sourceData = lossData;
        break;
      default:
        sourceData = { series: [] };
    }

    if (!sourceData || sourceData.series.length === 0) {
      return { xAxisData: [], seriesData: [] };
    }

    const allXValues = new Set<string>();
    sourceData.series.forEach((s) =>
      s.data.forEach((d) => allXValues.add(d.x))
    );
    const sortedXValues = Array.from(allXValues).sort((a, b) => {
      const [aId, aYear] = a.split("/");
      const [bId, bYear] = b.split("/");
      return (
        Number(aYear) * 100 + Number(aId) - (Number(bYear) * 100 + Number(bId))
      );
    });

    const transformedSeries = sourceData.series.map((s) => {
      const dataMap = new Map(s.data.map((d) => [d.x, d.y]));
      return {
        label: s.farmName,
        data: sortedXValues.map((x) => dataMap.get(x) ?? null),
      };
    });

    return { xAxisData: sortedXValues, seriesData: transformedSeries };
  }, [selectedChart, fcrData, ewwData, gasData, lossData]);

  const hasData = xAxisData.length > 0 && seriesData.length > 0;

  return (
    <Paper sx={{ p: 2, height: 375, display: "flex", flexDirection: "column" }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
        }}
      >
        <Typography variant="h6">Wyniki produkcyjne</Typography>
        <FormControl size="small" sx={{ minWidth: 240 }}>
          <Select
            value={selectedChart}
            onChange={(e) => setSelectedChart(e.target.value as ChartType)}
          >
            {Object.entries(chartOptions).map(([key, { label }]) => (
              <MenuItem key={key} value={key}>
                {label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>

      <Box sx={{ flexGrow: 1 }}>
        {hasData ? (
          <LineChart
            height={300}
            xAxis={[{ data: xAxisData, scaleType: "band" }]}
            series={seriesData}
            margin={{ top: 10, right: 20, bottom: 30, left: 50 }}
            grid={{ vertical: true, horizontal: true }}
          />
        ) : (
          <Box
            sx={{
              display: "flex",
              height: "100%",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <Typography color="text.secondary">
              Brak danych do wyświetlenia wykresu.
            </Typography>
          </Box>
        )}
      </Box>
    </Paper>
  );
}
