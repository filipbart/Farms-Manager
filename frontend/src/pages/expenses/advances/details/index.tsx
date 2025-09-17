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
import { useNavigate, useParams } from "react-router-dom";
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
import type { ExpenseAdvanceListModel } from "../../../../models/expenses/advances/expenses-advances";
import AddExpenseAdvanceModal from "../../../../components/modals/expenses/advances/add-expense-advance-modal";
import { toast } from "react-toastify";
import { ExpensesAdvancesService } from "../../../../services/expenses-advances-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import EditExpenseAdvanceModal from "../../../../components/modals/expenses/advances/edit-expense-advance-modal";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../../../utils/grid-state-helper";

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
  const nav = useNavigate();

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem(
      "expensesProductionAdvancesGridState"
    );
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const [selectedMonths, setSelectedMonths] = useState<number[]>([
    new Date().getMonth() + 1,
  ]);
  const [selectedYears, setSelectedYears] = useState<number[]>([
    new Date().getFullYear(),
  ]);

  const downloadExpenseAdvanceFile = async (path: string) => {
    const lastDotIndex = path.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < path.length - 1
        ? path.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "PlikEwidencji",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania pliku",
      fileExtension: fileExtension,
    });
  };

  const [filters, dispatch] = useReducer(
    filterReducer,
    {
      ...initialFilters,
      years: [new Date().getFullYear()],
      months: [new Date().getMonth() + 1],
    },
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "expensesProductionAdvancesGridState",
        "expenseAdvancesPageSize",
        ExpensesAdvancesOrderType,
        mapExpensesAdvancesOrderTypeToField
      )
  );

  const { response, loading, fetchExpenseAdvances } = useExpenseAdvances(
    employeeId,
    filters
  );
  const {
    employeeFullName,
    list,
    totalBalance,
    balance,
    totalIncome,
    totalExpenses,
  } = response || {};
  const { items: advances = [], totalRows = 0 } = list || {};

  useEffect(() => {
    dispatch({
      type: "setMultiple",
      payload: {
        years: selectedYears,
        months: selectedMonths,
        page: 0,
      },
    });
  }, [selectedMonths, selectedYears]);

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
            value={selectedMonths}
            onChange={(e) => {
              const value = e.target.value;
              setSelectedMonths(
                typeof value === "string" ? value.split(",").map(Number) : value
              );
            }}
            fullWidth
            slotProps={{
              select: {
                multiple: true,
                renderValue: (selected: any) =>
                  monthOptions
                    .filter((opt) => (selected as number[]).includes(opt.value))
                    .map((opt) => opt.label)
                    .join(", "),
              },
            }}
          >
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
            value={selectedYears}
            onChange={(e) => {
              const value = e.target.value;
              setSelectedYears(
                typeof value === "string" ? value.split(",").map(Number) : value
              );
            }}
            fullWidth
            slotProps={{
              select: {
                multiple: true,
                renderValue: (selected: any) =>
                  (selected as number[]).join(", "),
              },
            }}
          >
            {generateYearOptions().map((year) => (
              <MenuItem key={year} value={year}>
                {year}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid
          gap={2}
          size={{ xs: 12, sm: 6 }}
          display="flex"
          justifyContent="flex-end"
        >
          <Button
            variant="outlined"
            color="inherit"
            onClick={() => nav("/expenses/advances")}
          >
            Cofnij do listy
          </Button>
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
          <Grid size={{ xs: 12, md: 3 }} textAlign="center">
            <Typography variant="h6">Stan konta</Typography>
            <Typography
              variant="h5"
              color={(totalBalance ?? 0) >= 0 ? "text.primary" : "error.main"}
            >
              {formatCurrencyPLN(totalBalance)}
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 3 }} textAlign="center">
            <Typography variant="h6">Saldo dla wybranego okresu</Typography>
            <Typography
              variant="h5"
              color={(balance ?? 0) >= 0 ? "text.primary" : "error.main"}
            >
              {formatCurrencyPLN(balance)}
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 3 }} textAlign="center">
            <Typography variant="h6">Zaliczki</Typography>
            <Typography variant="h5" color="success.main">
              {formatCurrencyPLN(totalIncome)}
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, md: 3 }} textAlign="center">
            <Typography variant="h6">Wydatki</Typography>
            <Typography variant="h5" color="error.main">
              {formatCurrencyPLN(totalExpenses)}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      <Box mt={3} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={advances}
          columns={columns}
          rowCount={totalRows}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
            };
            localStorage.setItem(
              "expensesProductionAdvancesGridState",
              JSON.stringify(stateToSave)
            );
          }}
          scrollbarSize={17}
          pagination
          paginationMode="server"
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem(
              "expenseAdvancesPageSize",
              pageSize.toString()
            );

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              ExpensesAdvancesOrderType,
              mapExpensesAdvancesOrderTypeToField
            );
            const payload =
              model.length > 0
                ? { ...sortOptions, page: 0 }
                : { ...sortOptions };

            dispatch({
              type: "setMultiple",
              payload,
            });
          }}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ noRowsOverlay: NoRowsOverlay }}
          showToolbar
          getRowClassName={(params) => {
            if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
              return "aggregated-row";
            }
            return "";
          }}
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
            "& .aggregated-row": {
              fontWeight: "bold",
              "& .MuiDataGrid-cell": {
                borderTop: "1px solid rgba(224, 224, 224, 1)",
                backgroundColor: "rgba(240, 240, 240, 0.7)",
              },
            },
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
