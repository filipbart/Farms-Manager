import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridRowSelectionModel } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import FiltersForm from "../../../components/filters/filters-form";
import {
  ExpensesProductionsOrderType,
  filterReducer,
  initialFilters,
} from "../../../models/expenses/production/expenses-productions-filters";
import type { ExpensesProductionsDictionary } from "../../../models/expenses/production/expenses-productions-dictionary";
import type {
  DraftExpenseInvoice,
  ExpenseProductionListModel,
} from "../../../models/expenses/production/expenses-productions";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { useExpenseProductions } from "../../../hooks/expenses/useExpensesProducations";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { ExpensesService } from "../../../services/expenses-service";
import { downloadFile } from "../../../utils/download-file";
import ApiUrl from "../../../common/ApiUrl";
import { getExpenseProductionColumns } from "./expenses-production-columns";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import LoadingButton from "../../../components/common/loading-button";
import { mapExpenseProductionOrderTypeToField } from "../../../common/helpers/expenses-productions-order-type-helper";
import { getExpensesProductionFiltersConfig } from "./filter-config.expenses-production";
import AddExpenseProductionModal from "../../../components/modals/expenses/production/add-expense-production-modal";
import EditExpenseProductionModal from "../../../components/modals/expenses/production/edit-expense-production-modal";
import UploadExpenseInvoicesModal from "../../../components/modals/expenses/production/upload-expense-invoices-modal";
import SaveExpensesInvoicesModal from "../../../components/modals/expenses/production/save-expenses-invoices-modal";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
} from "@mui/x-data-grid-premium";
import { getSortOptionsFromGridModel } from "../../../utils/grid-state-helper";
import { useAuth } from "../../../auth/useAuth";

const ExpenseProductionPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<ExpensesProductionsDictionary>();
  const [openAddExpenseProductionModal, setOpenAddExpenseProductionModal] =
    useState(false);
  const [selectedExpenseProduction, setSelectedExpenseProduction] =
    useState<ExpenseProductionListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const [openSaveInvoicesModal, setOpenSaveInvoicesModal] = useState(false);
  const [openAddInvoicesModal, setOpenAddInvoicesModal] = useState(false);
  const [draftExpenseInvoices, setDraftExpenseInvoices] = useState<
    DraftExpenseInvoice[]
  >([]);

  const {
    expenseProductions,
    totalRows,
    loading,
    refetch: fetchExpenseProductions,
  } = useExpenseProductions(filters);

  const initialGridState = {
    columns: {
      columnVisibilityModel: { dateCreatedUtc: false },
    },
  };

  const [downloadingFilePath, setDownloadFilePath] = useState<string | null>(
    null
  );

  const [selectedRows, setSelectedRows] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });
  const [isDownloadMode, setIsDownloadMode] = useState(false);
  const [loadingZipFile, setLoadingZipFile] = useState(false);

  const handleCancelDownloadMode = () => {
    setIsDownloadMode(false);
    setSelectedRows({ type: "include", ids: new Set() });
  };

  const hasSelectedRows = () => {
    if (selectedRows.type === "include") {
      return selectedRows.ids.size > 0;
    } else {
      // type === "exclude" means all rows are selected except those in the Set
      return expenseProductions.length > selectedRows.ids.size;
    }
  };

  const uploadFiles = async (draftFiles: DraftExpenseInvoice[]) => {
    if (draftFiles.length === 0) {
      toast.error("Brak plików do przetworzenia");
      return;
    }
    setDraftExpenseInvoices(draftFiles);
    setOpenSaveInvoicesModal(true);
  };

  const uniqueCycles = useMemo(() => {
    if (!dictionary?.cycles) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      map.set(key, cycle);
    }
    return Array.from(map.values());
  }, [dictionary]);

  const deleteExpenseProduction = async (id: string) => {
    try {
      await handleApiResponse(
        () => ExpensesService.deleteExpenseProduction(id),
        async () => {
          toast.success("Wpis kosztów produkcji został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wpisu"
      );
    } catch {
      toast.error("Błąd podczas usuwania wpisu");
    }
  };

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => ExpensesService.getDictionaries(),
          (data) => setDictionary(data.responseData),
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania słowników filtrów");
      }
    };
    fetchDictionaries();
  }, []);

  useEffect(() => {
    fetchExpenseProductions();
  }, [fetchExpenseProductions]);

  const downloadExpenseProductionFile = async (path: string) => {
    const lastDotIndex = path.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < path.length - 1
        ? path.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "FakturaKosztu",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania faktury kosztu",
      fileExtension: fileExtension,
    });
  };

  const downloadMultipleInvoices = async () => {
    const selectedExpenses = expenseProductions.filter((expense) => {
      if (selectedRows.type === "include") {
        return selectedRows.ids.has(expense.id);
      } else {
        // type === "exclude" means all rows are selected except those in the Set
        return !selectedRows.ids.has(expense.id);
      }
    });

    const filePaths = selectedExpenses
      .map((expense) => expense.filePath)
      .filter((path): path is string => path !== undefined && path !== null);

    if (filePaths.length === 0) {
      toast.warning("Wybrane koszty nie mają przypisanych faktur");
      return;
    }

    await downloadFile({
      url: ApiUrl.FilesZip,
      params: { filePaths },
      defaultFilename: "faktury_kosztow",
      setLoading: setLoadingZipFile,
      errorMessage: "Błąd podczas pobierania faktur",
      fileExtension: "zip",
      usePost: true,
    });

    handleCancelDownloadMode();
  };

  const columns = useMemo(
    () =>
      getExpenseProductionColumns({
        setSelectedExpenseProduction,
        deleteExpenseProduction,
        setIsEditModalOpen,
        downloadExpenseProductionFile,
        downloadingFilePath,
        isAdmin,
      }),
    [isAdmin, downloadingFilePath]
  );

  const handleCloseSaveInvoicesModal = () => {
    setDraftExpenseInvoices([]);
    setOpenSaveInvoicesModal(false);
    dispatch({ type: "setMultiple", payload: { page: 0 } });
  };

  const handleSaveInvoicesModal = (expenseInvoiceData: DraftExpenseInvoice) => {
    const filteredInvoices = draftExpenseInvoices.filter(
      (t) => t.draftId !== expenseInvoiceData.draftId
    );

    if (filteredInvoices.length === 0) {
      setDraftExpenseInvoices([]);
      setOpenSaveInvoicesModal(false);
      dispatch({ type: "setMultiple", payload: { page: 0 } });
    }

    setDraftExpenseInvoices(filteredInvoices);
  };

  return (
    <Box p={4}>
      <Box
        mb={2}
        display="flex"
        flexDirection={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "flex-start", sm: "center" }}
        gap={2}
      >
        <Typography variant="h4">Koszty produkcyjne</Typography>
        <Box display="flex" gap={2} alignItems="center">
          {isDownloadMode ? (
            <>
              <LoadingButton
                loading={loadingZipFile}
                variant="contained"
                color="primary"
                onClick={downloadMultipleInvoices}
                disabled={!hasSelectedRows()}
              >
                Pobierz faktury
              </LoadingButton>
              <Button
                variant="outlined"
                color="inherit"
                onClick={handleCancelDownloadMode}
              >
                Anuluj
              </Button>
            </>
          ) : (
            <>
              <Button
                variant="outlined"
                color="primary"
                onClick={() => setIsDownloadMode(true)}
              >
                Pobierz faktury
              </Button>
              <Button
                variant="contained"
                color="primary"
                onClick={() => setOpenAddExpenseProductionModal(true)}
              >
                Dodaj fakturę ręcznie
              </Button>
              <Button
                variant="contained"
                color="primary"
                onClick={() => setOpenAddInvoicesModal(true)}
              >
                Dodaj faktury automatycznie
              </Button>
            </>
          )}
        </Box>
      </Box>

      <FiltersForm
        config={getExpensesProductionFiltersConfig(
          dictionary,
          uniqueCycles,
          filters,
          isAdmin
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={expenseProductions}
          columns={columns}
          initialState={initialGridState}
          scrollbarSize={17}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          checkboxSelection={isDownloadMode}
          disableRowSelectionOnClick
          onRowSelectionModelChange={(newSelection) => {
            setSelectedRows(newSelection);
          }}
          rowSelectionModel={selectedRows}
          rowSelection={isDownloadMode}
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
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              ExpensesProductionsOrderType,
              mapExpenseProductionOrderTypeToField
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
        />
      </Box>

      {draftExpenseInvoices.length > 0 && (
        <SaveExpensesInvoicesModal
          open={openSaveInvoicesModal}
          onClose={handleCloseSaveInvoicesModal}
          onSave={handleSaveInvoicesModal}
          draftExpenseInvoices={draftExpenseInvoices}
        />
      )}

      <UploadExpenseInvoicesModal
        open={openAddInvoicesModal}
        onClose={() => setOpenAddInvoicesModal(false)}
        onUpload={uploadFiles}
      />

      <EditExpenseProductionModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedExpenseProduction(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedExpenseProduction(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        expenseProductionToEdit={selectedExpenseProduction}
      />

      <AddExpenseProductionModal
        open={openAddExpenseProductionModal}
        onClose={() => setOpenAddExpenseProductionModal(false)}
        onSave={() => {
          setOpenAddExpenseProductionModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default ExpenseProductionPage;
