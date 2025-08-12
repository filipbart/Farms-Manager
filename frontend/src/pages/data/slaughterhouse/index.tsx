import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef, type GridRenderCellParams } from "@mui/x-data-grid";
import { useEffect, useMemo, useState } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import type { SlaughterhouseRowModel } from "../../../models/slaughterhouses/slaughterhouse-row-model";
import { SlaughterhousesService } from "../../../services/slaughterhouses-service";
import AddSlaughterhouseModal from "../../../components/modals/slaughterhouses/add-slaugtherhouse-modal";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { toast } from "react-toastify";
import EditSlaughterhouseModal from "../../../components/modals/slaughterhouses/edit-slaughterhouse-modal";

const SlaughterhousesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [slaughterhouses, setSlaughterhouses] = useState<
    SlaughterhouseRowModel[]
  >([]);

  const [addModalOpen, setAddModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedSlaughterhouse, setSelectedSlaughterhouse] =
    useState<SlaughterhouseRowModel | null>(null);

  const fetchSlaughterhouses = async () => {
    setLoading(true);
    await handleApiResponse<PaginateModel<SlaughterhouseRowModel>>(
      () => SlaughterhousesService.getAllSlaughterhouses(),
      (data) => {
        setSlaughterhouses(data.responseData?.items ?? []);
      },
      undefined,
      "Nie udało się pobrać listy ubojni"
    );
    setLoading(false);
  };

  useEffect(() => {
    fetchSlaughterhouses();
  }, []);

  const handleAddOpen = () => setAddModalOpen(true);
  const handleAddClose = () => setAddModalOpen(false);
  const handleAddSave = async () => {
    setAddModalOpen(false);
    await fetchSlaughterhouses();
  };

  const handleEditOpen = (slaughterhouse: SlaughterhouseRowModel) => {
    setSelectedSlaughterhouse(slaughterhouse);
    setEditModalOpen(true);
  };
  const handleEditClose = () => {
    setEditModalOpen(false);
    setSelectedSlaughterhouse(null);
  };
  const handleEditSave = async () => {
    setEditModalOpen(false);
    setSelectedSlaughterhouse(null);
    await fetchSlaughterhouses();
  };

  const columns: GridColDef<SlaughterhouseRowModel>[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
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
      renderCell: (
        params: GridRenderCellParams<any, SlaughterhouseRowModel>
      ) => (
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
                () =>
                  SlaughterhousesService.deleteSlaughterhouseAsync(
                    params.row.id
                  ),
                () => {
                  toast.success("Ubojnia została usunięta");
                  fetchSlaughterhouses();
                },
                undefined,
                "Nie udało się usunąć ubojni"
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
      rows: slaughterhouses,
      columns: columns,
    }),
    [slaughterhouses]
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
        <Typography variant="h4">Ubojnie</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleAddOpen}>
            Dodaj ubojnię
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
            minHeight: slaughterhouses.length === 0 ? 300 : "auto",
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

      <AddSlaughterhouseModal
        open={addModalOpen}
        onClose={handleAddClose}
        onSave={handleAddSave}
      />

      <EditSlaughterhouseModal
        open={editModalOpen}
        onClose={handleEditClose}
        onSave={handleEditSave}
        slaughterhouseData={selectedSlaughterhouse}
      />
    </Box>
  );
};

export default SlaughterhousesPage;
