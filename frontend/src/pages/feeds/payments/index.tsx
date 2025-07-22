import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useState, useEffect, useMemo } from "react";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { getFeedsPaymentsColumns } from "./payments-columns";
import ApiUrl from "../../../common/ApiUrl";
import { downloadFile } from "../../../utils/download-file";
import { FileType } from "../../../models/files/file-type";
import { FilesService } from "../../../services/files-service";
import type { FileModel } from "../../../models/files/file-model";

const FeedsPaymentsPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [feedsPayments, setFeedsPayments] = useState<FileModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loadingFileName, setLoadingFileId] = useState<string | null>(null);

  const fetchFeedsPrices = async () => {
    try {
      setLoading(true);
      await handleApiResponse<PaginateModel<FileModel>>(
        () => FilesService.getFilesByType(FileType.FeedDeliveryPayment),
        (data) => {
          setFeedsPayments(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania faktur przelewów"
      );
    } catch {
      toast.error("Błąd podczas pobierania faktur przelewów");
    } finally {
      setLoading(false);
    }
  };

  const downloadPaymentFile = async (id: string) => {
    await downloadFile({
      url: ApiUrl.GetFile(id),
      params: { fileType: FileType.FeedDeliveryPayment },
      defaultFilename: "FakturaPrzelewu",
      setLoading: (value) => setLoadingFileId(value ? id : null),
      errorMessage: "Błąd podczas pobierania faktury przelewu",
    });
  };

  useEffect(() => {
    fetchFeedsPrices();
  }, []);

  const columns = useMemo(
    () =>
      getFeedsPaymentsColumns({
        downloadPaymentFile,
        loadingFileName,
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
        <Typography variant="h4">Dostawy pasz</Typography>
      </Box>

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={feedsPayments}
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
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
        />
      </Box>
    </Box>
  );
};

export default FeedsPaymentsPage;
