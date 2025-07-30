import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import FiltersForm from "../../../components/filters/filters-form";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import {
  filterReducer,
  initialFilters,
  mapProductionDataOrderTypeToField,
  ProductionDataOrderType,
  type ProductionDataDictionary,
} from "../../../models/production-data/production-data-filters";
import type { ProductionDataTransferFeedListModel } from "../../../models/production-data/transfer-feed";
import { ProductionDataService } from "../../../services/production-data/production-data-service";
import { getFeedTransfersColumns } from "./transfer-feed-columns";
import { getProductionDataFiltersConfig } from "../filter-config.production-data";
import { DataGrid } from "@mui/x-data-grid";
import { ProductionDataTransferFeedService } from "../../../services/production-data/production-data-transfer-feed-service";
import AddProductionDataTransferFeedModal from "../../../components/modals/production-data/transfer-feed/add-production-data-transfer-feed-modal";
import EditProductionDataTransferFeedModal from "../../../components/modals/production-data/transfer-feed/edit-production-data-transfer-feed-modal";

const ProductionDataTransferFeedPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<ProductionDataDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [feedTransfers, setFeedTransfers] = useState<
    ProductionDataTransferFeedListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [selectedFeedTransfer, setSelectedFeedTransfer] =
    useState<ProductionDataTransferFeedListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelFeedTransfers");
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

  const deleteFeedTransfer = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => ProductionDataTransferFeedService.deleteFeedTransfer(id),
        async () => {
          toast.success("Przeniesienie zostało poprawnie usunięte");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania przeniesienia"
      );
    } catch {
      toast.error("Błąd podczas usuwania przeniesienia");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const fetchFeedTransfers = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => ProductionDataTransferFeedService.getFeedTransfers(filters),
          (data) => {
            setFeedTransfers(data.responseData?.items ?? []);
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
    fetchFeedTransfers();
  }, [filters]);

  const { columns, columnGroupingModel } = useMemo(
    () =>
      getFeedTransfersColumns({
        setSelectedTransfer: setSelectedFeedTransfer,
        deleteTransfer: deleteFeedTransfer,
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
        <Typography variant="h4">Przeniesienia Paszy</Typography>
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
          rows={feedTransfers}
          columns={columns}
          columnGroupingModel={columnGroupingModel}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelFeedTransfers",
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

      <EditProductionDataTransferFeedModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedFeedTransfer(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedFeedTransfer(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        feedTransfer={selectedFeedTransfer}
      />

      <AddProductionDataTransferFeedModal
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

export default ProductionDataTransferFeedPage;
