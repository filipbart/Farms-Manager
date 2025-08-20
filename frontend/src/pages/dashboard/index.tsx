import { useEffect, useMemo, useReducer, useState } from "react";
import {
  filterReducer,
  initialFilters,
  type DashboardDictionary,
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
  const [dictionaryLoading, setDictionaryLoading] = useState(true);
  const [data, setData] = useState<GetDashboardDataQueryResponse | null>(null);
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
      } catch (error) {
        toast.error(`Wystąpił nieoczekiwany błąd: ${error}`);
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
    let payload = {};
    if (dateCategory === "cycle") {
      const foundCycle = selectedCycle
        ? uniqueCycles.find(
            (c) => `${c.identifier}-${c.year}` === selectedCycle
          )
        : undefined;
      payload = { cycle: foundCycle, dateFrom: null, dateTo: null };
    } else {
      let from: Date | undefined, to: Date | undefined;
      payload = { cycle: undefined };
      if (dateCategory === "month") {
        from = new Date(selectedYear, selectedMonth, 1);
        to = new Date(selectedYear, selectedMonth + 1, 0);
      } else if (dateCategory === "year") {
        from = new Date(selectedYear, 0, 1);
        to = new Date(selectedYear, 11, 31);
      } else if (dateCategory === "range" && dateRange.from && dateRange.to) {
        payload = {
          ...payload,
          dateFrom: dateRange.from,
          dateTo: dateRange.to,
        };
      }

      if (from && to) {
        payload = {
          ...payload,
          dateFrom: from.toISOString().split("T")[0],
          dateTo: to.toISOString().split("T")[0],
        };
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
    const fetchData = async () => {
      if (!dictionary) return;
      if (!filters || Object.keys(filters).length === 0) return;

      const isInitial = data === null;
      if (isInitial) setInitialLoading(true);
      else setStatsLoading(true);

      try {
        await handleApiResponse(
          () => DashboardService.getDashboardData(filters),
          (apiData) => {
            setData(apiData.responseData ?? null);
          },
          undefined,
          "Błąd podczas pobierania danych dashboardu"
        );
      } catch (error) {
        toast.error(`Wystąpił nieoczekiwany błąd: ${error}`);
        setData(null);
      } finally {
        if (isInitial) setInitialLoading(false);
        else setStatsLoading(false);
      }
    };

    fetchData();
  }, [filters, dictionary]);

  const isLoading = initialLoading || statsLoading;
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
          {dictionaryLoading ? (
            <CircularProgress size={24} />
          ) : (
            <>
              <FormControl
                sx={{ minWidth: 200 }}
                size="small"
                disabled={isLoading}
              >
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

              <FormControl
                sx={{ minWidth: 120 }}
                size="small"
                disabled={isLoading}
              >
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
                      >
                        {`${cycle.identifier}/${cycle.year}`}
                      </MenuItem>
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
                  <TextField
                    label="Data od"
                    type="date"
                    size="small"
                    InputLabelProps={{ shrink: true }}
                    onChange={(e) =>
                      setDateRange((prev) => ({
                        ...prev,
                        from: e.target.value,
                      }))
                    }
                    disabled={isLoading}
                  />
                  <TextField
                    label="Data do"
                    type="date"
                    size="small"
                    InputLabelProps={{ shrink: true }}
                    onChange={(e) =>
                      setDateRange((prev) => ({ ...prev, to: e.target.value }))
                    }
                    disabled={isLoading}
                  />
                </>
              )}
            </>
          )}
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
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
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
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
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
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
        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
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
            <Paper sx={{ p: 2, height: 400 }}>
              <Typography variant="h6">
                Wyniki produkcyjne (miejsce na wykresy)
              </Typography>
            </Paper>
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

        <Grid size={{ xs: 12, sm: 6, md: 4 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <Paper sx={{ p: 2, height: "100%" }}>
              <Typography variant="h6" mb={2}>
                Kurniki w obsadzie
              </Typography>
              {displayData.chickenHousesStatus.farms.map((farm) => (
                <Typography key={farm.name} variant="body1">
                  {farm.name} - <strong>{farm.henhousesCount}</strong> kurników
                </Typography>
              ))}
              <Divider sx={{ my: 2 }} />
              <Typography variant="h6">
                Łącznie:{" "}
                <strong>
                  {displayData.chickenHousesStatus.totalHenhousesCount}
                </strong>
              </Typography>
            </Paper>
          )}
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 8 }}>
          {isLoading ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <Paper sx={{ p: 2, height: "100%" }}>
              <Typography variant="h6">
                Struktura wydatków (miejsce na wykres)
              </Typography>
            </Paper>
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
