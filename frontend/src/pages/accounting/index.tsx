import {
  Box,
  Button,
  Collapse,
  Grid,
  Tab,
  Tabs,
  tablePaginationClasses,
  Tooltip,
  Typography,
  IconButton,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import InfoIcon from "@mui/icons-material/Info";
import { MdAdd, MdSync } from "react-icons/md";
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
  type GridRowSelectionModel,
  type GridSortDirection,
} from "@mui/x-data-grid-premium";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
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
import NonKSeFInvoiceDetailsModal from "../../components/modals/accounting/non-ksef-invoice-details-modal";
import UploadInvoiceModal from "../../components/modals/accounting/upload-invoice-modal";
import SaveAccountingInvoiceModal from "../../components/modals/accounting/save-accounting-invoice-modal";
import type { DraftAccountingInvoice } from "../../services/accounting-service";
import { RenderFilterField } from "../../components/filters/render-filter-field";
import type { FilterConfig } from "../../components/filters/filter-types";
import { getAccountingFiltersConfig } from "./filter-config.accounting";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../utils/grid-state-helper";
import { UsersService } from "../../services/users-service";
import type { UserListModel } from "../../models/users/users";
import { FarmsService } from "../../services/farms-service";
import type FarmRowModel from "../../models/farms/farm-row-model";
import ConfirmDialog from "../../components/common/confirm-dialog";
import { getAccountingDueDateClassName } from "../../utils/due-date-helper";
import { useNotifications } from "../../context/notification-context";
import { useAuth } from "../../auth/useAuth";

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
  const { fetchNotifications } = useNotifications();
  const { userData } = useAuth();
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
    [],
  );
  const [selectedRowIds, setSelectedRowIds] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });
  const [deletingInvoiceId, setDeletingInvoiceId] = useState<string | null>(
    null,
  );
  const [advancedFiltersOpen, setAdvancedFiltersOpen] = useState(false);
  const [invoiceToDelete, setInvoiceToDelete] =
    useState<KSeFInvoiceListModel | null>(null);
  const [users, setUsers] = useState<UserListModel[]>([]);
  const [farms, setFarms] = useState<FarmRowModel[]>([]);

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

  // Fetch farms for filter
  useEffect(() => {
    const fetchFarms = async () => {
      const response = await FarmsService.getFarmsAsync();
      if (response.success && response.responseData) {
        setFarms(response.responseData.items || []);
      }
    };
    fetchFarms();
  }, []);

  const filterConfig = useMemo(
    () =>
      getAccountingFiltersConfig({
        users: users.map((u) => ({ value: u.id, label: u.name })),
        farms: farms.map((f) => ({ value: f.id, label: f.name })),
      }),
    [users, farms],
  );

  const filterConfigMap = useMemo(() => {
    return filterConfig.reduce(
      (acc, filter) => {
        acc[filter.key] = filter;
        return acc;
      },
      {} as Record<keyof KSeFInvoicesFilters, FilterConfig>,
    );
  }, [filterConfig]);

  const renderFilterField = useCallback(
    (
      filters: KSeFInvoicesFilters,
      dispatch: React.Dispatch<
        | { type: "set"; key: keyof KSeFInvoicesFilters; value: any }
        | { type: "setMultiple"; payload: Partial<KSeFInvoicesFilters> }
      >,
      key: keyof KSeFInvoicesFilters,
      gridProps: { xs?: number; sm?: number; md?: number; lg?: number } = {},
    ) => {
      const filter = filterConfigMap[key];
      if (!filter) return null;
      return (
        <Grid key={key} size={gridProps}>
          <RenderFilterField
            filter={filter}
            value={(filters[key] as string | string[] | boolean | null) ?? null}
            onChange={(val) =>
              dispatch({
                type: "setMultiple",
                payload: { [key]: val } as Partial<KSeFInvoicesFilters>,
              })
            }
          />
        </Grid>
      );
    },
    [filterConfigMap],
  );

  const renderFilters = (
    filters: KSeFInvoicesFilters,
    dispatch: React.Dispatch<
      | { type: "set"; key: keyof KSeFInvoicesFilters; value: any }
      | { type: "setMultiple"; payload: Partial<KSeFInvoicesFilters> }
    >,
  ) => (
    <Box sx={{ maxWidth: 1100, width: "100%" }}>
      <Grid container spacing={2} mb={2}>
        {renderFilterField(filters, dispatch, "buyerName", {
          xs: 12,
          sm: 6,
          md: 4,
          lg: 3,
        })}
        {renderFilterField(filters, dispatch, "sellerName", {
          xs: 12,
          sm: 6,
          md: 4,
          lg: 3,
        })}
        {renderFilterField(filters, dispatch, "invoiceNumber", {
          xs: 12,
          sm: 6,
          md: 4,
          lg: 3,
        })}
        {renderFilterField(filters, dispatch, "farmId", {
          xs: 12,
          sm: 6,
          md: 4,
          lg: 3,
        })}
      </Grid>
      <Grid container spacing={2} mb={2}>
        {renderFilterField(filters, dispatch, "status", {
          xs: 12,
          sm: 6,
          md: 4,
        })}
        {renderFilterField(filters, dispatch, "assignedUserId", {
          xs: 12,
          sm: 6,
          md: 4,
        })}
        {renderFilterField(filters, dispatch, "paymentStatus", {
          xs: 12,
          sm: 6,
          md: 4,
        })}
      </Grid>
      <Button
        variant="text"
        color="primary"
        onClick={() => setAdvancedFiltersOpen((prev) => !prev)}
        endIcon={
          <ExpandMoreIcon
            sx={{
              transform: advancedFiltersOpen ? "rotate(180deg)" : "rotate(0)",
              transition: "transform 0.2s",
            }}
          />
        }
      >
        Zaawansowane filtry
      </Button>
      <Collapse in={advancedFiltersOpen} timeout="auto" unmountOnExit>
        <Box mt={2}>
          <Grid container spacing={2} mb={2}>
            {renderFilterField(filters, dispatch, "invoiceDateFrom", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
            {renderFilterField(filters, dispatch, "invoiceDateTo", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
          </Grid>
          <Grid container spacing={2} mb={2}>
            {renderFilterField(filters, dispatch, "paymentDueDateFrom", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
            {renderFilterField(filters, dispatch, "paymentDueDateTo", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
          </Grid>
          <Grid container spacing={2}>
            {renderFilterField(filters, dispatch, "source", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
            {renderFilterField(filters, dispatch, "moduleType", {
              xs: 12,
              sm: 6,
              md: 6,
            })}
          </Grid>
        </Box>
      </Collapse>
    </Box>
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
        mapKSeFOrderTypeToField,
      ),
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
        mapKSeFOrderTypeToField,
      ),
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
        mapKSeFOrderTypeToField,
      ),
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
        "Błąd podczas pobierania faktur",
      );
    } catch {
      toast.error("Błąd podczas pobierania faktur");
    } finally {
      setLoading(false);
    }
  }, [getCurrentFilters]);

  const handleDeleteInvoice = useCallback(
    async (invoice: KSeFInvoiceListModel) => {
      setInvoiceToDelete(invoice);
    },
    [],
  );

  const confirmDeleteInvoice = useCallback(async () => {
    if (!invoiceToDelete) return;
    setDeletingInvoiceId(invoiceToDelete.id);
    try {
      await handleApiResponse(
        () => AccountingService.deleteInvoice(invoiceToDelete.id),
        () => {
          toast.success(`Usunięto fakturę: ${invoiceToDelete.invoiceNumber}`);
          fetchInvoices();
        },
        undefined,
        "Błąd podczas usuwania faktury",
      );
    } finally {
      setDeletingInvoiceId(null);
      setInvoiceToDelete(null);
    }
  }, [fetchInvoices, invoiceToDelete]);

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
        `Wystąpił błąd podczas synchronizacji z KSeF: ${errorMessage}`,
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
        onDelete: handleDeleteInvoice,
        downloadingId: downloadingId || deletingInvoiceId,
      }),
    [downloadingId, deletingInvoiceId, handleDeleteInvoice],
  );

  const getGridStateKey = useCallback(() => {
    switch (tabValue) {
      case 1:
        return "accountingSalesGridState";
      case 2:
        return "accountingPurchaseGridState";
      default:
        return "accountingAllGridState";
    }
  }, [tabValue]);

  const getInitialGridState = useCallback(() => {
    const { filters } = getCurrentFilters();
    const sortModel: Array<{ field: string; sort: GridSortDirection }> = [];

    if (filters.orderBy) {
      const field = mapKSeFOrderTypeToField(filters.orderBy);
      sortModel.push({
        field,
        sort: (filters.isDescending ? "desc" : "asc") as GridSortDirection,
      });
    }

    return {
      columns: {
        columnVisibilityModel: { id: false, quantity: false },
      },
      sorting: {
        sortModel,
      },
    };
  }, [getCurrentFilters]);

  const renderDataGrid = () => {
    const { filters, dispatch, storageKey } = getCurrentFilters();
    const gridStateKey = getGridStateKey();
    const initialGridState = getInitialGridState();

    const sortModel = filters.orderBy
      ? [
          {
            field: mapKSeFOrderTypeToField(filters.orderBy),
            sort: (filters.isDescending ? "desc" : "asc") as GridSortDirection,
          },
        ]
      : [];

    return (
      <DataGridPremium
        key={gridStateKey}
        loading={loading}
        rows={invoices}
        columns={columns}
        initialState={initialGridState}
        sortModel={sortModel}
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
        getRowClassName={(params) => {
          // Only highlight rows for invoices assigned to the current user
          const isAssignedToCurrentUser =
            params.row.assignedUserId === userData?.id;
          if (!isAssignedToCurrentUser) {
            return "";
          }
          return getAccountingDueDateClassName(
            params.row.paymentDueDate,
            params.row.paymentDate,
          );
        }}
        sx={{
          [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
          [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          minHeight: 400,
          "& .MuiDataGrid-row:hover": {
            cursor: "pointer",
          },
          "& .payment-overdue .MuiDataGrid-cell": {
            backgroundColor: "#ffebee",
          },
          "& .payment-overdue:hover .MuiDataGrid-cell": {
            backgroundColor: "#ffcdd2",
          },
          "& .payment-due-soon .MuiDataGrid-cell": {
            backgroundColor: "#fff3e0",
          },
          "& .payment-due-soon:hover .MuiDataGrid-cell": {
            backgroundColor: "#ffe0b2",
          },
          "& .payment-due-warning .MuiDataGrid-cell": {
            backgroundColor: "#fffde7",
          },
          "& .payment-due-warning:hover .MuiDataGrid-cell": {
            backgroundColor: "#fff9c4",
          },
        }}
        sortingMode="server"
        onSortModelChange={(model) => {
          const sortOptions = getSortOptionsFromGridModel(
            model,
            KSeFInvoicesOrderType,
            mapKSeFOrderTypeToField,
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
        <Box display="flex" alignItems="center" gap={1}>
          <Typography variant="h4">Księgowość</Typography>
          <Tooltip
            title={
              <Box>
                <Typography
                  variant="subtitle2"
                  sx={{ fontWeight: "bold", mb: 1 }}
                >
                  Legenda statusu terminów (Księgowość)
                </Typography>
                <Typography variant="body2">
                  Kolory informują o czasie pozostałym do terminu płatności:
                </Typography>
                <Box component="ul" sx={{ pl: 2, mt: 1, mb: 0 }}>
                  <Typography component="li" variant="body2">
                    14+ dni – bez zmian
                  </Typography>
                  <Typography component="li" variant="body2">
                    🟡 14–8 dni – zbliżający się termin
                  </Typography>
                  <Typography component="li" variant="body2">
                    🟠 7–4 dni – pilne
                  </Typography>
                  <Typography component="li" variant="body2">
                    🔴 3–1 dni – bardzo pilne
                  </Typography>
                  <Typography component="li" variant="body2">
                    🔴 Po terminie – po terminie
                  </Typography>
                </Box>
              </Box>
            }
            placement="right"
            arrow
          >
            <IconButton size="small" sx={{ color: "text.secondary" }}>
              <InfoIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
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
        {renderFilters(allFilters, dispatchAllFilters)}
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>
      <TabPanel value={tabValue} index={1}>
        {renderFilters(salesFilters, dispatchSalesFilters)}
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>
      <TabPanel value={tabValue} index={2}>
        {renderFilters(purchaseFilters, dispatchPurchaseFilters)}
        <Box mt={3}>{renderDataGrid()}</Box>
      </TabPanel>

      {/* Modals */}
      {/* Modal for KSeF invoices */}
      {selectedInvoice?.source !== "Manual" && (
        <InvoiceDetailsModal
          open={detailsModalOpen}
          onClose={() => {
            setDetailsModalOpen(false);
            setSelectedInvoice(null);
            if (sequentialMode) {
              setSequentialMode(false);
            }
          }}
          onSave={() => {
            fetchInvoices();
            fetchNotifications();
          }}
          invoice={selectedInvoice}
          sequentialMode={sequentialMode}
          currentIndex={sequentialIndex}
          totalCount={invoices.length}
          onNext={handleSequentialNext}
          onPrevious={handleSequentialPrevious}
          onExitSequential={handleExitSequential}
        />
      )}

      {/* Modal for non-KSeF (Manual) invoices */}
      {selectedInvoice?.source === "Manual" && (
        <NonKSeFInvoiceDetailsModal
          open={detailsModalOpen}
          onClose={() => {
            setDetailsModalOpen(false);
            setSelectedInvoice(null);
            if (sequentialMode) {
              setSequentialMode(false);
            }
          }}
          onSave={() => {
            fetchInvoices();
            fetchNotifications();
          }}
          invoice={selectedInvoice}
          sequentialMode={sequentialMode}
          currentIndex={sequentialIndex}
          totalCount={invoices.length}
          onNext={handleSequentialNext}
          onPrevious={handleSequentialPrevious}
          onExitSequential={handleExitSequential}
        />
      )}

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
              prev.filter((d) => d.draftId !== savedInvoice.draftId),
            );
            // Always refresh the table after each save
            fetchInvoices();
            fetchNotifications();
            if (draftInvoices.length <= 1) {
              setSaveModalOpen(false);
            }
          }}
        />
      )}

      <ConfirmDialog
        open={Boolean(invoiceToDelete)}
        onClose={() => setInvoiceToDelete(null)}
        onConfirm={confirmDeleteInvoice}
        title="Usuń fakturę"
        content={
          invoiceToDelete
            ? `Czy na pewno chcesz usunąć fakturę "${invoiceToDelete.invoiceNumber}"? Ta operacja jest nieodwracalna!`
            : "Czy na pewno chcesz usunąć fakturę? Ta operacja jest nieodwracalna!"
        }
        confirmText="Usuń"
        confirmColor="error"
      />
    </Box>
  );
};

export default AccountingPage;
