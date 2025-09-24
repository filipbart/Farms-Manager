import { Box, Button, tablePaginationClasses } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { mapFeedsPricesOrderTypeToField } from "../../../../common/helpers/feeds-price-order-type-helper";
import NoRowsOverlay from "../../../../components/datagrid/custom-norows";
import FiltersForm from "../../../../components/filters/filters-form";
import AddFeedPriceModal from "../../../../components/modals/feeds/prices/add-feed-price-modal";
import type { CycleDictModel } from "../../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../../models/feeds/feeds-dictionary";
import {
  FeedsPricesOrderType,
  filterReducer,
  initialFilters,
} from "../../../../models/feeds/prices/price-filters";
import { FeedsService } from "../../../../services/feeds-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { getFeedsPricesFiltersConfig } from "./filter-config.feeds-prices";
import { getFeedsPriceColumns } from "../price-columns";
import type { FeedPriceListModel } from "../../../../models/feeds/prices/feed-price";
import type { PaginateModel } from "../../../../common/interfaces/paginate";
import EditFeedPriceModal from "../../../../components/modals/feeds/prices/edit-feed-price-modal";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../../../utils/grid-state-helper";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";

const FeedsPricesTab: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "feedPricesGridState",
        "feedsPricesPageSize",
        FeedsPricesOrderType,
        mapFeedsPricesOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<FeedsDictionary>();
  const { feedsNames, fetchFeedsNames } = useFeedsNames();

  const [loading, setLoading] = useState(false);
  const [feedsPrices, setFeedsPrices] = useState<FeedPriceListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openModal, setOpenModal] = useState(false);
  const [selectedFeedPrice, setSelectedFeedPrice] = useState<null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("feedPricesGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const deleteFeedPrice = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => FeedsService.deleteFeedPrice(id),
        () => {
          toast.success("Cena paszy została usunięta");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania ceny paszy"
      );
    } catch {
      toast.error("Błąd podczas usuwania ceny paszy");
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () =>
      getFeedsPriceColumns({
        setSelectedFeedPrice,
        setIsEditModalOpen,
        deleteFeedPrice,
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

  const fetchFeedsPrices = async () => {
    try {
      setLoading(true);
      await handleApiResponse<PaginateModel<FeedPriceListModel>>(
        () => FeedsService.getFeedsPrices(filters),
        (data) => {
          setFeedsPrices(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania cen pasz"
      );
    } catch {
      toast.error("Błąd podczas pobierania cen pasz");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDictionaries();
    fetchFeedsNames();
  }, [fetchFeedsNames]);

  useEffect(() => {
    fetchFeedsPrices();
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
        <Box></Box>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenModal(true)}
          >
            Wprowadź cenę
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getFeedsPricesFiltersConfig(
          dictionary,
          uniqueCycles,
          feedsNames
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={feedsPrices}
          columns={columns}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
            };
            localStorage.setItem(
              "feedPricesGridState",
              JSON.stringify(stateToSave)
            );
          }}
          scrollbarSize={17}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem("feedsPricesPageSize", pageSize.toString());

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection={false}
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
              FeedsPricesOrderType,
              mapFeedsPricesOrderTypeToField
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
      <EditFeedPriceModal
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
        feedPrice={selectedFeedPrice}
      />

      <AddFeedPriceModal
        open={openModal}
        onClose={() => setOpenModal(false)}
        onSave={() => {
          setOpenModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default FeedsPricesTab;
