import { useEffect, useMemo, useReducer, useState } from "react";
import {
  filterReducer,
  initialFilters,
  type DashboardDictionary,
  type DashboardFilters,
} from "../../models/dashboard/dashboard-filters";
import {
  Typography,
  Box,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  TextField,
  Grid,
  Skeleton,
  Paper,
  Divider,
} from "@mui/material";
import { type GridColDef, DataGrid } from "@mui/x-data-grid";
import { FaPercentage, FaPiggyBank, FaChartArea } from "react-icons/fa";
import {
  MdTrendingUp,
  MdTrendingDown,
  MdAccountBalanceWallet,
  MdOutlineShoppingCart,
} from "react-icons/md";
import StatCard from "../../components/dashboard/stat-card";
import type { CycleDictModel } from "../../models/common/dictionaries";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { DashboardService } from "../../services/dashboard-service";

interface DashboardData {
  stats: {
    revenue: { value: number; trend: string };
    expenses: { value: number; trend: string };
    income: { value: number; trend: string };
    vatFromExpenses: { value: number; trend: string };
  };
  kpi: {
    incomePerKg: { value: number; trend: string };
    incomePerSqm: { value: number; trend: string };
    avgFeedPrice: { value: number; trend: string };
  };
  chickenHouseStatus: {
    farms: { id: string; farmName: string; activeHouses: number }[];
    totalActive: number;
  };
  reminders: {
    id: string;
    type: string;
    description: string;
    dueDate: string;
    priority: "low" | "medium" | "high";
  }[];
}

const columns: GridColDef[] = [
  { field: "type", headerName: "Typ", width: 150 },
  { field: "description", headerName: "Opis", flex: 1 },
  { field: "dueDate", headerName: "Termin", width: 120 },
  {
    field: "priority",
    headerName: "Priorytet",
    width: 120,
    renderCell: (params) => (
      <Typography variant="body2">{params.value}</Typography>
    ),
  },
];

const DashboardPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<DashboardDictionary>();
  const [dictionaryLoading, setDictionaryLoading] = useState(true);
  const [data, setData] = useState<DashboardData | null>(null);
  const [initialLoading, setInitialLoading] = useState(true);
  const [statsLoading, setStatsLoading] = useState(false);

  const [dateCategory, setDateCategory] = useState("month");
  const [selectedMonth, setSelectedMonth] = useState(new Date().getMonth());
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
  const [selectedCycle, setSelectedCycle] = useState("");
  const [dateRange, setDateRange] = useState({ from: "", to: "" });

  useEffect(() => {
    const fetchDictionary = async () => {
      setDictionaryLoading(true);
      try {
        await handleApiResponse(
          () => DashboardService.getDictionaries(),
          (data) => setDictionary(data.responseData),
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania słowników filtrów");
      } finally {
        setDictionaryLoading(false);
      }
    };
    fetchDictionary();
  }, []);

  const uniqueCycles = useMemo(() => {
    if (!dictionary) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) {
        map.set(key, cycle);
      }
    }
    return Array.from(map.values());
  }, [dictionary]);

  useEffect(() => {
    let payload: Partial<DashboardFilters> = {};

    if (dateCategory === "cycle") {
      const foundCycle = selectedCycle
        ? uniqueCycles.find(
            (c) => `${c.identifier}-${c.year}` === selectedCycle
          )
        : undefined;

      payload = {
        cycle: foundCycle,
        dateFrom: null,
        dateTo: null,
      };
    } else {
      payload.cycle = undefined;
      if (dateCategory === "month") {
        const year = new Date().getFullYear();
        const from = new Date(year, selectedMonth, 1);
        const to = new Date(year, selectedMonth + 1, 0);
        payload.dateFrom = from.toISOString().split("T")[0];
        payload.dateTo = to.toISOString().split("T")[0];
      } else if (dateCategory === "year") {
        const from = new Date(selectedYear, 0, 1);
        const to = new Date(selectedYear, 11, 31);
        payload.dateFrom = from.toISOString().split("T")[0];
        payload.dateTo = to.toISOString().split("T")[0];
      } else if (dateCategory === "range") {
        payload.dateFrom = dateRange.from;
        payload.dateTo = dateRange.to;
      }
    }
    dispatch({ type: "setMultiple", payload });
  }, [
    dateCategory,
    selectedMonth,
    selectedYear,
    dateRange,
    selectedCycle,
    uniqueCycles,
  ]);

  useEffect(() => {
    const fetchData = (isInitial: boolean) => {
      if (isInitial) setInitialLoading(true);
      else setStatsLoading(true);

      console.log("Pobieranie danych z filtrami:", filters);

      setTimeout(() => {
        setData({
          stats: {
            revenue: { value: 76500, trend: "+5.2% vs poprzedni miesiąc" },
            expenses: { value: 42300, trend: "+1.8% vs poprzedni miesiąc" },
            income: { value: 34200, trend: "Zysk +12% r/r" },
            vatFromExpenses: {
              value: 7900,
              trend: "Do odliczenia w tym miesiącu",
            },
          },
          kpi: {
            incomePerKg: { value: 1.25, trend: "+0.15 zł vs ost. cykl" },
            incomePerSqm: { value: 85.5, trend: "+2.30 zł vs ost. cykl" },
            avgFeedPrice: { value: 1750, trend: "-25 zł vs ost. dostawa" },
          },
          chickenHouseStatus: {
            farms: [
              {
                id: "f1",
                farmName: "Ferma Drobiu Robert Różański",
                activeHouses: 9,
              },
              {
                id: "f2",
                farmName: "Ferma Drobiu Witold Jędrzejewski Jaworowo Kłódź",
                activeHouses: 6,
              },
            ],
            totalActive: 15,
          },
          reminders: [
            {
              id: "1",
              type: "Faktura",
              description: "Zapłata dla AgriFerm",
              dueDate: "20.08.2025",
              priority: "high",
            },
            {
              id: "2",
              type: "Umowa",
              description: "Koniec umowy Jan Kowalski",
              dueDate: "25.08.2025",
              priority: "medium",
            },
          ],
        });
        if (isInitial) setInitialLoading(false);
        else setStatsLoading(false);
      }, 1000);
    };

    if (!dictionary) return;

    if (data === null) {
      fetchData(true);
    } else {
      fetchData(false);
    }
  }, [filters, dictionary]);

  if (initialLoading || dictionaryLoading || !data) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  const months = [
    "Styczeń",
    "Luty",
    "Marzec",
    "Kwiecień",
    "Maj",
    "Czerwiec",
    "Lipiec",
    "Sierpień",
    "Wrzesień",
    "Październik",
    "Listopad",
    "Grudzień",
  ];

  return (
    <Box p={3}>
      <Box
        mb={3}
        display="flex"
        flexDirection={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "flex-start", sm: "center" }}
      >
        <Typography variant="h4">Dashboard</Typography>
        <Box display="flex" gap={2} alignItems="center" flexWrap="wrap">
          <FormControl sx={{ minWidth: 200 }} size="small">
            <InputLabel>Ferma</InputLabel>
            <Select
              label="Ferma"
              defaultValue=""
              onChange={(e) =>
                dispatch({
                  type: "set",
                  key: "farmId",
                  value: e.target.value,
                })
              }
            >
              <MenuItem value="">Wszystkie fermy</MenuItem>
              {dictionary?.farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <FormControl sx={{ minWidth: 120 }} size="small">
            <InputLabel>Okres</InputLabel>
            <Select
              label="Okres"
              value={dateCategory}
              onChange={(e) => setDateCategory(e.target.value)}
            >
              <MenuItem value="month">Miesiąc</MenuItem>
              <MenuItem value="year">Rok</MenuItem>
              <MenuItem value="range">Zakres dat</MenuItem>
              <MenuItem value="cycle">Cykl</MenuItem>
            </Select>
          </FormControl>

          {dateCategory === "cycle" && (
            <FormControl sx={{ minWidth: 150 }} size="small">
              <InputLabel>Wybierz Cykl</InputLabel>
              <Select
                label="Wybierz Cykl"
                value={selectedCycle}
                onChange={(e) => setSelectedCycle(e.target.value as string)}
              >
                {uniqueCycles.map((cycle) => (
                  <MenuItem
                    key={cycle.id}
                    value={`${cycle.identifier}-${cycle.year}`}
                  >
                    {`${cycle.identifier}/${cycle.year}`}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}

          {dateCategory === "month" && (
            <FormControl sx={{ minWidth: 120 }} size="small">
              <InputLabel>Miesiąc</InputLabel>
              <Select
                label="Miesiąc"
                value={selectedMonth}
                onChange={(e) => setSelectedMonth(Number(e.target.value))}
              >
                {months.map((name, index) => (
                  <MenuItem key={name} value={index}>
                    {name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}

          {dateCategory === "year" && (
            <TextField
              label="Rok"
              type="number"
              size="small"
              sx={{ width: 100 }}
              value={selectedYear}
              onChange={(e) => setSelectedYear(Number(e.target.value))}
            />
          )}

          {dateCategory === "range" && (
            <>
              <TextField
                label="Data od"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                onChange={(e) =>
                  setDateRange((prev) => ({ ...prev, from: e.target.value }))
                }
              />
              <TextField
                label="Data do"
                type="date"
                size="small"
                InputLabelProps={{ shrink: true }}
                onChange={(e) =>
                  setDateRange((prev) => ({ ...prev, to: e.target.value }))
                }
              />
            </>
          )}
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="Przychody"
              value={data.stats.revenue.value.toString()}
              trend={data.stats.revenue.trend}
              icon={<MdTrendingUp />}
              color="success"
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="Wydatki"
              value={data.stats.expenses.value.toString()}
              trend={data.stats.expenses.trend}
              icon={<MdTrendingDown />}
              color="error"
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="Dochód"
              value={data.stats.income.value.toString()}
              trend={data.stats.income.trend}
              icon={<MdAccountBalanceWallet />}
              color="primary"
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="VAT z wydatków"
              value={data.stats.vatFromExpenses.value.toString()}
              trend={data.stats.vatFromExpenses.trend}
              icon={<FaPercentage />}
              color="warning"
            />
          )}
        </Grid>

        <Grid size={{ xs: 12, lg: 8 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={400} />
          ) : (
            <Paper sx={{ p: 2, height: 400 }}>
              <Typography variant="h6">Wyniki produkcyjne</Typography>
            </Paper>
          )}
        </Grid>
        <Grid size={{ xs: 12, lg: 4 }}>
          <Grid container spacing={2}>
            <Grid size={{ xs: 12, sm: 6, lg: 12 }}>
              {statsLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z kg żywca"
                  value={`${data.kpi.incomePerKg.value.toString()} zł`}
                  trend={data.kpi.incomePerKg.trend}
                  icon={<FaPiggyBank />}
                  color="success"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 6, lg: 12 }}>
              {statsLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z m²"
                  value={`${data.kpi.incomePerSqm.value.toString()} zł`}
                  trend={data.kpi.incomePerSqm.trend}
                  icon={<FaChartArea />}
                  color="primary"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 6, lg: 12 }}>
              {statsLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Średnia cena paszy"
                  value={`${data.kpi.avgFeedPrice.value.toString()} zł/t`}
                  trend={data.kpi.avgFeedPrice.trend}
                  icon={<MdOutlineShoppingCart />}
                  color="info"
                />
              )}
            </Grid>
          </Grid>
        </Grid>

        <Grid size={{ xs: 12, sm: 8 }}>
          <Paper sx={{ p: 2, height: "100%" }}>
            <Typography variant="h6" mb={2}>
              Liczba kurników w obsadzie
            </Typography>
            {data.chickenHouseStatus.farms.map((farm) => (
              <Typography key={farm.id} variant="body1">
                {farm.farmName} - <strong>{farm.activeHouses}</strong> kurników
              </Typography>
            ))}
            <Divider sx={{ my: 2 }} />
            <Typography variant="h6">
              Łącznie: <strong>{data.chickenHouseStatus.totalActive}</strong>{" "}
              kurników w obsadzie
            </Typography>
          </Paper>
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          {statsLoading ? (
            <Skeleton variant="rounded" height={300} />
          ) : (
            <Paper sx={{ p: 2, height: 300 }}>
              <Typography variant="h6">Finanse</Typography>
            </Paper>
          )}
        </Grid>

        <Grid size={{ xs: 12 }}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" mb={2}>
              Nadchodzące terminy i przypomnienia
            </Typography>
            <Box sx={{ height: 300, width: "100%" }}>
              <DataGrid
                rows={data.reminders}
                columns={columns}
                rowSelection={false}
              />
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default DashboardPage;
