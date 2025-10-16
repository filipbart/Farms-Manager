import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridRowSelectionModel } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect, useContext } from "react";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { downloadFile } from "../../../utils/download-file";
import ApiUrl from "../../../common/ApiUrl";
import FiltersForm from "../../../components/filters/filters-form";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import {
  filterReducer,
  initialFilters,
  mapSalesInvoiceOrderTypeToField,
  SalesInvoicesOrderType,
} from "../../../models/sales/sales-invoices-filters";
import type {
  DraftSalesInvoice,
  SalesInvoiceListModel,
} from "../../../models/sales/sales-invoices";
import { SalesService } from "../../../services/sales-service";
import { getSalesInvoicesColumns } from "./sales-invoices-columns";
import { getSalesInvoicesFiltersConfig } from "./filter-config.sales-invoices";
import { useSalesInvoices } from "../../../hooks/sales/useSalesInvoices";
import type { SalesDictionary } from "../../../models/sales/sales-dictionary";
import UploadSalesInvoicesModal from "../../../components/modals/sales/invoices/upload-sales-invoices-modal";
import SaveSalesInvoicesModal from "../../../components/modals/sales/invoices/save-sales-invoices-modal";
import EditSaleInvoiceModal from "../../../components/modals/sales/invoices/edit-sale-invoice-modal";
import BookPaymentModal from "../../../components/modals/sales/invoices/book-payment-modal";
import { NotificationContext } from "../../../context/notification-context";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../../utils/grid-state-helper";
import { getPriorityClassName } from "../../../utils/priority-helper";

const SalesInvoicesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "saleInvoicesGridState",
        "salesInvoicesPageSize",
        SalesInvoicesOrderType,
        mapSalesInvoiceOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<SalesDictionary>();
  const { fetchNotifications } = useContext(NotificationContext);
  const [selectedSalesInvoice, setSelectedSalesInvoice] =
    useState<SalesInvoiceListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [openSaveInvoicesModal, setOpenSaveInvoicesModal] = useState(false);
  const [openUploadInvoicesModal, setOpenUploadInvoicesModal] = useState(false);
  const [draftSalesInvoices, setDraftSalesInvoices] = useState<
    DraftSalesInvoice[]
  >([]);

  const [selectedRows, setSelectedRows] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });
  const [isPaymentModalOpen, setIsPaymentModalOpen] = useState(false);
  const [isBookingPayment, setIsBookingPayment] = useState(false);

  const { salesInvoices, totalRows, loading, fetchSalesInvoices } =
    useSalesInvoices(filters);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("saleInvoicesGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const [downloadingFilePath, setDownloadFilePath] = useState<string | null>(
    null
  );

  const uploadFiles = (draftFiles: DraftSalesInvoice[]) => {
    if (draftFiles.length === 0) {
      toast.error("Brak plików do przetworzenia");
      return;
    }
    setDraftSalesInvoices(draftFiles);
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

  const deleteSalesInvoice = async (id: string) => {
    try {
      await handleApiResponse(
        () => SalesService.deleteSaleInvoice(id),
        async () => {
          toast.success("Faktura sprzedaży została poprawnie usunięta");
          await fetchSalesInvoices();
        },
        undefined,
        "Błąd podczas usuwania faktury"
      );
    } catch {
      toast.error("Błąd podczas usuwania faktury");
    }
  };

  const handleBookPayment = async (paymentDate: string) => {
    try {
      setIsBookingPayment(true);
      await handleApiResponse(
        () =>
          SalesService.bookInvoicesPayment({
            invoicesIds: Array.from(selectedRows.ids).map(String),
            paymentDate,
          }),
        async () => {
          toast.success("Płatność została pomyślnie zaksięgowana");
          setIsPaymentModalOpen(false);
          setSelectedRows({ type: "include", ids: new Set() });
          await fetchSalesInvoices();
          fetchNotifications();
        },
        undefined,
        "Błąd podczas księgowania płatności"
      );
    } catch {
      toast.error("Błąd podczas księgowania płatności");
    } finally {
      setIsBookingPayment(false);
    }
  };

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => SalesService.getDictionaries(),
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
    fetchSalesInvoices();
  }, [fetchSalesInvoices]);

  const downloadSalesInvoiceFile = async (path: string) => {
    const lastDotIndex = path.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < path.length - 1
        ? path.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "FakturaSprzedazy",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania faktury sprzedaży",
      fileExtension: fileExtension,
    });
  };

  const columns = useMemo(
    () =>
      getSalesInvoicesColumns({
        setSelectedSalesInvoice,
        deleteSalesInvoice,
        setIsEditModalOpen,
        downloadSalesInvoiceFile,
        downloadingFilePath,
      }),
    [downloadingFilePath]
  );

  const handleCloseSaveInvoicesModal = () => {
    setDraftSalesInvoices([]);
    setOpenSaveInvoicesModal(false);
    dispatch({ type: "setMultiple", payload: { page: 0 } });
  };

  const handleSaveInvoicesModal = (salesInvoiceData: DraftSalesInvoice) => {
    const filteredInvoices = draftSalesInvoices.filter(
      (t) => t.draftId !== salesInvoiceData.draftId
    );
    if (filteredInvoices.length === 0) {
      handleCloseSaveInvoicesModal();
    }
    setDraftSalesInvoices(filteredInvoices);
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
        <Typography variant="h4">Faktury Sprzedażowe</Typography>
        <Box display="flex" gap={2} alignItems="center">
          {selectedRows.ids.size > 0 && (
            <Button
              variant="outlined"
              color="primary"
              onClick={() => setIsPaymentModalOpen(true)}
            >
              Zaksięguj płatność
            </Button>
          )}

          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenUploadInvoicesModal(true)}
          >
            Dodaj faktury
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getSalesInvoicesFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={salesInvoices}
          columns={columns}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
              rowGrouping: newState.rowGrouping,
            };
            localStorage.setItem(
              "saleInvoicesGridState",
              JSON.stringify(stateToSave)
            );
          }}
          localeText={{
            footerRowSelected: (count) => {
              if (count === 1) return "1 wiersz zaznaczony";
              if (count >= 2 && count <= 4)
                return `${count} wiersze zaznaczone`;
              return `${count} wierszy zaznaczonych`;
            },
            checkboxSelectionHeaderName: "Zaznaczanie wierszy",
            paginationRowsPerPage: "Wierszy na stronę:",
            paginationDisplayedRows: ({ from, to, count }) =>
              `${from} do ${to} z ${count}`,
          }}
          pagination
          paginationMode="server"
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem("salesInvoicesPageSize", pageSize.toString());

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          checkboxSelection
          disableRowSelectionOnClick
          onRowSelectionModelChange={(newSelection) => {
            setSelectedRows(newSelection);
          }}
          rowSelectionModel={selectedRows}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ noRowsOverlay: NoRowsOverlay }}
          showToolbar
          getRowClassName={(params) => {
            if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
              return "aggregated-row";
            }
            return getPriorityClassName(params.row.priority);
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
            "& .payment-overdue": {
              backgroundColor: "rgba(244, 67, 54, 0.1)",
              "&:hover": {
                backgroundColor: "rgba(244, 67, 54, 0.15)",
              },
            },
            "& .payment-due-soon": {
              backgroundColor: "rgba(255, 193, 7, 0.15)",
              "&:hover": {
                backgroundColor: "rgba(255, 193, 7, 0.2)",
              },
            },
            "& .payment-due-warning": {
              backgroundColor: "rgba(33, 150, 243, 0.1)",
              "&:hover": {
                backgroundColor: "rgba(33, 150, 243, 0.15)",
              },
            },
          }}
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              SalesInvoicesOrderType,
              mapSalesInvoiceOrderTypeToField
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

      {draftSalesInvoices.length > 0 && (
        <SaveSalesInvoicesModal
          open={openSaveInvoicesModal}
          onClose={handleCloseSaveInvoicesModal}
          onSave={handleSaveInvoicesModal}
          draftSalesInvoices={draftSalesInvoices}
        />
      )}

      <UploadSalesInvoicesModal
        open={openUploadInvoicesModal}
        onClose={() => setOpenUploadInvoicesModal(false)}
        onUpload={uploadFiles}
      />

      <EditSaleInvoiceModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedSalesInvoice(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedSalesInvoice(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        salesInvoice={selectedSalesInvoice}
      />

      <BookPaymentModal
        open={isPaymentModalOpen}
        loading={isBookingPayment}
        onClose={() => setIsPaymentModalOpen(false)}
        onBookPayment={handleBookPayment}
      />
    </Box>
  );
};

export default SalesInvoicesPage;
