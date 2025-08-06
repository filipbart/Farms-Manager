import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import FiltersForm from "../../../components/filters/filters-form";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import type { GasConsumptionsDictionary } from "../../../models/gas/gas-dictionaries";
import type { GasConsumptionListModel } from "../../../models/gas/gas-consumptions";
import { useGasConsumptions } from "../../../hooks/gas/useGasConsumptions";
import {
  filterReducer,
  GasConsumptionsOrderType,
  initialFilters,
  mapGasConsumptionOrderTypeToField,
} from "../../../models/gas/gas-consumptions-filters";
import { GasService } from "../../../services/gas-service";
import { getGasConsumptionsColumns } from "./gas-consumptions-columns";
import { getGasConsumptionsFiltersConfig } from "./filter-config.gas-consumptions";
import AddGasConsumptionModal from "../../../components/modals/gas/consumptions/add-gas-consumption-modal";
import EditGasConsumptionModal from "../../../components/modals/gas/consumptions/edit-gas-consumption-modal";

const GasConsumptionsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<GasConsumptionsDictionary>();
  const [openAddModal, setOpenAddModal] = useState(false);
  const [selectedGasConsumption, setSelectedGasConsumption] =
    useState<GasConsumptionListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const {
    gasConsumptions,
    totalRows,
    loading,
    refetch: fetchGasConsumptions,
  } = useGasConsumptions(filters);

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelGasConsumptions");
    return saved ? JSON.parse(saved) : {};
  });

  const uniqueCycles = useMemo(() => {
    if (!dictionary?.cycles) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      map.set(key, cycle);
    }
    return Array.from(map.values());
  }, [dictionary]);

  const deleteGasConsumption = async (id: string) => {
    try {
      await handleApiResponse(
        () => GasService.deleteGasConsumption(id),
        async () => {
          toast.success("Wpis zużycia gazu został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wpisu"
      );
    } catch {
      toast.error("Błąd podczas usuwania wpisu");
    }
  };

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => GasService.getConsumptionsDictionaries(),
          (data) => setDictionary(data.responseData),
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
    fetchGasConsumptions();
  }, [fetchGasConsumptions]);

  const columns = useMemo(
    () =>
      getGasConsumptionsColumns({
        setSelectedGasConsumption,
        deleteGasConsumption,
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
        <Typography variant="h4">Zużycie gazu</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddModal(true)}
          >
            Dodaj nowy wpis
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getGasConsumptionsFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          rows={gasConsumptions}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelGasConsumptions",
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
              const foundOrderBy = Object.values(GasConsumptionsOrderType).find(
                (orderType) =>
                  mapGasConsumptionOrderTypeToField(orderType) === sortField
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

      <EditGasConsumptionModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedGasConsumption(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedGasConsumption(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        gasConsumption={selectedGasConsumption}
      />

      <AddGasConsumptionModal
        open={openAddModal}
        onClose={() => setOpenAddModal(false)}
        onSave={() => {
          setOpenAddModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default GasConsumptionsPage;
