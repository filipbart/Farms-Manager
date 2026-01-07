import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import NoRowsOverlay from "../../../../components/datagrid/custom-norows";
import FiltersForm from "../../../../components/filters/filters-form";
import AddProductionDataWeighingModal from "../../../../components/modals/production-data/weighings/add-production-data-weighing-modal";
import EditProductionDataWeighingModal from "../../../../components/modals/production-data/weighings/edit-production-data-weighing-modal";
import type { CycleDictModel } from "../../../../models/common/dictionaries";
import type { ProductionDataWeighingListModel } from "../../../../models/production-data/weighings";
import {
  filterReducer,
  initialFilters,
  mapProductionDataWeighingsOrderTypeToField,
  ProductionDataWeighingsOrderType,
  type ProductionDataWeighingsDictionary,
} from "../../../../models/production-data/weighings-filters";
import { ProductionDataWeighingsService } from "../../../../services/production-data/production-data-weighings-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { getWeighingsColumns } from "./weighings-columns";
import { getWeighingsFiltersConfig } from "./filter-config.production-data-weighings";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
} from "@mui/x-data-grid-premium";
import { getSortOptionsFromGridModel } from "../../../../utils/grid-state-helper";
import { useAuth } from "../../../../auth/useAuth";

const ProductionDataWeighingsTab: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] =
    useState<ProductionDataWeighingsDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [weighings, setWeighings] = useState<ProductionDataWeighingListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);
  const [selectedWeighing, setSelectedWeighing] =
    useState<ProductionDataWeighingListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const initialGridState = {
    columns: {
      columnVisibilityModel: { dateCreatedUtc: false },
    },
  };

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

  useEffect(() => {
    const fetchWeighings = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ProductionDataWeighingsService.getWeighings(filters),
          (data) => {
            setWeighings(data.responseData?.items ?? []);
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
    fetchWeighings();
  }, [filters]);

  const columnStats = useMemo(() => {
    if (weighings.length === 0) return {};

    const stats: {
      [key in keyof ProductionDataWeighingListModel]?: {
        min: number;
        max: number;
        avg: number;
      };
    } = {};
    const keys: (keyof ProductionDataWeighingListModel)[] = [
      "weighing1Day",
      "weighing1Weight",
      "weighing2Day",
      "weighing2Weight",
      "weighing3Day",
      "weighing3Weight",
      "weighing4Day",
      "weighing4Weight",
      "weighing5Day",
      "weighing5Weight",
    ];

    for (const key of keys) {
      const values = weighings
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
  }, [weighings]);

  const columns = useMemo(
    () =>
      getWeighingsColumns({
        setSelectedWeighing,
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
        <Typography variant="h4">Ważenia</Typography>
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
        config={getWeighingsFiltersConfig(
          dictionary,
          uniqueCycles,
          filters,
          isAdmin
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={weighings}
          columns={columns}
          initialState={initialGridState}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
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
            "& .cell-good": {
              backgroundColor: "rgba(0, 255, 0, 0.1)",
            },
            "& .cell-bad": {
              backgroundColor: "rgba(255, 0, 0, 0.1)",
            },
            "& .cell-neutral": {
              backgroundColor: "rgba(255, 255, 0, 0.1)",
            },
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
              ProductionDataWeighingsOrderType,
              mapProductionDataWeighingsOrderTypeToField
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

      <EditProductionDataWeighingModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedWeighing(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedWeighing(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        weighing={selectedWeighing}
      />

      <AddProductionDataWeighingModal
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

export default ProductionDataWeighingsTab;
