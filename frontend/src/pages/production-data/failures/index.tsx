import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { ProductionDataFailureListModel } from "../../../models/production-data/failures";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { getProductionDataFailuresColumns } from "./failures-columns";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { ProductionDataFailuresService } from "../../../services/production-data/production-data-failures-service";
import AddProductionDataFailureModal from "../../../components/modals/production-data/failures/add-production-data-failure-modal";
import EditProductionDataFailureModal from "../../../components/modals/production-data/failures/edit-production-data-failure-modal";
import {
  filterReducer,
  initialFilters,
  mapProductionDataOrderTypeToField,
  ProductionDataOrderType,
  type ProductionDataDictionary,
} from "../../../models/production-data/production-data-filters";
import { getProductionDataFiltersConfig } from "../filter-config.production-data";
import FiltersForm from "../../../components/filters/filters-form";
import { ProductionDataService } from "../../../services/production-data/production-data-service";

const ProductionDataFailuresPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<ProductionDataDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [failures, setFailures] = useState<ProductionDataFailureListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);
  const [selectedFailure, setSelectedFailure] =
    useState<ProductionDataFailureListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelFailures");
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
          () => ProductionDataService.getDictionaries(),
          (data) => {
            console.log(data);
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

  const deleteFailure = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => ProductionDataFailuresService.deleteFailure(id),
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
    const fetchFailures = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ProductionDataFailuresService.getFailures(filters),
          (data) => {
            setFailures(data.responseData?.items ?? []);
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
    fetchFailures();
  }, [filters]);

  const columns = useMemo(
    () =>
      getProductionDataFailuresColumns({
        setSelectedFailure,
        deleteFailure,
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
        <Typography variant="h4">Upadki i Wybrakowania</Typography>
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
        config={getProductionDataFiltersConfig(
          dictionary,
          uniqueCycles,
          filters
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          rows={failures}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelFailures",
              JSON.stringify(model)
            );
          }}
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
          pagination
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
              const foundOrderBy = Object.values(ProductionDataOrderType).find(
                (orderType) =>
                  mapProductionDataOrderTypeToField(orderType) === sortField
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

      <EditProductionDataFailureModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedFailure(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedFailure(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        failure={selectedFailure}
      />

      <AddProductionDataFailureModal
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

export default ProductionDataFailuresPage;
