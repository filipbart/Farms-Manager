import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import NoRowsOverlay from "../../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../../components/datagrid/custom-toolbar";
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
import { getProductionDataWeighingsFiltersConfig } from "./filter-config.production-data-weighings";

const ProductionDataWeighingsTab: React.FC = () => {
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
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelWeighings");
    return saved ? JSON.parse(saved) : {};
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

  const deleteWeighing = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => ProductionDataWeighingsService.deleteWeighing(id),
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

  const columns = useMemo(
    () =>
      getWeighingsColumns({
        setSelectedWeighing,
        deleteWeighing,
        setIsEditModalOpen,
      }),
    []
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
        config={getProductionDataWeighingsFiltersConfig(
          dictionary,
          uniqueCycles,
          filters
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={weighings}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelWeighings",
              JSON.stringify(model)
            );
          }}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false },
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
              const foundOrderBy = Object.values(
                ProductionDataWeighingsOrderType
              ).find(
                (orderType) =>
                  mapProductionDataWeighingsOrderTypeToField(orderType) ===
                  sortField
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
