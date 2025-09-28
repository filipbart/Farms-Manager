import { useEffect, useMemo, useReducer, useState } from "react";
import {
  filterReducer,
  initialFilters,
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
import { ExpensesPieChart } from "../../components/dashboard/expenses-pie-chart";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import type { Dayjs } from "dayjs";
import { ProductionResultsChart } from "../../components/dashboard/production-results-charts";
import dayjs from "dayjs";
import type {
  DashboardChickenHousesStatus,
  DashboardDictionary,
  DashboardEwwChart,
  DashboardExpensesPieChart,
  DashboardFcrChart,
  DashboardFilters,
  DashboardFlockLossChart,
  DashboardGasConsumptionChart,
  DashboardNotificationsResponse,
  DashboardStats,
} from "../../models/dashboard/dashboard";

const DashboardPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<DashboardDictionary>();

  // Data states
  const [stats, setStats] = useState<DashboardStats>();
  const [chickenHousesStatus, setChickenHousesStatus] =
    useState<DashboardChickenHousesStatus>();
  const [notifications, setNotifications] =
    useState<DashboardNotificationsResponse>();
  const [ewwChart, setEwwChart] = useState<DashboardEwwChart>();
  const [fcrChart, setFcrChart] = useState<DashboardFcrChart>();
  const [flockLossChart, setFlockLossChart] =
    useState<DashboardFlockLossChart>();
  const [expensesPieChart, setExpensesPieChart] =
    useState<DashboardExpensesPieChart>();
  const [gasConsumptionChart, setGasConsumptionChart] =
    useState<DashboardGasConsumptionChart>();

  // Loading states
  const [isLoadingDictionary, setIsLoadingDictionary] = useState(true);
  const [isLoadingStats, setIsLoadingStats] = useState(true);
  const [isLoadingNotifications, setIsLoadingNotifications] = useState(true);
  const [isLoadingCharts, setIsLoadingCharts] = useState(true);
  const [isLoadingExpenses, setIsLoadingExpenses] = useState(true);

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
      setIsLoadingDictionary(true);
      try {
        await handleApiResponse(
          () => DashboardService.getDictionaries(),
          (data) => setDictionary(data.responseData)
        );
      } catch (error) {
        toast.error(`Wystąpił błąd podczas pobierania słowników: ${error}`);
      } finally {
        setIsLoadingDictionary(false);
      }
    };
    fetchDictionary();
  }, []);

  const uniqueCycles = useMemo(() => {
    if (!dictionary) return [];
    const map = new Map<string, CycleDictModel>();
    (dictionary.cycles as CycleDictModel[]).forEach((cycle) => {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) map.set(key, cycle);
    });
    return Array.from(map.values());
  }, [dictionary]);

  useEffect(() => {
    const payload: Partial<DashboardFilters> = {
      dateCategory: dateCategory,
    };

    if (dateCategory === "cycle") {
      payload.cycle = selectedCycle;
      payload.dateSince = undefined;
      payload.dateTo = undefined;
    } else {
      let dateSince: string | undefined = undefined;
      let dateTo: string | undefined = undefined;

      if (dateCategory === "month") {
        const from = dayjs()
          .year(selectedYear)
          .month(selectedMonth)
          .startOf("month");
        const to = dayjs()
          .year(selectedYear)
          .month(selectedMonth)
          .endOf("month");
        dateSince = from.format("YYYY-MM-DD");
        dateTo = to.format("YYYY-MM-DD");
      } else if (dateCategory === "year") {
        const from = new Date(selectedYear, 0, 1);
        const to = new Date(selectedYear, 11, 31);
        dateSince = from.toISOString().split("T")[0];
        dateTo = to.toISOString().split("T")[0];
      } else if (dateCategory === "range") {
        dateSince = dateRange.from?.format("YYYY-MM-DD") ?? undefined;
        dateTo = dateRange.to?.format("YYYY-MM-DD") ?? undefined;
      }

      payload.cycle = undefined;
      payload.dateSince = dateSince;
      payload.dateTo = dateTo;
    }

    dispatch({ type: "setMultiple", payload });
  }, [dateCategory, selectedMonth, selectedYear, dateRange, selectedCycle]);

  useEffect(() => {
    if (!filters || Object.keys(filters).length === 0) return;

    const fetchStats = async () => {
      setIsLoadingStats(true);
      try {
        await handleApiResponse(
          () => DashboardService.getStats(filters),
          (apiData) => {
            setStats(apiData.responseData?.stats);
            setChickenHousesStatus(apiData.responseData?.chickenHousesStatus);
          }
        );
      } catch (error) {
        toast.error(`Wystąpił błąd podczas ładowania statystyk: ${error}`);
      } finally {
        setIsLoadingStats(false);
      }
    };

    const fetchNotifications = async () => {
      setIsLoadingNotifications(true);
      try {
        await handleApiResponse(
          () => DashboardService.getNotifications(filters),
          (apiData) => {
            setNotifications(apiData.responseData ?? []);
          }
        );
      } catch (error) {
        toast.error(`Wystąpił błąd podczas ładowania powiadomień: ${error}`);
      } finally {
        setIsLoadingNotifications(false);
      }
    };

    const fetchCharts = async () => {
      setIsLoadingCharts(true);
      try {
        await Promise.all([
          handleApiResponse(
            () => DashboardService.getEwwChart(filters),
            (data) => setEwwChart(data.responseData)
          ),
          handleApiResponse(
            () => DashboardService.getFcrChart(filters),
            (data) => setFcrChart(data.responseData)
          ),
          handleApiResponse(
            () => DashboardService.getFlockLossChart(filters),
            (data) => setFlockLossChart(data.responseData)
          ),
          handleApiResponse(
            () => DashboardService.getGasConsumptionChart(filters),
            (data) => setGasConsumptionChart(data.responseData)
          ),
        ]);
      } catch (error) {
        toast.error(`Wystąpił błąd podczas ładowania wykresów: ${error}`);
      } finally {
        setIsLoadingCharts(false);
      }
    };

    const fetchExpenses = async () => {
      setIsLoadingExpenses(true);
      try {
        await handleApiResponse(
          () => DashboardService.getExpensesPieChart(filters),
          (apiData) => {
            setExpensesPieChart(apiData.responseData);
          }
        );
      } catch (error) {
        toast.error(
          `Wystąpił błąd podczas ładowania struktury wydatków: ${error}`
        );
      } finally {
        setIsLoadingExpenses(false);
      }
    };

    fetchStats();
    fetchNotifications();
    fetchCharts();
    fetchExpenses();
  }, [filters]);

  const isAnythingLoading =
    isLoadingDictionary ||
    isLoadingStats ||
    isLoadingNotifications ||
    isLoadingCharts ||
    isLoadingExpenses;

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

  if (isLoadingDictionary) {
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
          <FormControl sx={{ minWidth: 200 }} size="small" disabled={isAnythingLoading}>
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
          <FormControl sx={{ minWidth: 120 }} size="small" disabled={isAnythingLoading}>
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
              disabled={isAnythingLoading}
            >
              <InputLabel>Wybierz Cykl</InputLabel>
              <Select
                label="Wybierz Cykl"
                value={selectedCycle}
                onChange={(e) => setSelectedCycle(e.target.value as string)}
              >
                {uniqueCycles.map((cycle) => (
                  <MenuItem
                    key={`${cycle.identifier}-${cycle.year}`}
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
              disabled={isAnythingLoading}
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
              disabled={isAnythingLoading}
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
                disabled={isAnythingLoading}
                slotProps={{ textField: { size: "small" } }}
              />
              <DatePicker
                label="Data do"
                value={dateRange.to}
                onChange={(newValue) =>
                  setDateRange((prev) => ({ ...prev, to: newValue }))
                }
                disabled={isAnythingLoading}
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
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Przychody"
                  value={
                    stats?.revenue?.toLocaleString("pl-PL", {
                      style: "currency",
                      currency: "PLN",
                    }) ?? "-"
                  }
                  icon={<MdTrendingUp />}
                  color="success"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Wydatki"
                  value={
                    stats?.expenses?.toLocaleString("pl-PL", {
                      style: "currency",
                      currency: "PLN",
                    }) ?? "-"
                  }
                  icon={<MdTrendingDown />}
                  color="error"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 12, md: 4 }}>
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={120} />
              ) : (
                <StatCard
                  title="Dochód"
                  value={
                    stats?.income?.toLocaleString("pl-PL", {
                      style: "currency",
                      currency: "PLN",
                    }) ?? "-"
                  }
                  icon={<MdAccountBalanceWallet />}
                  color="primary"
                />
              )}
            </Grid>
          </Grid>
        </Grid>
        <Grid size={{ xs: 12, sm: 12, lg: 4 }}>
          {isLoadingStats ? (
            <Skeleton variant="rounded" height={120} />
          ) : (
            <StatCard
              title="VAT z wydatków"
              value={
                stats?.vatFromExpenses?.toLocaleString("pl-PL", {
                  style: "currency",
                  currency: "PLN",
                }) ?? "-"
              }
              icon={<FaPercentage />}
              color="warning"
            />
          )}
        </Grid>

        <Grid size={{ xs: 12, lg: 8 }}>
          {isLoadingCharts ? (
            <Skeleton variant="rounded" height={400} />
          ) : (
            <ProductionResultsChart
              fcrData={fcrChart}
              ewwData={ewwChart}
              gasData={gasConsumptionChart}
              lossData={flockLossChart}
            />
          )}
        </Grid>
        <Grid size={{ xs: 12, lg: 4 }}>
          <Grid container spacing={{ xs: 3, lg: 2 }}>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z kg żywca"
                  value={
                    !stats?.incomePerKg
                      ? "-"
                      : `${stats?.incomePerKg?.toLocaleString("pl-PL", {
                          minimumFractionDigits: 2,
                          maximumFractionDigits: 2,
                        })} zł`
                  }
                  icon={<FaPiggyBank />}
                  color="success"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Dochód z m²"
                  value={
                    !stats?.incomePerSqm
                      ? "-"
                      : `${stats?.incomePerSqm?.toLocaleString("pl-PL", {
                          minimumFractionDigits: 2,
                          maximumFractionDigits: 2,
                        })} zł`
                  }
                  icon={<FaChartArea />}
                  color="primary"
                />
              )}
            </Grid>
            <Grid size={{ xs: 12, sm: 4, lg: 12 }}>
              {isLoadingStats ? (
                <Skeleton variant="rounded" height={125} />
              ) : (
                <StatCard
                  title="Średnia cena paszy"
                  value={
                    !stats?.avgFeedPrice
                      ? "-"
                      : `${stats?.avgFeedPrice?.toLocaleString("pl-PL")} zł/t`
                  }
                  icon={<MdOutlineShoppingCart />}
                  color="info"
                />
              )}
            </Grid>
          </Grid>
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 6, lg: 8 }}>
          {isLoadingStats ? (
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
                {chickenHousesStatus?.farms.map((farm) => {
                  const activeHenhouses = farm.henhouses.filter(
                    (h) => h.chickenCount > 0
                  );
                  if (activeHenhouses.length === 0) return;
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
                  {chickenHousesStatus?.totalChickenCount.toLocaleString(
                    "pl-PL"
                  )}
                </strong>
              </Typography>
            </Paper>
          )}
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 6, lg: 4 }}>
          {isLoadingExpenses ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <ExpensesPieChart data={expensesPieChart?.data} />
          )}
        </Grid>

        <Grid size={{ xs: 12 }} sx={{ height: 380 }}>
          {isLoadingNotifications ? (
            <Skeleton variant="rounded" height="100%" />
          ) : (
            <DashboardNotifications notifications={notifications} />
          )}
        </Grid>
      </Grid>
    </Box>
  );
};

export default DashboardPage;