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
import { DashboardNotifications } from "../../components/dashboard/dashboard-notifications";
import type { GetDashboardDataQueryResponse } from "../../models/dashboard/dashboard";
import { ExpensesPieChart } from "../../components/dashboard/expenses-pie-chart";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import type { Dayjs } from "dayjs";
import { ProductionResultsChart } from "../../components/dashboard/production-results-charts";

const emptyDashboardData: GetDashboardDataQueryResponse = {
  stats: {
    revenue: 0,
    expenses: 0,
    income: 0,
    vatFromExpenses: 0,
    incomePerKg: 0,
    incomePerSqm: 0,
    avgFeedPrice: 0,
  },
  fcrChart: { series: [] },
  gasConsumptionChart: { series: [] },
  ewwChart: { series: [] },
  flockLossChart: { series: [] },
  chickenHousesStatus: {
    farms: [],
    totalHenhousesCount: 0,
    totalChickenCount: 0,
  },
  expensesPieChart: { data: [] },
  notifications: [],
};

const DashboardPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<DashboardDictionary>();
  const [data, setData] = useState<GetDashboardDataQueryResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const [dateCategory, setDateCategory] = useState("month");
  const [selectedMonth, setSelectedMonth] = useState(new Date().getMonth());
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());
  const [selectedCycle, setSelectedCycle] = useState("");
  const [dateRange, setDateRange] = useState<{
    from: Dayjs | null;
    to: Dayjs | null;
  }>({ from: null, to: null });

  useEffect(() => {
    const fetchDictionary = async () => {
      try {
        await handleApiResponse(
          () => DashboardService.getDictionaries(),
          (data) => setDictionary(data.responseData)
        );
      } catch (error) {
        toast.error(`Wystąpił błąd podczas pobierania słowników: ${error}`);
      }
    };
    fetchDictionary();
  }, []);

  const uniqueCycles = useMemo(() => {
    if (!dictionary) return [];
    const map = new Map<string, CycleDictModel>();
    dictionary.cycles.forEach((cycle) => {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) map.set(key, cycle);
    });
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
      payload = { cycle: foundCycle, dateSince: null, dateTo: null };
    } else {
      let dateSince: string | null = null;
      let dateTo: string | null = null;

      if (dateCategory === "month") {
        const from = new Date(selectedYear, selectedMonth, 1);

        const to = new Date(selectedYear, selectedMonth + 1, 0);
        dateSince = from.toISOString().split("T")[0];
        dateTo = to.toISOString().split("T")[0];
      } else if (dateCategory === "year") {
        const from = new Date(selectedYear, 0, 1);

        const to = new Date(selectedYear, 11, 31);
        dateSince = from.toISOString().split("T")[0];
        dateTo = to.toISOString().split("T")[0];
      } else if (dateCategory === "range") {
        dateSince = dateRange.from?.format("YYYY-MM-DD") ?? null;
        dateTo = dateRange.to?.format("YYYY-MM-DD") ?? null;
      }

      payload = { cycle: undefined, dateSince, dateTo };
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
    const fetchData = async () => {
      if (!dictionary) return;
      if (!filters || Object.keys(filters).length === 0) return;

      setIsLoading(true);
      try {
        await handleApiResponse(
          () => DashboardService.getDashboardData(filters),
          (apiData) => {
            setData(apiData.responseData ?? null);
          }
        );
      } catch (error) {
        toast.error(`Wystąpił błąd podczas ładowania danych: ${error}`);
        setData(null);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [filters, dictionary]);

  const displayData = data ?? emptyDashboardData;
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

  if (!dictionary) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

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
          <FormControl sx={{ minWidth: 200 }} size="small" disabled={isLoading}>
            <InputLabel>Ferma</InputLabel>
            <Select
              label="Ferma"
              defaultValue=""
              onChange={(e) =>
                dispatch({ type: "set", key: "farmId", value: e.target.value })
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
          <FormControl sx={{ minWidth: 120 }} size="small" disabled={isLoading}>
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
            <FormControl
              sx={{ minWidth: 150 }}
              size="small"
              disabled={isLoading}
            >
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
                  >{`${cycle.identifier}/${cycle.year}`}</MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
          {dateCategory === "month" && (
            <FormControl
              sx={{ minWidth: 120 }}
              size="small"
              disabled={isLoading}
            >
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
              disabled={isLoading}
            />
          )}
          {dateCategory === "range" && (
            <>
              <DatePicker
                label="Data od"
                value={dateRange.from}
                onChange={(newValue) =>
                  setDateRange((prev) => ({ ...prev, from: newValue }))
                }
                disabled={isLoading}
                slotProps={{ textField: { size: "small" } }}
              />
              <DatePicker
                label="Data do"
                value={dateRange.to}
                onChange={(newValue) =>
                  setDateRange((prev) => ({ ...prev, to: newValue }))
                }
                disabled={isLoading}
                slotProps={{ textField: { size: "small" } }}
              />
            </>
          )}
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, lg: 8 }}>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Przychody"
                  value={displayData.stats.revenue.toLocaleString("pl-PL", {
                    style: "currency",
                    currency: "PLN",
                  })}
                  icon={<MdTrendingUp />}
                  color="success"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Wydatki"
                  value={displayData.stats.expenses.toLocaleString("pl-PL", {
                    style: "currency",
                    currency: "PLN",
                  })}
                  icon={<MdTrendingDown />}
                  color="error"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 12, md: 4 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Dochód"
                  value={displayData.stats.income.toLocaleString("pl-PL", {
                    style: "currency",
                    currency: "PLN",
                  })}
                  icon={<MdAccountBalanceWallet />}
                  color="primary"
                />
              )}
            </Grid>
          </Grid>
        </Grid>
        <Grid size={{ xs: 12, sm: 12, lg: 4 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="VAT z wydatków"
              value={displayData.stats.vatFromExpenses.toLocaleString("pl-PL", {
                style: "currency",
                currency: "PLN",
              })}
              icon={<FaPercentage />}
              color="warning"
            />
          )}
        </Grid>

        <Grid size={{ xs: 12, lg: 8 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height={400} />
          ) : (
            <ProductionResultsChart
              fcrData={displayData.fcrChart}
              ewwData={displayData.ewwChart}
              gasData={displayData.gasConsumptionChart}
              lossData={displayData.flockLossChart}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, lg: 4 }}>
          <Grid container spacing={{ xs: 3, lg: 2 }}>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z kg żywca"
                  value={`${displayData.stats.incomePerKg.toLocaleString(
                    "pl-PL",
                    { minimumFractionDigits: 2, maximumFractionDigits: 2 }
                  )} zł`}
                  icon={<FaPiggyBank />}
                  color="success"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z m²"
                  value={`${displayData.stats.incomePerSqm.toLocaleString(
                    "pl-PL",
                    { minimumFractionDigits: 2, maximumFractionDigits: 2 }
                  )} zł`}
                  icon={<FaChartArea />}
                  color="primary"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoading ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Średnia cena paszy"
                  value={`${displayData.stats.avgFeedPrice.toLocaleString(
                    "pl-PL"
                  )} zł/t`}
                  icon={<MdOutlineShoppingCart />}
                  color="info"
                />
              )}
            </Grid>
          </Grid>
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 6, lg: 8 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <Paper
              sx={{
                p: 2,
                height: "100%",
                display: "flex",
                flexDirection: "column",
              }}
            >
              <Typography variant="h6" mb={1}>
                Kurniki w obsadzie
              </Typography>
              <Box sx={{ flexGrow: 1, overflowY: "auto" }}>
                {displayData.chickenHousesStatus.farms.map((farm) => {
                  const activeHenhouses = farm.henhouses.filter(
                    (h) => h.chickenCount > 0
                  );
                  if (activeHenhouses.length === 0) return null;
                  return (
                    <Box key={farm.name} mb={2}>
                      <Typography
                        variant="subtitle1"
                        sx={{ fontWeight: "bold" }}
                      >
                        {farm.name}
                      </Typography>
                      <Box sx={{ pl: 2 }}>
                        {activeHenhouses.map((henhouse) => (
                          <Typography
                            key={henhouse.name}
                            variant="body2"
                            color="text.secondary"
                          >
                            {henhouse.name}:{" "}
                            <strong>
                              {henhouse.chickenCount.toLocaleString("pl-PL")}{" "}
                              szt.
                            </strong>
                          </Typography>
                        ))}
                      </Box>
                    </Box>
                  );
                })}
              </Box>
              <Divider sx={{ my: 1 }} />
              <Typography variant="h6">
                Łącznie sztuk w obsadzie:{" "}
                <strong>
                  {displayData.chickenHousesStatus.totalChickenCount.toLocaleString(
                    "pl-PL"
                  )}
                </strong>
              </Typography>
            </Paper>
          )}
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <ExpensesPieChart data={displayData.expensesPieChart.data} />
          )}
        </Grid>

        <Grid size={{ xs: 12 }} sx={{ height: 380 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <DashboardNotifications notifications={displayData.notifications} />
          )}
        </Grid>
      </Grid>
    </Box>
  );
};

export default DashboardPage;
