import {
  Box,
  Button,
  Tab,
  Tabs,
  tablePaginationClasses,
  Typography,
} from "@mui/material";
import { MdSend } from "react-icons/md";
import React, {
  useCallback,
  useEffect,
  useMemo,
  useReducer,
  useState,
} from "react";
import { toast } from "react-toastify";
import {
  DataGridPremium,
  type GridState,
  type GridRowSelectionModel,
} from "@mui/x-data-grid-premium";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import { MdAdd, MdSync, MdDeleteForever } from "react-icons/md";
import { AccountingService } from "../../services/accounting-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { downloadFile } from "../../utils/download-file";
import ApiUrl from "../../common/ApiUrl";
import {
  ksefFiltersReducer,
  initialKSeFFilters,
  type KSeFInvoicesFilters,
  KSeFInvoicesOrderType,
  mapKSeFOrderTypeToField,
} from "../../models/accounting/ksef-filters";
import {
  KSeFInvoiceType,
  type KSeFInvoiceListModel,
} from "../../models/accounting/ksef-invoice";
import { getKSeFInvoicesColumns } from "./ksef-invoices-columns";
import InvoiceDetailsModal from "../../components/modals/accounting/invoice-details-modal";
import UploadInvoiceModal from "../../components/modals/accounting/upload-invoice-modal";
import SaveAccountingInvoiceModal from "../../components/modals/accounting/save-accounting-invoice-modal";
import type { DraftAccountingInvoice } from "../../services/accounting-service";
import FiltersForm from "../../components/filters/filters-form";
import { getAccountingFiltersConfig } from "./filter-config.accounting";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../utils/grid-state-helper";
import { UsersService } from "../../services/users-service";
import type { UserListModel } from "../../models/users/users";

interface TabPanelProps {
  children: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, index, value }) => (
  <div
    role="tabpanel"
    hidden={value !== index}
    id={`accounting-tabpanel-${index}`}
    aria-labelledby={`accounting-tab-${index}`}
  >
    {value === index && <Box sx={{ pt: 2 }}>{children}</Box>}
  </div>
);

