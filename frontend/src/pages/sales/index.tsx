import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { mapSaleOrderTypeToField } from "../../common/helpers/sale-order-type-helper";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
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

const SalesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<SalesDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [selectedSale, setSelectedSale] = useState<SaleListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [loadingExport, setLoadingExport] = useState(false);
  const { sales, totalRows, loading, refetch: fetchSales } = useSales(filters);
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelSales");
    return saved ? JSON.parse(saved) : {};
  });

  const [downloadDirectoryPath, setDownloadDirectoryPath] = useState<
    string | null
  >(null);

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

  const onClickExport = async () => {
    await downloadFile({
      url: ApiUrl.SaleExportFile,
      params: filters,
      defaultFilename: "sprzedaze",
      setLoading: setLoadingExport,
      successMessage: "Eksport zakończony sukcesem",
      errorMessage: "Błąd podczas eksportu sprzedaży",
      fileExtension: "xlsx",
    });
  };

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
      }),
    [dispatch, filters]
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
        config={getSaleFiltersConfig(dictionary, uniqueCycles, filters)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={sales}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelSales",
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
          slots={{
            toolbar: (props) => (
              <CustomToolbar
                {...props}
                withExport={true}
                onClickExport={onClickExport}
                loadingExport={loadingExport}
              />
            ),
            noRowsOverlay: NoRowsOverlay,
          }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
          scrollbarSize={17}
          sortingMode="server"
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(SalesOrderType).find(
                (orderType) => mapSaleOrderTypeToField(orderType) === sortField
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
