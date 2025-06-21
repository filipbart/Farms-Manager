import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useEffect, useMemo, useState } from "react";
import { FarmsService } from "../../../services/farms-service";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import AddFarmModal from "../../../components/modals/farms/add-farm-modal";

const FarmsPage: React.FC = () => {
  const [loading, setLoading] = useState(true);

  const [farms, setFarms] = useState<FarmRowModel[]>([]);

  const fetchFarms = async () => {
    try {
      const response = await FarmsService.getFarmsAsync();
      setFarms(response.responseData?.items ?? []);
    } catch (error) {
      console.error("Error fetching farms:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFarms();
  }, []);

  const columns: GridColDef[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
    {
      field: "henHousesCount",
      headerName: "Liczba kurników",
      flex: 1,
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia rekordu",
      type: "dateTime",
      flex: 1,
      valueGetter: (params: any) => {
        return params ? new Date(params) : null;
      },
    },
    {
      field: "actions",
      headerName: "Akcje",
      flex: 1,
      sortable: false,
      filterable: false,
      renderCell: (params: any) => (
        <div className="text-center">
          <Button variant="outlined">Edytuj</Button>
          <Button
            variant="outlined"
            color="error"
            sx={{ ml: 1 }}
            onClick={async () => {
              try {
                await FarmsService.deleteFarmAsync(params.row.id);
                await fetchFarms();
              } catch (e) {
                console.error("Nie udało się usunąć farmy", e);
              }
            }}
          >
            Usuń
          </Button>
        </div>
      ),
    },
  ];

  const [openModal, setOpenModal] = useState(false);

  const handleOpen = () => setOpenModal(true);
  const handleClose = async () => {
    setOpenModal(false);
  };

  const handleAddFarm = async () => {
    setOpenModal(false);
    await fetchFarms();
  };

  const data = useMemo(
    () => ({
      rows: farms,
      columns: columns,
    }),
    [farms]
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
        <Typography variant="h4">Fermy</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleOpen}>
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
        <DataGrid
          loading={loading}
          {...data}
          columns={columns}
          localeText={{
            noRowsLabel: "Brak wpisów",
          }}
          hideFooterPagination={true}
          rowSelection={false}
          showToolbar={false}
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: {
              display: "block",
            },
            [`& .${tablePaginationClasses.input}`]: {
              display: "inline-flex",
            },
            "& .MuiDataGrid-columnHeaders": {
              borderTop: "none",
            },
          }}
        />
      </Box>

      <AddFarmModal
        open={openModal}
        onClose={handleClose}
        onSave={handleAddFarm}
      />
    </Box>
  );
};

export default FarmsPage;
