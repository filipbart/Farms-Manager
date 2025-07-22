import { useEffect, useMemo, useReducer, useState } from "react";
import {
  filterReducer,
  initialFilters,
} from "../../../models/feeds/corrections/corrections-filters";
import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { toast } from "react-toastify";
import { mapFeedsPricesOrderTypeToField } from "../../../common/helpers/feeds-price-order-type-helper";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import { FeedsService } from "../../../services/feeds-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { FeedCorrectionListModel } from "../../../models/feeds/corrections/correction";
import { getFeedsCorrectionsColumns } from "./corrections-columns";
import { downloadFile } from "../../../utils/download-file";
import ApiUrl from "../../../common/ApiUrl";
import { FileType } from "../../../models/files/file-type";

const FeedsCorrectionsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);

  const [loading, setLoading] = useState(false);
  const [feedsCorrections, setFeedsCorrections] = useState<
    FeedCorrectionListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);

  const [downloadFilePath, setDownloadFilePath] = useState<string | null>(null);

  const fetchFeedsCorrections = async () => {
    try {
      setLoading(true);
      await handleApiResponse<PaginateModel<FeedCorrectionListModel>>(
        () => FeedsService.getFeedsCorrections(filters),
        (data) => {
          setFeedsCorrections(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania korekt faktur"
      );
    } catch {
      toast.error("Błąd podczas pobierania korekt faktur");
    } finally {
      setLoading(false);
    }
  };

  const downloadCorrectionFile = async (filePath: string) => {
    const cleanedPath = filePath.substring(filePath.indexOf("/") + 1);

    await downloadFile({
      url: ApiUrl.GetFile(cleanedPath),
      params: { fileType: FileType.FeedDeliveryCorrection },
      defaultFilename: "FakturaKorekty",
      setLoading: (value) => setDownloadFilePath(value ? cleanedPath : null),
      errorMessage: "Błąd podczas pobierania faktury korekty",
    });
  };

  const columns = useMemo(
    () =>
      getFeedsCorrectionsColumns({
        downloadCorrectionFile,
        downloadFilePath,
      }),
    [downloadFilePath]
  );

  useEffect(() => {
    fetchFeedsCorrections();
  }, [filters]);

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
        <Typography variant="h4">Korekty</Typography>
      </Box>

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={feedsCorrections}
          columns={columns}
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
              const foundOrderBy = Object.values(FeedsCorrectionsPage).find(
                (orderType) =>
                  mapFeedsPricesOrderTypeToField(orderType) === sortField
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

export default FeedsCorrectionsPage;
