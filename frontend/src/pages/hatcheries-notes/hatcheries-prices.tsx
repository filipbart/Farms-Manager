import { Box, Button, tablePaginationClasses } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { getHatcheriesPriceColumns } from "./hatcheries-prices-columns";
import { getHatcheriesPricesFiltersConfig } from "./filter-config.hatcheries-prices";
import FiltersForm from "../../components/filters/filters-form";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import {
  filterReducer,
  HatcheriesPricesOrderType,
  initialFilters,
  mapHatcheriesPricesOrderTypeToField,
} from "../../models/hatcheries/hatcheries-prices-filters";
import type {
  HatcheriesNames,
  HatcheryPriceListModel,
} from "../../models/hatcheries/hatcheries-prices";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { HatcheriesService } from "../../services/hatcheries-service";
import AddHatcheryPriceModal from "../../components/modals/hatcheries/add-hatchery-price-modal";
import EditHatcheryPriceModal from "../../components/modals/hatcheries/edit-hatchery-price-modal";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../utils/grid-state-helper";
import { useAuth } from "../../auth/useAuth";

const HatcheriesPricesPanel: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "hatcheriesPricesGridState",
        "hatcheriesPricesPageSize",
        HatcheriesPricesOrderType,
        mapHatcheriesPricesOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<HatcheriesNames>();

  const [loading, setLoading] = useState(false);
  const [hatcheriesPrices, setHatcheriesPrices] = useState<
    HatcheryPriceListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);

  const [openModal, setOpenModal] = useState(false);
  const [selectedHatcheryPrice, setSelectedHatcheryPrice] =
    useState<HatcheryPriceListModel>();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const deleteHatcheryPrice = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => HatcheriesService.deleteHatcheryPrice(id),
        () => {
          toast.success("Cena została usunięta pomyślnie");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania ceny"
      );
    } catch {
      toast.error("Błąd podczas usuwania ceny");
    } finally {
      setLoading(false);
    }
  };

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("hatcheriesPricesGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const columns = useMemo(
    () =>
      getHatcheriesPriceColumns({
        setSelectedHatcheryPrice,
        setIsEditModalOpen,
        deleteHatcheryPrice,
        isAdmin,
      }),
    [isAdmin]
  );

  const fetchDictionaries = async () => {
    try {
      await handleApiResponse(
        () => HatcheriesService.getPricesNames(),
        (data) => setDictionary(data.responseData),
        undefined,
        "Błąd podczas pobierania nazw wylęgarni"
      );
    } catch {
      toast.error("Błąd podczas pobierania nazw wylęgarni");
    }
  };

  const fetchHatcheriesPrices = async () => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => HatcheriesService.getHatcheriesPrices(filters),
        (data) => {
          setHatcheriesPrices(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania cen"
      );
    } catch {
      toast.error("Błąd podczas pobierania cen");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDictionaries();
  }, []);

  useEffect(() => {
    fetchHatcheriesPrices();
  }, [filters]);

  return (
    <Box p={2}>
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
        config={getHatcheriesPricesFiltersConfig(dictionary, isAdmin)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={hatcheriesPrices}
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
              "hatcheriesPricesGridState",
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
            localStorage.setItem(
              "hatcheriesPricesPageSize",
              pageSize.toString()
            );

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
              HatcheriesPricesOrderType,
              mapHatcheriesPricesOrderTypeToField
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
      <EditHatcheryPriceModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedHatcheryPrice(undefined);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedHatcheryPrice(undefined);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        hatcheryPrice={selectedHatcheryPrice}
      />

      <AddHatcheryPriceModal
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

export default HatcheriesPricesPanel;
