import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { mapSaleOrderTypeToField } from "../../common/helpers/sale-order-type-helper";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import FiltersForm from "../../components/filters/filters-form";
import type { CycleDictModel } from "../../models/common/dictionaries";

import type { SalesDictionary } from "../../models/sales/sales-dictionary";
import {
  filterReducer,
  initialFilters,
  SalesOrderType,
} from "../../models/sales/sales-filters";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { getSaleFiltersConfig } from "./filter-config.sales";
import AddSaleModal from "../../components/modals/sales/add-sale-modal/add-sale-modal";
import { SalesService } from "../../services/sales-service";
import type { SaleListModel } from "../../models/sales/sales";
import EditSaleModal from "../../components/modals/sales/edit-sale-modal/edit-sale-modal";
import { getSalesColumns } from "./sales-columns";
import { useSales } from "../../hooks/sales/useSales";
import ApiUrl from "../../common/ApiUrl";
import { downloadFile } from "../../utils/download-file";
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

const SalesPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "salesGridState",
        "salesPageSize",
        SalesOrderType,
        mapSaleOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<SalesDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [selectedSale, setSelectedSale] = useState<SaleListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const { sales, totalRows, loading, refetch: fetchSales } = useSales(filters);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("salesGridState");
    if (savedState) {
      try {
        const parsed = JSON.parse(savedState);
        // Clear rowGrouping to prevent errors with undefined fields
        if (parsed.rowGrouping) {
          delete parsed.rowGrouping;
        }
        return parsed;
      } catch (e) {
        console.error("Failed to parse salesGridState", e);
        localStorage.removeItem("salesGridState");
      }
    }
    return {
      columns: {
        columnVisibilityModel: { dateCreatedUtc: false },
      },
    };
  });

  const [downloadDirectoryPath, setDownloadDirectoryPath] = useState<
    string | null
  >(null);

  const { transformedRows, uniqueExtraNames } = useMemo(() => {
    if (!sales || sales.length === 0) {
      return { transformedRows: [], uniqueExtraNames: [] };
    }

    const extraNamesSet = new Set<string>();
    sales.forEach((sale) => {
      sale.otherExtras?.forEach((extra) => extraNamesSet.add(extra.name));
    });
    const uniqueExtraNames = Array.from(extraNamesSet);

    const transformedRows = sales.map((sale) => {
      const newRow: Record<string, any> = { ...sale };
      uniqueExtraNames.forEach((extraName) => {
        const extra = sale.otherExtras?.find((e) => e.name === extraName);
        newRow[extraName] = extra ? extra.value : 0;
      });
      return newRow;
    });

    return { transformedRows, uniqueExtraNames };
  }, [sales]);

  const uniqueCycles = useMemo(() => {
    if (!dictionary?.cycles) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      map.set(key, cycle);
    }
    return Array.from(map.values());
  }, [dictionary]);

  const deleteSale = async (id: string) => {
    try {
      await handleApiResponse(
        () => SalesService.deleteSale(id),
        async () => {
          toast.success("Wstawienie zostało poprawnie usunięte");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wstawienia"
      );
    } catch {
      toast.error("Błąd podczas usuwania wstawienia");
    }
  };

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => SalesService.getDictionaries(),
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
    fetchSales();
  }, [fetchSales]);

  const downloadSaleDirectory = async (path: string) => {
    await downloadFile({
      url: ApiUrl.SaleDownloadZip,
      params: { path },
      defaultFilename: "Przelew",
      setLoading: (value) => setDownloadDirectoryPath(value ? path : null),
      errorMessage: "Błąd podczas folderu dokumentów sprzedaży",
    });
  };

  const columns = useMemo(
    () =>
      getSalesColumns({
        setSelectedSale,
        deleteSale,
        setIsEditModalOpen,
        downloadSaleDirectory,
        downloadDirectoryPath,
        dispatch,
        filters,
        uniqueExtraNames,
        isAdmin,
      }),
    [uniqueExtraNames, downloadDirectoryPath, isAdmin]
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
        <Typography variant="h4">Sprzedaże</Typography>
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
        config={getSaleFiltersConfig(
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
          rows={transformedRows}
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
            localStorage.setItem("salesGridState", JSON.stringify(stateToSave));
          }}
          pagination
          paginationMode="server"
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem("salesPageSize", pageSize.toString());

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{
            noRowsOverlay: NoRowsOverlay,
          }}
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
          scrollbarSize={17}
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              SalesOrderType,
              mapSaleOrderTypeToField
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

      <EditSaleModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedSale(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedSale(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        sale={selectedSale}
      />

      <AddSaleModal
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

export default SalesPage;
