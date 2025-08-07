import {
  Box,
  Typography,
  CircularProgress,
  Button,
  Grid,
  TextField,
  MenuItem,
  tablePaginationClasses,
  Paper,
} from "@mui/material";
import { useEffect, useReducer, useState, useMemo } from "react";
import { useParams } from "react-router-dom";
import dayjs from "dayjs";
import { useExpenseAdvances } from "../../../../hooks/expenses/advances/useExpensesAdvances";
import {
  ExpensesAdvancesOrderType,
  filterReducer,
  initialFilters,
  mapExpensesAdvancesOrderTypeToField,
} from "../../../../models/expenses/advances/expenses-advances-filters";
import { getAdvancesColumns } from "./expenses-advances-columns";
import { downloadFile } from "../../../../utils/download-file";
import ApiUrl from "../../../../common/ApiUrl";
import NoRowsOverlay from "../../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../../components/datagrid/custom-toolbar";
import type { ExpenseAdvanceListModel } from "../../../../models/expenses/advances/expenses-advances";
import { DataGridPro } from "@mui/x-data-grid-pro";
import AddExpenseAdvanceModal from "../../../../components/modals/expenses/advances/add-expense-advance-modal";
import { toast } from "react-toastify";
import { ExpensesAdvancesService } from "../../../../services/expenses-advances-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import EditExpenseAdvanceModal from "../../../../components/modals/expenses/advances/edit-expense-advance-modal";

const generateYearOptions = () => {
  const currentYear = new Date().getFullYear();
  const years = [];
  for (let i = 0; i < 10; i++) years.push(currentYear - i);
  return years;
};
const monthOptions = [
  { value: 1, label: "Styczeń" },
  { value: 2, label: "Luty" },
  { value: 3, label: "Marzec" },
  { value: 4, label: "Kwiecień" },
  { value: 5, label: "Maj" },
  { value: 6, label: "Czerwiec" },
  { value: 7, label: "Lipiec" },
  { value: 8, label: "Sierpień" },
  { value: 9, label: "Wrzesień" },
  { value: 10, label: "Październik" },
  { value: 11, label: "Listopad" },
  { value: 12, label: "Grudzień" },
];

const formatCurrencyPLN = (value: number | null | undefined): string => {
  const numberToFormat = value ?? 0;

  const formatter = new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: "PLN",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  return formatter.format(numberToFormat);
};

