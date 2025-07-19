import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import FiltersForm from "../../../components/filters/filters-form";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { FeedPriceListModel } from "../../../models/feeds/prices/feed-price";
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

const FeedsDeliveriesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

  const [loading, setLoading] = useState(false);
  const [feedsPrices, setFeedsPrices] = useState<FeedPriceListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openUploadModal, setOpenUploadModal] = useState(false);
  const [openSaveDataModal, setOpenSaveDataModal] = useState(false);
  const [draftFeedInvoices, setDraftFeedInvoices] = useState<
    DraftFeedInvoice[]
  >([]);

  const [selectedFeedDelivery, setSelectedFeedDelivery] = useState();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

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
      // await handleApiResponse(
      //   () => FeedsService.deleteFeedPrice(id),
      //   () => {
      //     toast.success("Cena paszy została usunięta");
      //     dispatch({ type: "setMultiple", payload: { page: 0 } });
      //   },
      //   undefined,
      //   "Błąd podczas usuwania ceny paszy"
      // );
    } catch {
      toast.error("Błąd podczas usuwania faktury paszy");
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () =>
      getFeedsDeliveriesColumns({
        setSelectedFeedDelivery,
        setIsEditModalOpen,
        deleteFeedDelivery,
      }),
    []
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
    }

    setDraftFeedInvoices(filteredInvoices);
  };
  // const fetchFeedsPrices = async () => {
  //   try {
  //     setLoading(true);
  //     // await handleApiResponse<PaginateModel<FeedPriceListModel>>(
  //     //   () => FeedsService.getFeedsPrices(filters),
  //     //   (data) => {
  //     //     setFeedsPrices(data.responseData?.items ?? []);
  //     //     setTotalRows(data.responseData?.totalRows ?? 0);
  //     //   },
  //     //   undefined,
  //     //   "Błąd podczas pobierania cen pasz"
  //     // );
  //   } catch {
  //     toast.error("Błąd podczas pobierania cen pasz");
  //   } finally {
  //     setLoading(false);
  //   }
  // };

  useEffect(() => {
    fetchDictionaries();
  }, []);

  useEffect(() => {
    //fetchFeedsPrices();
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
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            color="secondary"
            onClick={() => setOpenSaveDataModal(true)}
          >
            Test zapisywania faktury
          </Button>
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
          rows={feedsPrices}
          columns={columns}
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
    </Box>
  );
};

export default FeedsDeliveriesPage;
