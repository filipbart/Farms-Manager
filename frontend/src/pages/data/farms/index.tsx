import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import { FarmsService } from "../../../services/farms-service";
import AddFarmModal from "../../../components/modals/farms/add-farm-modal";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { useFarms } from "../../../hooks/useFarms";
import { DataGridPro } from "@mui/x-data-grid-pro";
import ActionsCell from "../../../components/datagrid/actions-cell";
import EditFarmModal from "../../../components/modals/farms/edit-farm-modal";

type FarmRow = {
  id: string;
  name: string;
  nip: string;
  producerNumber: string;
  address: string;
};

const FarmsPage: React.FC = () => {
  const { farms, loadingFarms, fetchFarms } = useFarms();

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedFarm, setSelectedFarm] = useState<FarmRow | null>(null);

  const handleEditOpen = (farm: FarmRow) => {
    setSelectedFarm(farm);
    setIsEditModalOpen(true);
  };

  const handleEditClose = () => {
    setIsEditModalOpen(false);
    setSelectedFarm(null);
  };

  const handleUpdateFarm = async () => {
    handleEditClose();
    await fetchFarms();
  };

  const handleDelete = async (id: string) => {
    await handleApiResponse(
      () => FarmsService.deleteFarmAsync(id),
      async () => {
        await fetchFarms();
      },
      undefined,
      "Nie udało się usunąć farmy"
    );
  };

  const columns: GridColDef<FarmRow>[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "producerNumber", headerName: "Numer producenta", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
    { field: "henHousesCount", headerName: "Liczba kurników", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia rekordu",
      type: "dateTime",
      flex: 1,
      valueGetter: (params: { value: string | Date }) => {
        return params.value ? new Date(params.value) : null;
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 150,
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

  const handleAddOpen = () => setIsAddModalOpen(true);
  const handleAddClose = () => setIsAddModalOpen(false);

  const handleAddFarm = async () => {
    setIsAddModalOpen(false);
    await fetchFarms();
  };

  useEffect(() => {
    fetchFarms();
  }, [fetchFarms]);

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
        <Typography variant="h4">Fermy</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleAddOpen}>
            Dodaj fermę
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
          loading={loadingFarms}
          rows={farms}
          columns={columns}
          localeText={{
            noRowsLabel: "Brak wpisów",
            footerTotalRows: "Łączna liczba wierszy:",
          }}
          hideFooterPagination
          rowSelection={false}
          sx={{
            minHeight: farms.length === 0 ? 300 : "auto",
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
            "& .aggregated-row": {
              fontWeight: "bold",

              "& .MuiDataGrid-cell": {
                borderTop: "1px solid rgba(224, 224, 224, 1)",
                backgroundColor: "rgba(240, 240, 240, 0.7)",
              },
            },
            "& .MuiDataGrid-columnHeaders": { borderTop: "none" },
          }}
        />
      </Box>

      <AddFarmModal
        open={isAddModalOpen}
        onClose={handleAddClose}
        onSave={handleAddFarm}
      />

      <EditFarmModal
        open={isEditModalOpen}
        onClose={handleEditClose}
        onSave={handleUpdateFarm}
        farmData={selectedFarm}
      />
    </Box>
  );
};

export default FarmsPage;
