import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridRowSelectionModel } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
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
import UploadInvoicesModal from "../../../components/modals/feeds/deliveries/upload-invoices-modal";
import SaveInvoiceModal from "../../../components/modals/feeds/deliveries/save-invoice-modal";
import type { DraftFeedInvoice } from "../../../models/feeds/deliveries/draft-feed-invoice";
import type { FeedDeliveryListModel } from "../../../models/feeds/deliveries/feed-invoice";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import ApiUrl from "../../../common/ApiUrl";
import EditFeedDeliveryModal from "../../../components/modals/feeds/deliveries/edit-feed-delivery-modal";
import LoadingButton from "../../../components/common/loading-button";
import GeneratePaymentModal from "../../../components/modals/feeds/deliveries/generate-paymet-modal";
import { downloadFile } from "../../../utils/download-file";

const FeedsDeliveriesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

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

  const [loadingFileId, setLoadingFileId] = useState<string | null>(null);
  const [selectedFeedDelivery, setSelectedFeedDelivery] =
    useState<FeedDeliveryListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const [selectedRows, setSelectedRows] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelDeliveries");
    return saved ? JSON.parse(saved) : {};
  });

  const [openPaymentModal, setOpenPaymentModal] = useState(false);
  const [loadingPaymentFile, setLoadingPaymentFile] = useState(false);

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

  const downloadInvoiceFile = async (id: string) => {
    await downloadFile({
      url: `${ApiUrl.DownloadFeedDeliveryFile}/${id}`,
      defaultFilename: "faktura",
      setLoading: (value) => setLoadingFileId(value ? id : null),
      errorMessage: "Błąd podczas pobierania faktury dostawy",
    });
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
  };

  const columns = useMemo(
    () =>
      getFeedsDeliveriesColumns({
        setSelectedFeedDelivery,
        setIsEditModalOpen,
        deleteFeedDelivery,
        downloadInvoiceFile,
        loadingFileId,
      }),
    [loadingFileId]
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
  }, []);

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
        </Box>
      </Box>

      <FiltersForm
        config={getFeedsDeliveriesFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={feedsDeliveries}
          columns={columns}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
            },
          }}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelDeliveries",
              JSON.stringify(model)
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
          checkboxSelection
          isRowSelectable={(params) => !params.row.paymentDateUtc}
          onRowSelectionModelChange={(newSelection) => {
            setSelectedRows(newSelection);
          }}
          rowSelectionModel={selectedRows}
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
          rowSelection
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
              const foundOrderBy = Object.values(FeedsDeliveriesOrderType).find(
                (orderType) =>
                  mapFeedsDeliveriesOrderTypeToField(orderType) === sortField
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
      {draftFeedInvoices.length > 0 && (
        <SaveInvoiceModal
          open={openSaveDataModal}
          onClose={handleCloseSaveDataModal}
          draftFeedInvoices={draftFeedInvoices}
          onSave={handleSaveInvoiceData}
        />
      )}

      <UploadInvoicesModal
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

      <GeneratePaymentModal
        open={openPaymentModal}
        onClose={() => setOpenPaymentModal(false)}
        onGenerate={generatePaymentWithComment}
      />
    </Box>
  );
};

export default FeedsDeliveriesPage;