const ExpenseAdvanceDetailsPage: React.FC = () => {
  const { employeeId } = useParams<{ employeeId: string }>();
  const [openAddModal, setOpenAddModal] = useState(false);
  const [openEditModal, setOpenEditModal] = useState(false);
  const [selectedAdvance, setSelectedAdvance] =
    useState<ExpenseAdvanceListModel | null>(null);
  const [downloadingFilePath, setDownloadFilePath] = useState<string | null>(
    null
  );

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelExpensesAdvances");
    return saved ? JSON.parse(saved) : {};
  });

  const [selectedMonth, setSelectedMonth] = useState<number | "">(
    new Date().getMonth() + 1
  );
  const [selectedYear, setSelectedYear] = useState<number | "">(
    new Date().getFullYear()
  );

  const downloadExpenseAdvanceFile = async (path: string) => {
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "PlikEwidencji",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania pliku",
    });
  };

  const [filters, dispatch] = useReducer(filterReducer, {
    ...initialFilters,
    dateSince: dayjs().startOf("month").format("YYYY-MM-DD"),
    dateTo: dayjs().endOf("month").format("YYYY-MM-DD"),
  });

  const { response, loading, fetchExpenseAdvances } = useExpenseAdvances(
    employeeId,
    filters
  );
  const { employeeFullName, list, balance, totalIncome, totalExpenses } =
    response || {};
  const { items: advances = [], totalRows = 0 } = list || {};

  useEffect(() => {
    let dateSince: string | undefined = undefined;
    let dateTo: string | undefined = undefined;
    const currentYear = new Date().getFullYear();

    if (selectedYear && selectedMonth) {
      const date = dayjs()
        .year(selectedYear)
        .month(selectedMonth - 1);
      dateSince = date.startOf("month").format("YYYY-MM-DD");
      dateTo = date.endOf("month").format("YYYY-MM-DD");
    } else if (selectedYear && !selectedMonth) {
      const date = dayjs().year(selectedYear);
      dateSince = date.startOf("year").format("YYYY-MM-DD");
      dateTo = date.endOf("year").format("YYYY-MM-DD");
    } else if (!selectedYear && selectedMonth) {
      const date = dayjs()
        .year(currentYear)
        .month(selectedMonth - 1);
      dateSince = date.startOf("month").format("YYYY-MM-DD");
      dateTo = date.endOf("month").format("YYYY-MM-DD");
    }
    //

    dispatch({ type: "setMultiple", payload: { dateSince, dateTo } });
  }, [selectedMonth, selectedYear]);

  const deleteAdvance = async (id: string) => {
    await handleApiResponse(
      () => ExpensesAdvancesService.deleteExpenseAdvance(id),
      () => {
        toast.success("Ewidencja została usunięta");
        fetchExpenseAdvances();
      },
      undefined,
      "Błąd podczas usuwania ewidencji"
    );
  };

  const columns = useMemo(
    () =>
      getAdvancesColumns({
        setSelectedAdvance,
        deleteAdvance,
        setIsEditModalOpen: setOpenEditModal,
        downloadAdvanceFile: downloadExpenseAdvanceFile,
        downloadingFilePath,
      }),
    [downloadingFilePath]
  );

  useEffect(() => {
    if (employeeId) {
      fetchExpenseAdvances();
    }
  }, [employeeId, filters]);

  if (loading && !employeeFullName) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  if (!employeeFullName) {
    return <Typography p={5}>Nie znaleziono pracownika.</Typography>;
  }

  return (
    <Box p={4}>
      <Box
        mb={2}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
      >
        <Typography variant="h4">
          Ewidencja zaliczek: {employeeFullName}
        </Typography>
      </Box>

      <Grid container spacing={2} sx={{ mb: 3, alignItems: "center" }}>
        <Grid size={{ xs: 12, sm: 3 }}>
          <TextField
            select
            label="Miesiąc"
            value={selectedMonth}
            onChange={(e) => setSelectedMonth(Number(e.target.value))}
            fullWidth
          >
            <MenuItem value="">Wszystkie</MenuItem>
            {monthOptions.map((option) => (
              <MenuItem key={option.value} value={option.value}>
                {option.label}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid size={{ xs: 12, sm: 3 }}>
          <TextField
            select
            label="Rok"
            value={selectedYear}
            onChange={(e) => setSelectedYear(Number(e.target.value))}
            fullWidth
          >
            <MenuItem value="">Wszystkie</MenuItem>
            {generateYearOptions().map((year) => (
              <MenuItem key={year} value={year}>
                {year}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }} display="flex" justifyContent="flex-end">
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddModal(true)}
          >
            Dodaj ewidencję
          </Button>
        </Grid>
      </Grid>

      <Paper sx={{ p: 2, mb: 3 }} variant="outlined">
        <Grid container spacing={2} justifyContent="space-around">
          <Grid size={{ xs: 12, md: 4 }} textAlign="center">
            <Typography variant="h6">Saldo</Typography>
            <Typography
              variant="h5"
              color={(balance ?? 0) >= 0 ? "text.primary" : "error.main"}
            >
              {formatCurrencyPLN(balance)}
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }} textAlign="center">
            <Typography variant="h6">Zaliczki</Typography>
            <Typography variant="h5" color="success.main">
              {formatCurrencyPLN(totalIncome)}
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }} textAlign="center">
            <Typography variant="h6">Wydatki</Typography>
            <Typography variant="h5" color="error.main">
              {formatCurrencyPLN(totalExpenses)}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      <Box mt={3} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          rows={advances}
          columns={columns}
          rowCount={totalRows}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelExpenseProductions",
              JSON.stringify(model)
            );
          }}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
            },
          }}
          localeText={{
            paginationRowsPerPage: "Wierszy na stronę:",
            paginationDisplayedRows: ({ from, to, count }) =>
              `${from} do ${to} z ${count}`,
          }}
          scrollbarSize={17}
          pagination
          paginationMode="server"
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) =>
            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            })
          }
          sortingMode="server"
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(
                ExpensesAdvancesOrderType
              ).find(
                (orderType) =>
                  mapExpensesAdvancesOrderTypeToField(orderType) === sortField
              );
              dispatch({
                type: "setMultiple",
                payload: {
                  orderBy: foundOrderBy,
                  isDescending: model[0].sort === "desc",
                  page: 0,
                },
              });
            } else {
              dispatch({
                type: "setMultiple",
                payload: { orderBy: undefined, isDescending: undefined },
              });
            }
          }}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
        />
      </Box>

      <AddExpenseAdvanceModal
        open={openAddModal}
        onClose={() => setOpenAddModal(false)}
        onSave={() => {
          setOpenAddModal(false);
          fetchExpenseAdvances();
        }}
        employeeId={employeeId!}
      />
      <EditExpenseAdvanceModal
        open={openEditModal}
        onClose={() => {
          setOpenEditModal(false);
          setSelectedAdvance(null);
        }}
        onSave={() => {
          setOpenEditModal(false);
          setSelectedAdvance(null);
          fetchExpenseAdvances();
        }}
        advance={selectedAdvance}
      />
    </Box>
  );
};

export default ExpenseAdvanceDetailsPage;
