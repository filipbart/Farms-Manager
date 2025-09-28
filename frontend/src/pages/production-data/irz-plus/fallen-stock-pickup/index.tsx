import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useEffect, useMemo, useState, useCallback } from "react";
import { toast } from "react-toastify";
import type { FallenStockFilterModel } from "../../../../models/fallen-stocks/fallen-stocks-filters";
import type { FallenStockPickupRow } from "../../../../models/fallen-stocks/fallen-stock-pickups";
import { useFallenStockPickups } from "../../../../hooks/useFallenStockPickups";
import { FallenStockPickupService } from "../../../../services/production-data/fallen-stock-pickups-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { getFallenStockPickupColumns } from "./fallen-stock-pickup-columns";
import NoRowsOverlay from "../../../../components/datagrid/custom-norows";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridState,
} from "@mui/x-data-grid-premium";
import AddFallenStockPickupsModal from "../../../../components/modals/production-data/fallen-stocks/add-fallen-stock-pickups-modal";
import EditFallenStockPickupModal from "../../../../components/modals/production-data/fallen-stocks/edit-fallen-stock-pickup-modal";

interface FallenStocksPickupPageProps {
  filters: FallenStockFilterModel;
  onReloadMain: () => void;
}

const FallenStocksPickupPage: React.FC<FallenStocksPickupPageProps> = ({
  filters,
  onReloadMain,
}) => {
  const { pickups, loadingPickups, fetchPickups } =
    useFallenStockPickups(filters);

  const [openAddModal, setOpenAddModal] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedPickup, setSelectedPickup] = useState<
    FallenStockPickupRow | undefined
  >(undefined);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem(
      "productionDataFallenStockPickupGridState"
    );
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const [paginationModel, setPaginationModel] = useState(() => {
    const savedPageSize = localStorage.getItem(
      "productionDataFallenStockPickupPageSize"
    );
    return {
      page: 0,
      pageSize: savedPageSize ? parseInt(savedPageSize, 10) : 10,
    };
  });

  useEffect(() => {
    fetchPickups();
  }, [fetchPickups]);

  const deletePickupRecord = useCallback(
    async (id: string) => {
      await handleApiResponse(
        () => FallenStockPickupService.deleteFallenStockPickup(id),
        async () => {
          toast.success("Wpis odbioru został poprawnie usunięty");
          fetchPickups();
          onReloadMain();
        },
        undefined,
        "Błąd podczas usuwania wpisu odbioru"
      );
    },
    [fetchPickups]
  );

  const columns = useMemo(
    () =>
      getFallenStockPickupColumns({
        setSelectedPickup,
        deletePickupRecord,
        setIsEditModalOpen,
      }),
    [deletePickupRecord]
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
        <Typography variant="h5">Odbiory sztuk padłych</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddModal(true)}
            disabled={!filters.farmId || !filters.cycle}
          >
            Dodaj odbiór
          </Button>
        </Box>
      </Box>

      <Box
        mt={4}
        sx={{
          width: "100%",
          overflowX: "auto",
        }}
      >
        <DataGridPremium
          loading={loadingPickups}
          rows={pickups}
          columns={columns}
          slots={{ noRowsOverlay: NoRowsOverlay }}
          rowSelection={false}
          scrollbarSize={17}
          showToolbar
          pagination
          paginationModel={paginationModel}
          onPaginationModelChange={(newModel) => {
            localStorage.setItem(
              "productionDataFallenStockPickupPageSize",
              newModel.pageSize.toString()
            );
            setPaginationModel(newModel);
          }}
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
            localStorage.setItem(
              "productionDataFallenStockPickupGridState",
              JSON.stringify(stateToSave)
            );
          }}
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
        />
      </Box>

      <EditFallenStockPickupModal
        open={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSave={() => {
          setIsEditModalOpen(false);
          fetchPickups();
          onReloadMain();
        }}
        pickupToEdit={selectedPickup}
      />

      <AddFallenStockPickupsModal
        open={openAddModal}
        onClose={() => setOpenAddModal(false)}
        onSave={() => {
          setOpenAddModal(false);
          fetchPickups();
          onReloadMain();
        }}
        farmId={filters.farmId}
      />
    </Box>
  );
};

export default FallenStocksPickupPage;
