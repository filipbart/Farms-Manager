import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useEffect, useMemo, useState } from "react";
import { FarmsService } from "../../../services/farms-service";
import AddFarmModal from "../../../components/modals/farms/add-farm-modal";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { useFarms } from "../../../hooks/useFarms";

const FarmsPage: React.FC = () => {
  const { farms, loadingFarms, fetchFarms } = useFarms();

  const columns: GridColDef[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "producerNumber", headerName: "Numer producenta", flex: 1 },
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
              await handleApiResponse(
                () => FarmsService.deleteFarmAsync(params.row.id),
                undefined,
                undefined,
                "Nie udało się usunąć farmy"
              );
              await fetchFarms();
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

  useEffect(() => {
    fetchFarms();
  }, []);

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
          loading={loadingFarms}
          {...data}
          columns={columns}
          localeText={{
            noRowsLabel: "Brak wpisów",
          }}
          hideFooterPagination={true}
          rowSelection={false}
          showToolbar={false}
          sx={{
            minHeight: farms.length === 0 ? 300 : "auto",
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
