import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
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
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import { mapExpenseProductionOrderTypeToField } from "../../../common/helpers/expenses-productions-order-type-helper";
import { getExpensesProductionsFiltersConfig } from "./filter-config.expenses-production";
import AddExpenseProductionModal from "../../../components/modals/expenses/production/add-expense-production-modal";
import EditExpenseProductionModal from "../../../components/modals/expenses/production/edit-expense-production-modal";
import UploadExpenseInvoicesModal from "../../../components/modals/expenses/production/upload-expense-invoices-modal";
import SaveExpensesInvoicesModal from "../../../components/modals/expenses/production/save-expenses-invoices-modal";

const ExpenseProductionPage: React.FC = () => {
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
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem(
      "columnVisibilityModelExpenseProductions"
    );
    return saved ? JSON.parse(saved) : {};
  });

  const [downloadingFilePath, setDownloadFilePath] = useState<string | null>(
    null
  );

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
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "FakturaKosztu",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania faktury kosztu",
    });
  };

  const columns = useMemo(
    () =>
      getExpenseProductionColumns({
        setSelectedExpenseProduction,
        deleteExpenseProduction,
        setIsEditModalOpen,
        downloadExpenseProductionFile,
        downloadingFilePath,
      }),
    []
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
        <Box display="flex" gap={2}>
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
        </Box>
      </Box>

      <FiltersForm
        config={getExpensesProductionsFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={expenseProductions}
          columns={columns}
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
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
          sortingMode="server"
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(
                ExpensesProductionsOrderType
              ).find(
                (orderType) =>
                  mapExpenseProductionOrderTypeToField(orderType) === sortField
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
