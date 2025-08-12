import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef, type GridRenderCellParams } from "@mui/x-data-grid";
import { useEffect, useMemo, useState } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { HatcheryRowModel } from "../../../models/hatcheries/hatchery-row-model";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import { HatcheriesService } from "../../../services/hatcheries-service";
import AddHatcheryModal from "../../../components/modals/hatcheries/add-hatchery-modal";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { toast } from "react-toastify";
import EditHatcheryModal from "../../../components/modals/hatcheries/edit-hatchery-modal";

const HatcheriesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [hatcheries, setHatcheries] = useState<HatcheryRowModel[]>([]);

  // Stany dla modali
  const [addModalOpen, setAddModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedHatchery, setSelectedHatchery] =
    useState<HatcheryRowModel | null>(null);

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

  const handleAddOpen = () => setAddModalOpen(true);
  const handleAddClose = () => setAddModalOpen(false);
  const handleAddSave = async () => {
    setAddModalOpen(false);
    await fetchHatcheries();
  };

  const handleEditOpen = (hatchery: HatcheryRowModel) => {
    setSelectedHatchery(hatchery);
    setEditModalOpen(true);
  };
  const handleEditClose = () => {
    setEditModalOpen(false);
    setSelectedHatchery(null);
  };
  const handleEditSave = async () => {
    setEditModalOpen(false);
    setSelectedHatchery(null);
    await fetchHatcheries();
  };

  const columns: GridColDef<HatcheryRowModel>[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "fullName", headerName: "Pełna nazwa", flex: 1 },
    { field: "producerNumber", headerName: "Numer producenta", flex: 1 },
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
      headerName: "Akcje",
      flex: 1,
      sortable: false,
      filterable: false,
      renderCell: (params: GridRenderCellParams<any, HatcheryRowModel>) => (
        <div className="text-center">
          <Button variant="outlined" onClick={() => handleEditOpen(params.row)}>
            Edytuj
          </Button>
          <Button
            variant="outlined"
            color="error"
            sx={{ ml: 1 }}
            onClick={async () => {
              await handleApiResponse(
                () => HatcheriesService.deleteHatcheryAsync(params.row.id),
                () => {
                  toast.success("Wylęgarnia została usunięta");
                  fetchHatcheries();
                },
                undefined,
                "Nie udało się usunąć wylęgarni"
              );
            }}
          >
            Usuń
          </Button>
        </div>
      ),
    },
  ];

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
          <Button variant="contained" color="primary" onClick={handleAddOpen}>
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
        <DataGridPro
          loading={loading}
          {...data}
          columns={columns}
          localeText={{
            noRowsLabel: "Brak wpisów",
            footerTotalRows: "Łączna liczba wierszy:",
          }}
          hideFooterPagination
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
        open={addModalOpen}
        onClose={handleAddClose}
        onSave={handleAddSave}
      />

      <EditHatcheryModal
        open={editModalOpen}
        onClose={handleEditClose}
        onSave={handleEditSave}
        hatcheryData={selectedHatchery}
      />
    </Box>
  );
};

export default HatcheriesPage;
