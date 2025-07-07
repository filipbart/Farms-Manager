import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import { getFeedsStockColumns } from "./stock-columns";
import {
  FeedsStocksOrderType,
  filterReducer,
  initialFilters,
} from "../../../models/feeds/stocks/stock-filters";
import FiltersForm from "../../../components/filters/filters-form";
import { getInsertionFiltersConfig } from "./filter-config.feeds-stocks";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { mapFeedsStocksOrderTypeToField } from "../../../common/helpers/feeds-stock-order-type-helper";
import AddFeedStockModal from "../../../components/modals/feeds/stocks/add-feed-stock-modal";

const FeedsStockPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

  const [loading, setLoading] = useState(false);
  const [feedsStocks, setFeedsStocks] = useState<[]>([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openModal, setOpenModal] = useState(false);
  const [selectedFeedStock, setSelectedFeedStock] = useState<null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const columns = useMemo(
    () =>
      getFeedsStockColumns({
        setSelectedFeedStock,
        setIsEditModalOpen,
      }),
    []
  );

  useEffect(() => {
    const fetchDictionaries = async () => {
      // try {
      //   await handleApiResponse(
      //     () => InsertionsService.getDictionaries(),
      //     (data) => setDictionary(data.responseData),
      //     undefined,
      //     "Błąd podczas pobierania słowników filtrów"
      //   );
      // } catch {
      //   toast.error("Błąd podczas pobierania słowników filtrów");
      // }
    };
    fetchDictionaries();
  }, []);

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
        <Typography variant="h4">Ceny pasz</Typography>
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
        config={getInsertionFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={feedsStocks}
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
              const foundOrderBy = Object.values(FeedsStocksOrderType).find(
                (orderType) =>
                  mapFeedsStocksOrderTypeToField(orderType) === sortField
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
      {/* <EditInsertionModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedInsertion(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedInsertion(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        insertion={selectedInsertion}
      /> */}

      <AddFeedStockModal
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

export default FeedsStockPage;
