import {
  Box,
  Button,
  Tab,
  Tabs,
  tablePaginationClasses,
  Typography,
} from "@mui/material";
import React, { useCallback, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import { DataGridPremium, type GridState } from "@mui/x-data-grid-premium";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import { MdAdd, MdSync } from "react-icons/md";
import { AccountingService } from "../../services/accounting-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { downloadFile } from "../../utils/download-file";
import ApiUrl from "../../common/ApiUrl";
import {
  ksefFiltersReducer,
  initialKSeFFilters,
  type KSeFInvoicesFilters,
} from "../../models/accounting/ksef-filters";
import {
  KSeFInvoiceType,
  type KSeFInvoiceListModel,
} from "../../models/accounting/ksef-invoice";
import { getKSeFInvoicesColumns } from "./ksef-invoices-columns";
import InvoiceDetailsModal from "../../components/modals/accounting/invoice-details-modal";
import UploadInvoiceModal from "../../components/modals/accounting/upload-invoice-modal";

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
  const [selectedInvoice, setSelectedInvoice] =
    useState<KSeFInvoiceListModel | null>(null);

  // Filters for each tab (all, sales, purchases)
  const [allFilters, dispatchAllFilters] = useReducer(
    ksefFiltersReducer,
    initialKSeFFilters,
    (init) => {
      const savedPageSize = localStorage.getItem("accountingAllPageSize");
      return {
        ...init,
        pageSize: savedPageSize ? parseInt(savedPageSize, 10) : init.pageSize,
      };
    }
  );

  const [salesFilters, dispatchSalesFilters] = useReducer(
    ksefFiltersReducer,
    { ...initialKSeFFilters, invoiceType: KSeFInvoiceType.Sales },
    (init) => {
      const savedPageSize = localStorage.getItem("accountingSalesPageSize");
      return {
        ...init,
        pageSize: savedPageSize ? parseInt(savedPageSize, 10) : init.pageSize,
      };
    }
  );

  const [purchaseFilters, dispatchPurchaseFilters] = useReducer(
    ksefFiltersReducer,
    { ...initialKSeFFilters, invoiceType: KSeFInvoiceType.Purchase },
    (init) => {
      const savedPageSize = localStorage.getItem("accountingPurchasePageSize");
      return {
        ...init,
        pageSize: savedPageSize ? parseInt(savedPageSize, 10) : init.pageSize,
      };
    }
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
      await handleApiResponse(
        () => AccountingService.syncWithKSeF(),
        () => {
          toast.success("Synchronizacja z KSeF została uruchomiona");
          // Refresh data after a short delay
          setTimeout(() => fetchInvoices(), 2000);
        },
        undefined,
        "Błąd podczas synchronizacji z KSeF"
      );
    } catch {
      toast.error("Błąd podczas synchronizacji z KSeF");
    } finally {
      setSyncing(false);
    }
  };

  const columns = useMemo(
    () =>
      getKSeFInvoicesColumns({
        onViewDetails: handleViewDetails,
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
        rowSelection={false}
        pageSizeOptions={[10, 25, 50, 100]}
        slots={{ noRowsOverlay: NoRowsOverlay }}
        showToolbar
        localeText={{
          paginationRowsPerPage: "Wierszy na stronę:",
          paginationDisplayedRows: ({ from, to, count }) =>
            `${from} do ${to} z ${count}`,
        }}
        sx={{
          [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
          [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          minHeight: 400,
        }}
        sortingMode="server"
        onSortModelChange={(model) => {
          if (model.length > 0) {
            dispatch({
              type: "setMultiple",
              payload: {
                orderBy: model[0].field,
                isDescending: model[0].sort === "desc",
                page: 0,
              },
            });
          }
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
        <Tabs value={tabValue} onChange={handleTabChange} variant="fullWidth">
          <Tab
            label={
              <Typography variant="subtitle1" fontWeight={600}>
                Wszystkie faktury
              </Typography>
            }
          />
          <Tab
            label={
              <Typography variant="subtitle1" fontWeight={600}>
                Sprzedaż
              </Typography>
            }
          />
          <Tab
            label={
              <Typography variant="subtitle1" fontWeight={600}>
                Zakupy
              </Typography>
            }
          />
        </Tabs>
      </Box>

      <TabPanel value={tabValue} index={0}>
        {renderDataGrid()}
      </TabPanel>
      <TabPanel value={tabValue} index={1}>
        {renderDataGrid()}
      </TabPanel>
      <TabPanel value={tabValue} index={2}>
        {renderDataGrid()}
      </TabPanel>

      {/* Modals */}
      <InvoiceDetailsModal
        open={detailsModalOpen}
        onClose={() => {
          setDetailsModalOpen(false);
          setSelectedInvoice(null);
        }}
        invoice={selectedInvoice}
      />

      <UploadInvoiceModal
        open={uploadModalOpen}
        onClose={() => setUploadModalOpen(false)}
        onSuccess={fetchInvoices}
      />
    </Box>
  );
};

export default AccountingPage;
