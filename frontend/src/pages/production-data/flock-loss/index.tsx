import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { type ProductionDataWeighingsDictionary } from "../../../models/production-data/weighings-filters";
import type { ProductionDataFlockLossListModel } from "../../../models/production-data/flock-loss";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { ProductionDataWeighingsService } from "../../../services/production-data/production-data-weighings-service";
import FiltersForm from "../../../components/filters/filters-form";
import { getFlockLossColumns } from "./flock-loss-columns";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import { getProductionDataFlockLossFiltersConfig } from "./filter-config.production-data-flock-loss";
import {
  filterReducer,
  initialFilters,
  mapProductionDataFlockLossOrderTypeToField,
  ProductionDataFlockLossOrderType,
} from "../../../models/production-data/flock-loss-filters";
import AddProductionDataFlockLossModal from "../../../components/modals/production-data/flock-loss/add-flock-loss-modal";
import EditProductionDataFlockLossModal from "../../../components/modals/production-data/flock-loss/edit-flock-loss-modal";
import { ProductionDataFlockLossService } from "../../../services/production-data/flock-loss-measures-service";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../../utils/grid-state-helper";
import { useAuth } from "../../../auth/useAuth";

const ProductionDataFlockLossPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "productionDataFlockLossGridState",
        "productionDataFlockLossPageSize",
        ProductionDataFlockLossOrderType,
        mapProductionDataFlockLossOrderTypeToField
      )
  );
  const [dictionary, setDictionary] =
    useState<ProductionDataWeighingsDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [flockLosses, setFlockLosses] = useState<
    ProductionDataFlockLossListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [selectedFlockLoss, setSelectedFlockLoss] =
    useState<ProductionDataFlockLossListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("productionDataFlockLossGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

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

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => ProductionDataWeighingsService.getDictionaries(),
          (data) => {
            setDictionary(data.responseData);
          },
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania słowników filtrów");
      }
    };
    fetchDictionaries();
  }, []);

  const deleteFlockLoss = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => ProductionDataFlockLossService.deleteFlockLoss(id),
        async () => {
          toast.success("Wpis został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wpisu"
      );
    } catch {
      toast.error("Błąd podczas usuwania wpisu");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const fetchFlockLosses = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ProductionDataFlockLossService.getFlockLosses(filters),
          (data) => {
            setFlockLosses(data.responseData?.items ?? []);
            setTotalRows(data.responseData?.totalRows ?? 0);
          },
          undefined,
          "Błąd podczas pobierania danych"
        );
      } catch {
        toast.error("Błąd podczas pobierania danych");
      } finally {
        setLoading(false);
      }
    };
    fetchFlockLosses();
  }, [filters]);

  const columnStats = useMemo(() => {
    if (flockLosses.length === 0) return {};

    const stats: {
      [key in keyof ProductionDataFlockLossListModel]?: {
        min: number;
        max: number;
        avg: number;
      };
    } = {};
    const keys: (keyof ProductionDataFlockLossListModel)[] = [
      "flockLoss1Percentage",
      "flockLoss2Percentage",
      "flockLoss3Percentage",
      "flockLoss4Percentage",
    ];

    for (const key of keys) {
      const values = flockLosses
        .map((row) => row[key] as number)
        .filter((v) => v !== null && !isNaN(v));
      if (values.length > 0) {
        const min = Math.min(...values);
        const max = Math.max(...values);
        const avg = values.reduce((a, b) => a + b, 0) / values.length;
        stats[key] = { min, max, avg };
      }
    }
    return stats;
  }, [flockLosses]);

  const columns = useMemo(
    () =>
      getFlockLossColumns({
        setSelectedFlockLoss,
        deleteFlockLoss,
        setIsEditModalOpen,
        columnStats,
        isAdmin,
      }),
    [columnStats, isAdmin]
  );

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
        <Typography variant="h4">Pomiary upadków i wybrakowań</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenModal(true)}
          >
            Dodaj nowy wpis
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getProductionDataFlockLossFiltersConfig(
          dictionary,
          uniqueCycles,
          filters
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={flockLosses}
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
              "productionDataFlockLossGridState",
              JSON.stringify(stateToSave)
            );
          }}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem(
              "productionDataFlockLossPageSize",
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
              ProductionDataFlockLossOrderType,
              mapProductionDataFlockLossOrderTypeToField
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

      <EditProductionDataFlockLossModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedFlockLoss(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedFlockLoss(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        flockLoss={selectedFlockLoss}
      />

      <AddProductionDataFlockLossModal
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

export default ProductionDataFlockLossPage;
