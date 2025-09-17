import { useEffect, useMemo, useReducer, useState } from "react";
import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import { FeedsService } from "../../../services/feeds-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { downloadFile } from "../../../utils/download-file";
import ApiUrl from "../../../common/ApiUrl";
import type { FeedPaymentListModel } from "../../../models/feeds/payments/payment";
import { getFeedsPaymentsColumns } from "./payments-columns";
import {
  FeedsPaymentsOrderType,
  filterReducer,
  initialFilters,
} from "../../../models/feeds/payments/payments-filters";
import { mapFeedsPaymentsOrderTypeToField } from "../../../common/helpers/feeds-payment-order-type-helper";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import FiltersForm from "../../../components/filters/filters-form";
import { getFeedsPaymentsFiltersConfig } from "./filter-config.feeds-payments";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../../utils/grid-state-helper";

const FeedsPaymentsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "feedPaymentsGridState",
        "feedsPaymentsPageSize",
        FeedsPaymentsOrderType,
        mapFeedsPaymentsOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

  const [loading, setLoading] = useState(false);
  const [feedsPayments, setFeedsPayments] = useState<FeedPaymentListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);

  const [downloadFilePath, setDownloadFilePath] = useState<string | null>(null);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("feedPaymentsGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const fetchFeedsPayments = async () => {
    try {
      setLoading(true);
      await handleApiResponse<PaginateModel<FeedPaymentListModel>>(
        () => FeedsService.getFeedsPayments(filters),
        (data) => {
          setFeedsPayments(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania przelewów"
      );
    } catch {
      toast.error("Błąd podczas pobierania przelewów");
    } finally {
      setLoading(false);
    }
  };

  const deleteFeedPayment = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => FeedsService.deleteFeedPayment(id),
        async () => {
          toast.success("Przelew został poprawnie usunięty");
          await fetchFeedsPayments();
        },
        undefined,
        "Błąd podczas usuwania przelewu"
      );
    } catch {
      toast.error("Błąd podczas usuwania przelewu");
    } finally {
      setLoading(false);
    }
  };

  const downloadPaymentFile = async (filePath: string) => {
    const lastDotIndex = filePath.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < filePath.length - 1
        ? filePath.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath },
      defaultFilename: "Przelew",
      setLoading: (value) => setDownloadFilePath(value ? filePath : null),
      errorMessage: "Błąd podczas pobierania przelewu",
      fileExtension: fileExtension,
    });
  };
  const fetchDictionaries = async () => {
    try {
      await handleApiResponse(
        () => FeedsService.getDictionaries(),
        (data) => setDictionary(data.responseData),
        undefined,
        "Błąd podczas pobierania słowników filtrów"
      );
    } catch {
      toast.error("Błąd podczas pobierania słowników filtrów");
    }
  };

  const columns = useMemo(
    () =>
      getFeedsPaymentsColumns({
        deleteFeedPayment,
        downloadPaymentFile,
        downloadFilePath,
      }),
    [downloadFilePath]
  );

  useEffect(() => {
    fetchDictionaries();
  }, []);

  useEffect(() => {
    fetchFeedsPayments();
  }, [filters]);

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
        <Typography variant="h4">Przelewy</Typography>
      </Box>

      <FiltersForm
        config={getFeedsPaymentsFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={feedsPayments}
          columns={columns}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
            };
            localStorage.setItem(
              "feedPaymentsGridState",
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
            localStorage.setItem("feedsPaymentsPageSize", pageSize.toString());

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
              FeedsPaymentsOrderType,
              mapFeedsPaymentsOrderTypeToField
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
    </Box>
  );
};

export default FeedsPaymentsPage;
