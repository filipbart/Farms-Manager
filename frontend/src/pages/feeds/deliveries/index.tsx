import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridRowSelectionModel } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect, useContext } from "react";
import { toast } from "react-toastify";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import FiltersForm from "../../../components/filters/filters-form";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import { FeedsService } from "../../../services/feeds-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { getFeedsDeliveriesFiltersConfig } from "./filter-config.feeds-deliveries";
import { mapFeedsDeliveriesOrderTypeToField } from "../../../common/helpers/feeds-delivery-order-type-helper";
import {
  FeedsDeliveriesOrderType,
  filterReducer,
  initialFilters,
} from "../../../models/feeds/deliveries/deliveries-filters";
import { getFeedsDeliveriesColumns } from "./deliveries-columns";
import UploadFeedInvoicesModal from "../../../components/modals/feeds/deliveries/upload-feed-invoices-modal";
import SaveInvoiceModal from "../../../components/modals/feeds/deliveries/save-invoice-modal";
import type { DraftFeedInvoice } from "../../../models/feeds/deliveries/draft-feed-invoice";
import type { FeedDeliveryListModel } from "../../../models/feeds/deliveries/feed-invoice";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import ApiUrl from "../../../common/ApiUrl";
import EditFeedDeliveryModal from "../../../components/modals/feeds/deliveries/edit-feed-delivery-modal";
import LoadingButton from "../../../components/common/loading-button";
import GeneratePaymentModal from "../../../components/modals/feeds/deliveries/generate-paymet-modal";
import { downloadFile } from "../../../utils/download-file";
import AddCorrectionModal from "../../../components/modals/feeds/deliveries/add-correction-modal";
import EditCorrectionModal from "../../../components/modals/feeds/deliveries/edit-correction-modal";
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
import { useFeedsNames } from "../../../hooks/feeds/useFeedsNames";

const FeedsDeliveriesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "feedsDeliveriesGridState",
        "feedsDeliveriesPageSize",
        FeedsDeliveriesOrderType,
        mapFeedsDeliveriesOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<FeedsDictionary>();
  const { fetchNotifications } = useContext(NotificationContext);
  const { feedsNames, fetchFeedsNames } = useFeedsNames();

  const [loading, setLoading] = useState(false);
  const [feedsDeliveries, setFeedsDeliveries] = useState<
    FeedDeliveryListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openUploadModal, setOpenUploadModal] = useState(false);
  const [openSaveDataModal, setOpenSaveDataModal] = useState(false);
  const [draftFeedInvoices, setDraftFeedInvoices] = useState<
    DraftFeedInvoice[]
  >([]);

  const [downloadFilePath, setDownloadFilePath] = useState<string | null>(null);
  const [selectedFeedDelivery, setSelectedFeedDelivery] =
    useState<FeedDeliveryListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isEditCorrectionModalOpen, setIsEditCorrectionModalOpen] =
    useState(false);

  const [selectedRows, setSelectedRows] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("feedsDeliveriesGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: { columnVisibilityModel: { dateCreatedUtc: false } },
        };
  });

  const [openPaymentModal, setOpenPaymentModal] = useState(false);
  const [loadingPaymentFile, setLoadingPaymentFile] = useState(false);
  const [loadingZipFile, setLoadingZipFile] = useState(false);

  const [openCorrectionModal, setOpenCorrectionModal] = useState(false);
  const [isCorrectionMode, setIsCorrectionMode] = useState(false);
  const [isDownloadMode, setIsDownloadMode] = useState(false);

  const handleCancelCorrectionMode = () => {
    setIsCorrectionMode(false);
    setSelectedRows({ type: "include", ids: new Set() });
  };

  const handleCancelDownloadMode = () => {
    setIsDownloadMode(false);
    setSelectedRows({ type: "include", ids: new Set() });
  };

  const uploadFiles = async (draftFiles: DraftFeedInvoice[]) => {
    if (draftFiles.length === 0) {
      toast.error("Brak plików do przetworzenia");
      return;
    }
    setDraftFeedInvoices(draftFiles);
    setOpenSaveDataModal(true);
  };

  const deleteFeedDelivery = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => FeedsService.deleteFeedDelivery(id),
        () => {
          toast.success("Wpis dostawy razem z fakturą zostały usunięte");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania faktury paszy"
      );
    } catch {
      toast.error("Błąd podczas usuwania faktury paszy");
    } finally {
      setLoading(false);
    }
  };

  const deleteFeedCorrection = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => FeedsService.deleteFeedCorrection(id),
        () => {
          toast.success("Korekta została poprawnie usunięta");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania korekty"
      );
    } catch {
      toast.error("Błąd podczas usuwania korekty");
    } finally {
      setLoading(false);
    }
  };

  const downloadInvoiceFile = async (id: string) => {
    await downloadFile({
      url: `${ApiUrl.DownloadFeedDeliveryFile}/${id}`,
      defaultFilename: "faktura",
      setLoading: (value) => setDownloadFilePath(value ? id : null),
      errorMessage: "Błąd podczas pobierania faktury dostawy",
    });
  };

  const downloadCorrectionFile = async (filePath: string) => {
    const lastDotIndex = filePath.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < filePath.length - 1
        ? filePath.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath },
      defaultFilename: "FakturaKorekty",
      setLoading: (value) => setDownloadFilePath(value ? filePath : null),
      errorMessage: "Błąd podczas pobierania faktury korekty",
      fileExtension: fileExtension,
    });
  };

  const downloadMultipleInvoices = async () => {
    const selectedDeliveries = feedsDeliveries.filter((delivery) =>
      selectedRows.ids.has(delivery.id)
    );

    if (selectedDeliveries.length === 0) {
      toast.warning("Nie wybrano żadnych dostaw");
      return;
    }

    // Pobierz faktury zwykłych dostaw (używając ID)
    const regularDeliveryIds = selectedDeliveries
      .filter((delivery) => !delivery.isCorrection)
      .map((delivery) => delivery.id);

    // Pobierz faktury korekt (używając filePath)
    const correctionFilePaths = selectedDeliveries
      .filter((delivery) => delivery.isCorrection && delivery.filePath)
      .map((delivery) => delivery.filePath as string);

    if (regularDeliveryIds.length === 0 && correctionFilePaths.length === 0) {
      toast.warning("Wybrane dostawy nie mają przypisanych faktur");
      return;
    }

    await downloadFile({
      url: ApiUrl.DownloadFeedsInvoicesZip,
      params: { 
        deliveryIds: regularDeliveryIds,
        correctionFilePaths: correctionFilePaths 
      },
      defaultFilename: "faktury_pasz",
      setLoading: setLoadingZipFile,
      errorMessage: "Błąd podczas pobierania faktur",
      fileExtension: "zip",
    });
    
    handleCancelDownloadMode();
  };

  const generatePaymentWithComment = async (comment: string) => {
    setOpenPaymentModal(false);
    await downloadFile({
      url: ApiUrl.DownloadPaymentFile,
      params: {
        ids: [...selectedRows.ids],
        comment,
      },
      defaultFilename: "przelew",
      setLoading: setLoadingPaymentFile,
      errorMessage: "Błąd podczas pobierania pliku przelewu",
    });
    dispatch({ type: "setMultiple", payload: { page: filters.page } });
    fetchNotifications();
  };

  const columns = useMemo(
    () =>
      getFeedsDeliveriesColumns({
        setSelectedFeedDelivery,
        setIsEditModalOpen,
        setIsEditCorrectionModalOpen,
        deleteFeedDelivery,
        deleteFeedCorrection,
        downloadInvoiceFile,
        downloadCorrectionFile,
        downloadFilePath,
      }),
    [downloadFilePath]
  );

  const fetchDictionaries = async () => {
    try {
      await handleApiResponse(
        () => FeedsService.getDictionaries(),
        (data) => setDictionary(data.responseData),
        undefined,
        "Błąd podczas pobierania słowników filtrów"
      );
    } catch {
      toast.error("Błąd podczas pobierania słowników filtrów");
    }
  };

  const handleCloseSaveDataModal = () => {
    setDraftFeedInvoices([]);
    setOpenSaveDataModal(false);
    dispatch({ type: "setMultiple", payload: { page: 0 } });
  };

  const handleSaveInvoiceData = (feedInvoiceData: DraftFeedInvoice) => {
    const filteredInvoices = draftFeedInvoices.filter(
      (t) => t.draftId !== feedInvoiceData.draftId
    );

    if (filteredInvoices.length === 0) {
      setDraftFeedInvoices([]);
      setOpenSaveDataModal(false);
      dispatch({ type: "setMultiple", payload: { page: 0 } });
    }

    setDraftFeedInvoices(filteredInvoices);
  };

  const fetchFeedsDeliveries = async () => {
    try {
      setLoading(true);
      await handleApiResponse<PaginateModel<FeedDeliveryListModel>>(
        () => FeedsService.getFeedsDeliveries(filters),
        (data) => {
          setFeedsDeliveries(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania dostaw pasz"
      );
    } catch {
      toast.error("Błąd podczas pobierania dostaw pasz");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDictionaries();
    fetchFeedsNames();
  }, [fetchFeedsNames]);

  useEffect(() => {
    fetchFeedsDeliveries();
  }, [filters]);

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
        <Typography variant="h4">Dostawy pasz</Typography>
        <Box display="flex" gap={2} alignItems="center">
          {isCorrectionMode ? (
            <>
              <Button
                variant="contained"
                color="primary"
                onClick={() => setOpenCorrectionModal(true)}
                disabled={selectedRows.ids.size === 0}
              >
                Wprowadź fakturę korygującą
              </Button>
              <Button
                variant="outlined"
                color="inherit"
                onClick={handleCancelCorrectionMode}
              >
                Anuluj
              </Button>
            </>
          ) : isDownloadMode ? (
            <>
              <LoadingButton
                loading={loadingZipFile}
                variant="contained"
                color="primary"
                onClick={downloadMultipleInvoices}
                disabled={selectedRows.ids.size === 0}
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
                onClick={() => setIsCorrectionMode(true)}
              >
                Utwórz korektę
              </Button>
              <Button
                variant="outlined"
                color="primary"
                onClick={() => setIsDownloadMode(true)}
              >
                Pobierz faktury
              </Button>
              {selectedRows.ids.size > 0 && (
                <LoadingButton
                  loading={loadingPaymentFile}
                  variant="outlined"
                  color="primary"
                  onClick={() => setOpenPaymentModal(true)}
                >
                  Przelew
                </LoadingButton>
              )}
              <Button
                variant="contained"
                color="primary"
                onClick={() => setOpenUploadModal(true)}
              >
                Wprowadź fakturę
              </Button>
            </>
          )}
        </Box>
      </Box>

      <FiltersForm
        config={getFeedsDeliveriesFiltersConfig(
          dictionary,
          uniqueCycles,
          filters,
          feedsNames
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={feedsDeliveries}
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
              "feedsDeliveriesGridState",
              JSON.stringify(stateToSave)
            );
          }}
          checkboxSelection
          disableRowSelectionOnClick
          isRowSelectable={(params) =>
            isCorrectionMode || isDownloadMode || !params.row.paymentDateUtc
          }
          onRowSelectionModelChange={(newSelection) => {
            setSelectedRows(newSelection);
          }}
          rowSelectionModel={selectedRows}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem(
              "feedsDeliveriesPageSize",
              pageSize.toString()
            );

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection
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
          scrollbarSize={17}
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              FeedsDeliveriesOrderType,
              mapFeedsDeliveriesOrderTypeToField
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
      {draftFeedInvoices.length > 0 && (
        <SaveInvoiceModal
          open={openSaveDataModal}
          onClose={handleCloseSaveDataModal}
          draftFeedInvoices={draftFeedInvoices}
          onSave={handleSaveInvoiceData}
        />
      )}

      <UploadFeedInvoicesModal
        open={openUploadModal}
        onClose={() => setOpenUploadModal(false)}
        onUpload={uploadFiles}
      />

      <EditFeedDeliveryModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedFeedDelivery(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedFeedDelivery(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        feedDelivery={selectedFeedDelivery}
      />

      <EditCorrectionModal
        open={isEditCorrectionModalOpen}
        onClose={() => {
          setIsEditCorrectionModalOpen(false);
          setSelectedFeedDelivery(null);
        }}
        onSave={() => {
          setIsEditCorrectionModalOpen(false);
          setSelectedFeedDelivery(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        correction={selectedFeedDelivery}
      />

      <GeneratePaymentModal
        open={openPaymentModal}
        onClose={() => setOpenPaymentModal(false)}
        onGenerate={generatePaymentWithComment}
      />

      {selectedRows.ids.size > 0 && (
        <AddCorrectionModal
          open={openCorrectionModal}
          onClose={() => {
            setOpenCorrectionModal(false);
            handleCancelCorrectionMode();
          }}
          onSave={() => {
            setOpenCorrectionModal(false);
            handleCancelCorrectionMode();
            dispatch({ type: "setMultiple", payload: { page: filters.page } });
          }}
          selectedDeliveries={selectedRows.ids}
        />
      )}
    </Box>
  );
};

export default FeedsDeliveriesPage;
