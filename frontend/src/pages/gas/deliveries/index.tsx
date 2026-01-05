import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { downloadFile } from "../../../utils/download-file";
import ApiUrl from "../../../common/ApiUrl";
import FiltersForm from "../../../components/filters/filters-form";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import type {
  DraftGasInvoice,
  GasDeliveryListModel,
} from "../../../models/gas/gas-deliveries";
import { getGasDeliveriesColumns } from "./gas-deliveries-columns";
import { getGasDeliveriesFiltersConfig } from "./filter-config.gas-deliveries";
import type { GasDeliveriesDictionary } from "../../../models/gas/gas-dictionaries";
import {
  filterReducer,
  GasDeliveriesOrderType,
  initialFilters,
  mapGasDeliveryOrderTypeToField,
} from "../../../models/gas/gas-deliveries-filters";
import { useGasDeliveries } from "../../../hooks/gas/useGasDeliveries";
import { GasService } from "../../../services/gas-service";
import AddGasDeliveryModal from "../../../components/modals/gas/deliveries/add-gas-delivery-modal";
import UploadGasInvoicesModal from "../../../components/modals/gas/deliveries/upload-gas-invoices-modal";
import SaveGasInvoicesModal from "../../../components/modals/gas/deliveries/save-gas-invoices-modal";
import EditGasDeliveryModal from "../../../components/modals/gas/deliveries/edit-gas-delivery-modal";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
} from "@mui/x-data-grid-premium";
import { getSortOptionsFromGridModel } from "../../../utils/grid-state-helper";
import { useAuth } from "../../../auth/useAuth";

const GasDeliveriesPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<GasDeliveriesDictionary>();
  const [openAddGasDeliveryModal, setOpenAddGasDeliveryModal] = useState(false);
  const [selectedGasDelivery, setSelectedGasDelivery] =
    useState<GasDeliveryListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [openSaveInvoicesModal, setOpenSaveInvoicesModal] = useState(false);
  const [openUploadInvoicesModal, setOpenUploadInvoicesModal] = useState(false);
  const [draftGasInvoices, setDraftGasInvoices] = useState<DraftGasInvoice[]>(
    []
  );

  const {
    gasDeliveries,
    totalRows,
    loading,
    refetch: fetchGasDeliveries,
  } = useGasDeliveries(filters);

  const initialGridState = {
    columns: {
      columnVisibilityModel: { dateCreatedUtc: false },
    },
  };

  const [downloadingFilePath, setDownloadFilePath] = useState<string | null>(
    null
  );

  const uploadFiles = (draftFiles: DraftGasInvoice[]) => {
    if (draftFiles.length === 0) {
      toast.error("Brak plików do przetworzenia");
      return;
    }
    setDraftGasInvoices(draftFiles);
    setOpenSaveInvoicesModal(true);
  };

  const deleteGasDelivery = async (id: string) => {
    try {
      await handleApiResponse(
        () => GasService.deleteGasDelivery(id),
        async () => {
          toast.success("Dostawa gazu została poprawnie usunięta");
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
          () => GasService.getDictionaries(),
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
    fetchGasDeliveries();
  }, [fetchGasDeliveries]);

  const downloadGasDeliveryFile = async (path: string) => {
    const lastDotIndex = path.lastIndexOf(".");
    const fileExtension =
      lastDotIndex !== -1 && lastDotIndex < path.length - 1
        ? path.substring(lastDotIndex + 1)
        : "pdf";
    await downloadFile({
      url: ApiUrl.GetFile,
      params: { filePath: path },
      defaultFilename: "FakturaGaz",
      setLoading: (value) => setDownloadFilePath(value ? path : null),
      errorMessage: "Błąd podczas pobierania faktury za gaz",
      fileExtension: fileExtension,
    });
  };

  const columns = useMemo(
    () =>
      getGasDeliveriesColumns({
        setSelectedGasDelivery,
        deleteGasDelivery,
        setIsEditModalOpen,
        downloadGasDeliveryFile,
        downloadingFilePath,
        isAdmin,
      }),
    [downloadingFilePath, isAdmin]
  );

  const handleCloseSaveInvoicesModal = () => {
    setDraftGasInvoices([]);
    setOpenSaveInvoicesModal(false);
    dispatch({ type: "setMultiple", payload: { page: 0 } });
  };

  const handleSaveInvoicesModal = (gasInvoiceData: DraftGasInvoice) => {
    const filteredInvoices = draftGasInvoices.filter(
      (t) => t.draftId !== gasInvoiceData.draftId
    );
    if (filteredInvoices.length === 0) {
      handleCloseSaveInvoicesModal();
    }
    setDraftGasInvoices(filteredInvoices);
  };

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
        <Typography variant="h4">Dostawy gazu</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddGasDeliveryModal(true)}
          >
            Dodaj fakturę ręcznie
          </Button>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenUploadInvoicesModal(true)}
          >
            Dodaj faktury automatycznie
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getGasDeliveriesFiltersConfig(dictionary, isAdmin)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={gasDeliveries}
          columns={columns}
          initialState={initialGridState}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
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
              GasDeliveriesOrderType,
              mapGasDeliveryOrderTypeToField
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

      {draftGasInvoices.length > 0 && (
        <SaveGasInvoicesModal
          open={openSaveInvoicesModal}
          onClose={handleCloseSaveInvoicesModal}
          onSave={handleSaveInvoicesModal}
          draftGasInvoices={draftGasInvoices}
        />
      )}

      <UploadGasInvoicesModal
        open={openUploadInvoicesModal}
        onClose={() => setOpenUploadInvoicesModal(false)}
        onUpload={uploadFiles}
      />

      <EditGasDeliveryModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedGasDelivery(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedGasDelivery(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        gasDelivery={selectedGasDelivery}
      />

      <AddGasDeliveryModal
        open={openAddGasDeliveryModal}
        onClose={() => setOpenAddGasDeliveryModal(false)}
        onSave={() => {
          setOpenAddGasDeliveryModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default GasDeliveriesPage;
