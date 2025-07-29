import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import FiltersForm from "../../../components/filters/filters-form";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import type { ProductionDataRemainingFeedListModel } from "../../../models/production-data/remaining-feed";
import {
  filterReducer,
  initialFilters,
  mapProductionDataOrderTypeToField,
  ProductionDataOrderType,
  type ProductionDataDictionary,
} from "../../../models/production-data/production-data-filters";
import { ProductionDataRemainingFeedService } from "../../../services/production-data/production-data-remaining-feed-service";
import { getRemainingFeedColumns } from "./remaining-feed-columns";
import { getProductionDataFiltersConfig } from "../filter-config.production-data";
import AddProductionDataRemainingFeedModal from "../../../components/modals/production-data/remaining-feed/add-production-data-remaining-feed-modal";
import EditProductionDataRemainingFeedModal from "../../../components/modals/production-data/remaining-feed/edit-production-data-failure-modal";
import { ProductionDataService } from "../../../services/production-data/production-data-service";

const ProductionDataRemainingFeedPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<ProductionDataDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [remainingFeeds, setRemainingFeeds] = useState<
    ProductionDataRemainingFeedListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [selectedRemainingFeed, setSelectedRemainingFeed] =
    useState<ProductionDataRemainingFeedListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelRemainingFeeds");
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

  const deleteRemainingFeed = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => ProductionDataRemainingFeedService.deleteRemainingFeed(id),
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
    const fetchRemainingFeeds = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ProductionDataRemainingFeedService.getRemainingFeeds(filters),
          (data) => {
            setRemainingFeeds(data.responseData?.items ?? []);
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
    fetchRemainingFeeds();
  }, [filters]);

  const columns = useMemo(
    () =>
      getRemainingFeedColumns({
        setSelectedRemainingFeed,
        deleteRemainingFeed,
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
        <Typography variant="h4">Pozostała Pasza</Typography>
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
        <DataGrid
          loading={loading}
          rows={remainingFeeds}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelRemainingFeeds",
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

      <EditProductionDataRemainingFeedModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedRemainingFeed(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedRemainingFeed(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        remainingFeed={selectedRemainingFeed}
      />

      <AddProductionDataRemainingFeedModal
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

export default ProductionDataRemainingFeedPage;
