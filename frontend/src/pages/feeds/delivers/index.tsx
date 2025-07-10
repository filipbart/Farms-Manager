import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { mapFeedsPricesOrderTypeToField } from "../../../common/helpers/feeds-price-order-type-helper";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import FiltersForm from "../../../components/filters/filters-form";
import AddFeedPriceModal from "../../../components/modals/feeds/prices/add-feed-price-modal";
import EditFeedPriceModal from "../../../components/modals/feeds/prices/edit-feed-price-modal";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { FeedPriceListModel } from "../../../models/feeds/prices/feed-price";
import { FeedsPricesOrderType } from "../../../models/feeds/prices/price-filters";
import { FeedsService } from "../../../services/feeds-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { getFeedsDeliversFiltersConfig } from "./filter-config.feeds-delivers";
import { getFeedsDeliversColumns } from "./delivers-columns";
import {
  FeedsDeliversOrderType,
  filterReducer,
  initialFilters,
} from "../../../models/feeds/delivers/delivers-filters";
import { mapFeedsDeliversOrderTypeToField } from "../../../common/helpers/feeds-delivery-order-type-helper";

const FeedsDeliversPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

  const [loading, setLoading] = useState(false);
  const [feedsPrices, setFeedsPrices] = useState<FeedPriceListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openModal, setOpenModal] = useState(false);
  const [selectedFeedDelivery, setSelectedFeedDelivery] = useState<null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

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
      getFeedsDeliversColumns({
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
            variant="contained"
            color="primary"
            onClick={() => setOpenModal(true)}
          >
            Wprowadź fakturę
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getFeedsDeliversFiltersConfig(dictionary, uniqueCycles)}
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
              const foundOrderBy = Object.values(FeedsDeliversOrderType).find(
                (orderType) =>
                  mapFeedsDeliversOrderTypeToField(orderType) === sortField
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
      {/* <EditFeedPriceModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedFeedPrice(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedFeedPrice(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        feedPrice={selectedFeedDelivery}
      />

      <AddFeedPriceModal
        open={openModal}
        onClose={() => setOpenModal(false)}
        onSave={() => {
          setOpenModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      /> */}
    </Box>
  );
};

export default FeedsDeliversPage;
