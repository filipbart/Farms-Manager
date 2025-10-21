import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro, type GridColDef } from "@mui/x-data-grid-pro";
import { useEffect, useState, useCallback } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { toast } from "react-toastify";
import { UtilizationPlantsService } from "../../../services/utilization-plants-service";
import type { UtilizationPlantRowModel } from "../../../models/utilization-plants/utilization-plants";
import { useUtilizationPlants } from "../../../hooks/useUtilizationPlants";
import AddUtilizationPlantModal from "../../../components/modals/utilization-plants/add-utilization-plant-modal";
import EditUtilizationPlantModal from "../../../components/modals/utilization-plants/edit-utilization-plant-modal";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { useAuth } from "../../../auth/useAuth";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import type { AuditFields } from "../../../common/interfaces/audit-fields";

const UtilizationPlantsPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [openAddModal, setOpenAddModal] = useState(false);
  const [openEditModal, setOpenEditModal] = useState(false);
  const [selectedPlant, setSelectedPlant] =
    useState<UtilizationPlantRowModel | null>(null);

  const {
    utilizationPlants,
    loadingUtilizationPlants: loading,
    fetchUtilizationPlants,
  } = useUtilizationPlants();

  useEffect(() => {
    fetchUtilizationPlants();
  }, [fetchUtilizationPlants]);

  const handleDelete = useCallback(
    async (id: string) => {
      await handleApiResponse(
        () => UtilizationPlantsService.deleteUtilizationPlantAsync(id),
        () => {
          toast.success("Pomyślnie usunięto zakład utylizacyjny");
          fetchUtilizationPlants();
        },
        undefined,
        "Nie udało się usunąć zakładu utylizacyjnego"
      );
    },
    [fetchUtilizationPlants]
  );

  const handleEditOpen = (plant: UtilizationPlantRowModel) => {
    setSelectedPlant(plant);
    setOpenEditModal(true);
  };

  const handleEditClose = () => {
    setOpenEditModal(false);
    setSelectedPlant(null);
  };

  const handleEditSave = () => {
    handleEditClose();
    fetchUtilizationPlants();
  };

  const handleAddClose = () => {
    setOpenAddModal(false);
  };

  const handleAddSave = () => {
    setOpenAddModal(false);
    fetchUtilizationPlants();
  };

  const baseColumns: GridColDef<UtilizationPlantRowModel & AuditFields>[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "irzNumber", headerName: "Numer IRZ", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia rekordu",
      type: "dateTime",
      flex: 1,
      valueGetter: (value: any) => (value ? new Date(value) : null),
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <ActionsCell
          key="actions"
          params={params}
          onEdit={handleEditOpen}
          onDelete={handleDelete}
        />,
      ],
    },
  ];
  
  const auditColumns = getAuditColumns<UtilizationPlantRowModel & AuditFields>(isAdmin);
  const columns: GridColDef<UtilizationPlantRowModel & AuditFields>[] = [...baseColumns, ...auditColumns];

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
        <Typography variant="h4">Zakłady utylizacyjne</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddModal(true)}
          >
            Dodaj zakład utylizacyjny
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
        <DataGridPro
          loading={loading}
          rows={utilizationPlants}
          columns={columns}
          localeText={{
            noRowsLabel: "Brak wpisów",
            footerTotalRows: "Łączna liczba wierszy:",
          }}
          hideFooterPagination
          rowSelection={false}
          sx={{
            minHeight: utilizationPlants.length === 0 ? 300 : "auto",
            "& .MuiDataGrid-columnHeaders": {
              borderTop: "none",
            },
            [`& .${tablePaginationClasses.selectLabel}`]: {
              display: "block",
            },
            [`& .${tablePaginationClasses.input}`]: {
              display: "inline-flex",
            },
          }}
        />
      </Box>

      <AddUtilizationPlantModal
        open={openAddModal}
        onClose={handleAddClose}
        onSave={handleAddSave}
      />

      <EditUtilizationPlantModal
        open={openEditModal}
        onClose={handleEditClose}
        onSave={handleEditSave}
        utilizationPlantData={selectedPlant}
      />
    </Box>
  );
};

export default UtilizationPlantsPage;
