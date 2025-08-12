import { useEffect, useMemo, useReducer, useState } from "react";
import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
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

const FeedsPaymentsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FeedsDictionary>();

  const [loading, setLoading] = useState(false);
  const [feedsPayments, setFeedsPayments] = useState<FeedPaymentListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);

  const [downloadFilePath, setDownloadFilePath] = useState<string | null>(null);

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
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath },
      defaultFilename: "Przelew",
      setLoading: (value) => setDownloadFilePath(value ? filePath : null),
      errorMessage: "Błąd podczas pobierania przelewu",
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
        <DataGridPro
          loading={loading}
          rows={feedsPayments}
          columns={columns}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false },
            },
          }}
          scrollbarSize={17}
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
              const foundOrderBy = Object.values(FeedsPaymentsOrderType).find(
                (orderType) =>
                  mapFeedsPaymentsOrderTypeToField(orderType) === sortField
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
    </Box>
  );
};

export default FeedsPaymentsPage;