const AccountingPage: React.FC = () => {
  const [tabValue, setTabValue] = useState(0);
  const [loading, setLoading] = useState(false);
  const [syncing, setSyncing] = useState(false);
  const [invoices, setInvoices] = useState<KSeFInvoiceListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [downloadingId, setDownloadingId] = useState<string | null>(null);

  // Modals
  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [uploadModalOpen, setUploadModalOpen] = useState(false);
  const [saveModalOpen, setSaveModalOpen] = useState(false);
  const [selectedInvoice, setSelectedInvoice] =
    useState<KSeFInvoiceListModel | null>(null);
  const [draftInvoices, setDraftInvoices] = useState<DraftAccountingInvoice[]>(
    []
  );
  const [selectedRowIds, setSelectedRowIds] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });
  const [transferring, setTransferring] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [users, setUsers] = useState<UserListModel[]>([]);

  // Sequential processing state
  const [sequentialMode, setSequentialMode] = useState(false);
  const [sequentialIndex, setSequentialIndex] = useState(0);

  // Fetch users for filter
  useEffect(() => {
    const fetchUsers = async () => {
      const response = await UsersService.getUsers({ page: 0, pageSize: 100 });
      if (response.success && response.responseData) {
        setUsers(response.responseData.items || []);
      }
    };
    fetchUsers();
  }, []);

  const filterConfig = useMemo(
    () =>
      getAccountingFiltersConfig({
        users: users.map((u) => ({ value: u.id, label: u.name })),
      }),
    [users]
  );

  const handleUploadedInvoices = (files: DraftAccountingInvoice[]) => {
    // Jeśli lista jest pusta, to znaczy że były tylko pliki XML - odśwież listę
    if (!files || files.length === 0) {
      fetchInvoices();
      return;
    }
    setDraftInvoices(files);
    setSaveModalOpen(true);
  };

  const handleDeleteAllInvoices = async () => {
    if (
      !window.confirm(
        "Czy na pewno chcesz usunąć WSZYSTKIE faktury? Ta operacja jest nieodwracalna!"
      )
    ) {
      return;
    }
    setDeleting(true);
    try {
      await handleApiResponse(
        () => AccountingService.deleteAllInvoices(),
        (data) => {
          toast.success(
            `Usunięto ${data.responseData?.deletedCount || 0} faktur`
          );
          fetchInvoices();
        },
        undefined,
        "Błąd podczas usuwania faktur"
      );
    } finally {
      setDeleting(false);
    }
  };

  // Filters for each tab (all, sales, purchases)
  const [allFilters, dispatchAllFilters] = useReducer(
    ksefFiltersReducer,
    initialKSeFFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "accountingAllGridState",
        "accountingAllPageSize",
        KSeFInvoicesOrderType,
        mapKSeFOrderTypeToField
      )
  );

  const [salesFilters, dispatchSalesFilters] = useReducer(
    ksefFiltersReducer,
    { ...initialKSeFFilters, invoiceType: KSeFInvoiceType.Sales },
    (init) =>
      initializeFiltersFromLocalStorage(
        { ...init, invoiceType: KSeFInvoiceType.Sales },
        "accountingSalesGridState",
        "accountingSalesPageSize",
        KSeFInvoicesOrderType,
        mapKSeFOrderTypeToField
      )
  );

  const [purchaseFilters, dispatchPurchaseFilters] = useReducer(
    ksefFiltersReducer,
    { ...initialKSeFFilters, invoiceType: KSeFInvoiceType.Purchase },
    (init) =>
      initializeFiltersFromLocalStorage(
        { ...init, invoiceType: KSeFInvoiceType.Purchase },
        "accountingPurchaseGridState",
        "accountingPurchasePageSize",
        KSeFInvoicesOrderType,
        mapKSeFOrderTypeToField
      )
  );

  const getCurrentFilters = useCallback((): {
    filters: KSeFInvoicesFilters;
    dispatch: React.Dispatch<any>;
    storageKey: string;
  } => {
    switch (tabValue) {
      case 1:
        return {
          filters: salesFilters,
          dispatch: dispatchSalesFilters,
          storageKey: "accountingSalesPageSize",
        };
      case 2:
        return {
          filters: purchaseFilters,
          dispatch: dispatchPurchaseFilters,
          storageKey: "accountingPurchasePageSize",
        };
      default:
        return {
          filters: allFilters,
          dispatch: dispatchAllFilters,
          storageKey: "accountingAllPageSize",
        };
    }
  }, [tabValue, allFilters, salesFilters, purchaseFilters]);

  const fetchInvoices = useCallback(async () => {
    const { filters } = getCurrentFilters();
    setLoading(true);
    try {
      await handleApiResponse(
        () => AccountingService.getKSeFInvoices(filters),
        (data) => {
          if (data.responseData) {
            setInvoices(data.responseData.items || []);
            setTotalRows(data.responseData.totalRows || 0);
          }
        },
        undefined,
        "Błąd podczas pobierania faktur"
      );
    } catch {
      toast.error("Błąd podczas pobierania faktur");
    } finally {
      setLoading(false);
    }
  }, [getCurrentFilters]);

  // Fetch data when tab or filters change
  React.useEffect(() => {
    fetchInvoices();
  }, [fetchInvoices]);

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const handleViewDetails = (invoice: KSeFInvoiceListModel) => {
    setSelectedInvoice(invoice);
    setDetailsModalOpen(true);
  };

  // Sequential processing handlers
  const handleStartSequential = () => {
    if (invoices.length === 0) {
      toast.warning("Brak faktur do przeglądania");
      return;
    }
    setSequentialIndex(0);
    setSelectedInvoice(invoices[0]);
    setSequentialMode(true);
    setDetailsModalOpen(true);
  };

  const handleSequentialNext = () => {
    if (sequentialIndex < invoices.length - 1) {
      const nextIndex = sequentialIndex + 1;
      setSequentialIndex(nextIndex);
      setSelectedInvoice(invoices[nextIndex]);
    }
  };

  const handleSequentialPrevious = () => {
    if (sequentialIndex > 0) {
      const prevIndex = sequentialIndex - 1;
      setSequentialIndex(prevIndex);
      setSelectedInvoice(invoices[prevIndex]);
    }
  };

  const handleExitSequential = () => {
    setSequentialMode(false);
    setDetailsModalOpen(false);
    setSelectedInvoice(null);
    fetchInvoices();
  };

  const handleDownloadPdf = async (invoice: KSeFInvoiceListModel) => {
    await downloadFile({
      url: ApiUrl.AccountingInvoicePdf(invoice.id),
      defaultFilename: `Faktura_${invoice.invoiceNumber}`,
      setLoading: (value) => setDownloadingId(value ? invoice.id : null),
      errorMessage: "Błąd podczas pobierania PDF faktury",
      fileExtension: "pdf",
    });
  };

  const handleDownloadXml = async (invoice: KSeFInvoiceListModel) => {
    await downloadFile({
      url: ApiUrl.AccountingInvoiceXml(invoice.id),
      defaultFilename: `Faktura_KSeF_${
        invoice.kSeFNumber || invoice.invoiceNumber
      }`,
      setLoading: (value) => setDownloadingId(value ? invoice.id : null),
      errorMessage: "Błąd podczas pobierania XML faktury",
      fileExtension: "xml",
    });
  };

  const handleTransferToOffice = async () => {
    if (!selectedRowIds.ids || selectedRowIds.ids.size === 0) {
      toast.warning("Zaznacz faktury do przekazania");
      return;
    }
    setTransferring(true);
    const invoiceIds = Array.from(selectedRowIds.ids).map((id) => String(id));
    try {
      await handleApiResponse(
        () => AccountingService.transferToOffice(invoiceIds),
        async (data) => {
          if (data.responseData) {
            if (data.responseData.transferredCount > 0) {
              toast.success(
                `Przekazano ${data.responseData.transferredCount} faktur do biura`
              );
              // Pobierz ZIP z plikami faktur
              try {
                const blob = await AccountingService.downloadInvoicesZip(
                  invoiceIds
                );
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement("a");
                link.href = url;
                link.download = `Faktury_${new Date()
                  .toISOString()
                  .slice(0, 10)}.zip`;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
              } catch {
                toast.warning("Nie udało się pobrać plików faktur");
              }
            }
            if (data.responseData.errors.length > 0) {
              data.responseData.errors.forEach((err) => toast.warning(err));
            }
            setSelectedRowIds({ type: "include", ids: new Set() });
            fetchInvoices();
          }
        },
        undefined,
        "Błąd podczas przekazywania faktur do biura"
      );
    } finally {
      setTransferring(false);
    }
  };

  const handleSyncKSeF = async () => {
    setSyncing(true);
    try {
      const response = await AccountingService.syncWithKSeF();
      if (response.success) {
        toast.success("Synchronizacja z KSeF została uruchomiona");
        // Refresh data after a short delay
        setTimeout(() => fetchInvoices(), 2000);
      } else {
        // Handle error response (including 500 errors)
        const errorMessage =
          response.domainException?.errorDescription ||
          (response.errors && typeof response.errors === "object"
            ? Object.values(response.errors).flat().join(", ")
            : null) ||
          `Błąd podczas synchronizacji z KSeF (kod: ${response.statusCode})`;
        toast.error(errorMessage);
      }
    } catch (error: unknown) {
      const errorMessage =
        error instanceof Error ? error.message : "Nieznany błąd";
      toast.error(
        `Wystąpił błąd podczas synchronizacji z KSeF: ${errorMessage}`
      );
    } finally {
      setSyncing(false);
    }
  };

  const columns = useMemo(
    () =>
      getKSeFInvoicesColumns({
        onDownloadPdf: handleDownloadPdf,
        onDownloadXml: handleDownloadXml,
        downloadingId,
      }),
    [downloadingId]
  );

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("accountingGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { id: false },
          },
        };
  });

  const renderDataGrid = () => {
    const { filters, dispatch, storageKey } = getCurrentFilters();

    return (
      <DataGridPremium
        loading={loading}
        rows={invoices}
        columns={columns}
        initialState={initialGridState}
        onStateChange={(newState: GridState) => {
          const stateToSave = {
            columns: newState.columns,
            sorting: newState.sorting,
            filter: newState.filter,
            pinnedColumns: newState.pinnedColumns,
          };
          localStorage.setItem(
            "accountingGridState",
            JSON.stringify(stateToSave)
          );
        }}
        paginationMode="server"
        pagination
        paginationModel={{
          pageSize: filters.pageSize,
          page: filters.page,
        }}
        onPaginationModelChange={({ page, pageSize }) => {
          localStorage.setItem(storageKey, pageSize.toString());
          dispatch({
            type: "setMultiple",
            payload: { page, pageSize },
          });
        }}
        rowCount={totalRows}
        checkboxSelection
        rowSelectionModel={selectedRowIds}
        onRowSelectionModelChange={(newSelection) => {
          setSelectedRowIds(newSelection);
        }}
        onRowClick={(params) => {
          handleViewDetails(params.row);
        }}
        disableRowSelectionOnClick
        pageSizeOptions={[10, 25, 50, 100]}
        slots={{ noRowsOverlay: NoRowsOverlay }}
        showToolbar
        sx={{
          [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
          [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          minHeight: 400,
          "& .MuiDataGrid-row:hover": {
            cursor: "pointer",
          },
        }}
        sortingMode="server"
        onSortModelChange={(model) => {
          const sortOptions = getSortOptionsFromGridModel(
            model,
            KSeFInvoicesOrderType,
            mapKSeFOrderTypeToField
          );
          const payload =
            model.length > 0 ? { ...sortOptions, page: 0 } : { ...sortOptions };

          dispatch({
            type: "setMultiple",
            payload,
          });
        }}
      />
    );
  };

  return (
    <Box p={4}>
      <Box
        mb={3}
        display="flex"
        flexDirection={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "flex-start", sm: "center" }}
        gap={2}
      >
        <Typography variant="h4">Księgowość</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            color="info"
            onClick={handleStartSequential}
            disabled={invoices.length === 0}
          >
            Przeglądaj faktury ({invoices.length})
          </Button>
          <Button
            variant="outlined"
            color="error"
            startIcon={<MdDeleteForever />}
            onClick={handleDeleteAllInvoices}
            disabled={deleting}
          >
            Usuń wszystkie
          </Button>
          <Button
            variant="outlined"
            color="secondary"
            startIcon={<MdSend />}
            onClick={handleTransferToOffice}
            disabled={transferring || selectedRowIds.ids.size === 0}
          >
            Przekaż do biura ({selectedRowIds.ids.size})
          </Button>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<MdSync className={syncing ? "animate-spin" : ""} />}
            onClick={handleSyncKSeF}
            disabled={syncing}
          >
            Synchronizuj z KSeF
          </Button>
          <Button
            variant="contained"
            color="primary"
            startIcon={<MdAdd />}
            onClick={() => setUploadModalOpen(true)}
          >
            Dodaj fakturę
          </Button>
        </Box>
      </Box>

      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={tabValue}
          onChange={handleTabChange}
          variant="scrollable"
          scrollButtons="auto"
        >
          <Tab label="Wszystkie faktury" />
          <Tab label="Sprzedaż" />
          <Tab label="Zakupy" />
        </Tabs>
      </Box>

      <TabPanel value={tabValue} index={0}>
        <FiltersForm
          config={filterConfig}
          filters={allFilters}
          dispatch={dispatchAllFilters}
        />
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>
      <TabPanel value={tabValue} index={1}>
        <FiltersForm
          config={filterConfig}
          filters={salesFilters}
          dispatch={dispatchSalesFilters}
        />
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>
      <TabPanel value={tabValue} index={2}>
        <FiltersForm
          config={filterConfig}
          filters={purchaseFilters}
          dispatch={dispatchPurchaseFilters}
        />
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>

      {/* Modals */}
      <InvoiceDetailsModal
        open={detailsModalOpen}
        onClose={() => {
          setDetailsModalOpen(false);
          setSelectedInvoice(null);
          if (sequentialMode) {
            setSequentialMode(false);
          }
        }}
        onSave={fetchInvoices}
        invoice={selectedInvoice}
        sequentialMode={sequentialMode}
        currentIndex={sequentialIndex}
        totalCount={invoices.length}
        onNext={handleSequentialNext}
        onPrevious={handleSequentialPrevious}
        onExitSequential={handleExitSequential}
      />

      <UploadInvoiceModal
        open={uploadModalOpen}
        onClose={() => setUploadModalOpen(false)}
        onUpload={handleUploadedInvoices}
      />

      {draftInvoices.length > 0 && (
        <SaveAccountingInvoiceModal
          open={saveModalOpen}
          onClose={() => {
            setSaveModalOpen(false);
            setDraftInvoices([]);
          }}
          draftInvoices={draftInvoices}
          onSave={(savedInvoice) => {
            setDraftInvoices((prev) =>
              prev.filter((d) => d.draftId !== savedInvoice.draftId)
            );
            if (draftInvoices.length <= 1) {
              setSaveModalOpen(false);
              fetchInvoices();
            }
          }}
        />
      )}
    </Box>
  );
};

export default AccountingPage;
