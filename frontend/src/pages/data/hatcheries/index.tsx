import { Box, Button, Checkbox, FormControlLabel, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef, DataGridPro } from "@mui/x-data-grid-pro";
import { useEffect, useState } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { HatcheryRowModel } from "../../../models/hatcheries/hatchery-row-model";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import { HatcheriesService } from "../../../services/hatcheries-service";
import AddHatcheryModal from "../../../components/modals/hatcheries/add-hatchery-modal";
import { toast } from "react-toastify";
import EditHatcheryModal from "../../../components/modals/hatcheries/edit-hatchery-modal";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { useAuth } from "../../../auth/useAuth";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import type { AuditFields } from "../../../common/interfaces/audit-fields";

const HatcheriesPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [loading, setLoading] = useState(false);
  const [showDeleted, setShowDeleted] = useState(false);
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

  // --- Obsługa modali ---
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

  const deleteHatchery = async (id: string) => {
    await handleApiResponse(
      () => HatcheriesService.deleteHatcheryAsync(id),
      () => {
        toast.success("Wylęgarnia została usunięta");
        fetchHatcheries();
      },
      undefined,
      "Nie udało się usunąć wylęgarni"
    );
  };

  const baseColumns: GridColDef<HatcheryRowModel & AuditFields>[] = [
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
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      cellClassName: "actions",
      getActions: (params) => {
        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={handleEditOpen}
            onDelete={deleteHatchery}
          />,
        ];
      },
    },
  ];
  
  const auditColumns = getAuditColumns<HatcheryRowModel & AuditFields>(isAdmin);
  const columns: GridColDef<HatcheryRowModel & AuditFields>[] = [...baseColumns, ...auditColumns];

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

      {isAdmin && (
        <Box mb={2}>
          <FormControlLabel
            control={
              <Checkbox
                checked={showDeleted}
                onChange={(e) => setShowDeleted(e.target.checked)}
              />
            }
            label="Pokaż usunięte"
          />
        </Box>
      )}

      <Box
        mt={4}
        sx={{
          width: "100%",
          overflowX: "auto",
        }}
      >
        <DataGridPro
          loading={loading}
          rows={hatcheries}
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
