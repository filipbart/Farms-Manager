import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useEffect, useMemo, useState } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { HatcheryRowModel } from "../../../models/hatcheries/hatchery-row-model";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import { HatcheriesService } from "../../../services/hatcheries-service";
import AddHatcheryModal from "../../../components/modals/hatcheries/add-hatchery-modal";

const HatcheriesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [hatcheries, setHatcheries] = useState<HatcheryRowModel[]>([]);

  const fetchHatcheries = async () => {
    setLoading(true);
    await handleApiResponse<PaginateModel<HatcheryRowModel>>(
      () => HatcheriesService.getAllHatcheries(),
      (data) => {
        setHatcheries(data.responseData?.items ?? []);
      },
      undefined,
      "Nie udało się pobrać listy wylęgarń"
    );
    setLoading(false);
  };

  useEffect(() => {
    fetchHatcheries();
  }, []);

  const columns: GridColDef[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "fullName", headerName: "Pełna nazwa", flex: 1 },
    { field: "nip", headerName: "NIP", flex: 1 },
    { field: "address", headerName: "Adres", flex: 1 },
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
                () => HatcheriesService.deleteHatcheryAsync(params.row.id),
                undefined,
                undefined,
                "Nie udało się usunąć wylęgarni"
              );
              await fetchHatcheries();
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

  const handleAddHatchery = async () => {
    setOpenModal(false);
    await fetchHatcheries();
  };

  const data = useMemo(
    () => ({
      rows: hatcheries,
      columns: columns,
    }),
    [hatcheries]
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
        <Typography variant="h4">Wylęgarnie</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleOpen}>
            Dodaj wylęgarnię
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
            minHeight: hatcheries.length === 0 ? 300 : "auto",
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

      <AddHatcheryModal
        open={openModal}
        onClose={handleClose}
        onSave={handleAddHatchery}
      />
    </Box>
  );
};

export default HatcheriesPage;
