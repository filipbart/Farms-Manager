import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef } from "@mui/x-data-grid";
import { useEffect, useState, useMemo, useCallback } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { DataGridPro } from "@mui/x-data-grid-pro";
import ActionsCell from "../../../components/datagrid/actions-cell";
import { useAuth } from "../../../auth/useAuth";
import { getAuditColumns } from "../../../utils/audit-columns-helper";
import type { TaxBusinessEntityRowModel } from "../../../models/data/tax-business-entity";
import { TaxBusinessEntitiesService } from "../../../services/tax-business-entities-service";
import AddTaxBusinessEntityModal from "../../../components/modals/data/add-tax-business-entity-modal";
import EditTaxBusinessEntityModal from "../../../components/modals/data/edit-tax-business-entity-modal";
import type { PaginateModel } from "../../../common/interfaces/paginate";

const TaxBusinessEntitiesPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [entities, setEntities] = useState<TaxBusinessEntityRowModel[]>([]);
  const [loading, setLoading] = useState(false);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedEntity, setSelectedEntity] =
    useState<TaxBusinessEntityRowModel | null>(null);

  const fetchEntities = useCallback(async () => {
    setLoading(true);
    await handleApiResponse<PaginateModel<TaxBusinessEntityRowModel>>(
      () => TaxBusinessEntitiesService.getAllAsync(),
      (data) => {
        setEntities(data.responseData?.items ?? []);
      },
      undefined,
      "Nie udało się pobrać listy podmiotów",
    );
    setLoading(false);
  }, []);

  const handleEditOpen = (entity: TaxBusinessEntityRowModel) => {
    setSelectedEntity(entity);
    setIsEditModalOpen(true);
  };

  const handleEditClose = () => {
    setIsEditModalOpen(false);
    setSelectedEntity(null);
  };

  const handleUpdateEntity = async () => {
    handleEditClose();
    await fetchEntities();
  };

  const handleDelete = async (id: string) => {
    await handleApiResponse(
      () => TaxBusinessEntitiesService.deleteAsync(id),
      async () => {
        await fetchEntities();
      },
      undefined,
      "Nie udało się usunąć podmiotu",
    );
  };

  const columns: GridColDef<TaxBusinessEntityRowModel>[] = useMemo(() => {
    const baseColumns: GridColDef<TaxBusinessEntityRowModel>[] = [
      { field: "nip", headerName: "NIP", width: 130 },
      { field: "name", headerName: "Nazwa", flex: 1 },
      { field: "businessType", headerName: "Typ działalności", flex: 1 },
      { field: "description", headerName: "Opis", flex: 1 },
      {
        field: "hasKSeFToken",
        headerName: "Token KSeF",
        width: 120,
        renderCell: (params) => (params.value ? "✓ Skonfigurowany" : "—"),
      },
      {
        field: "dateCreatedUtc",
        headerName: "Data utworzenia",
        type: "dateTime",
        width: 170,
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
    const auditColumns = getAuditColumns<TaxBusinessEntityRowModel>(isAdmin);
    return [...baseColumns, ...auditColumns];
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin]);

  const handleAddOpen = () => setIsAddModalOpen(true);
  const handleAddClose = () => setIsAddModalOpen(false);

  const handleAddEntity = async () => {
    setIsAddModalOpen(false);
    await fetchEntities();
  };

  useEffect(() => {
    fetchEntities();
  }, [fetchEntities]);

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
        <Typography variant="h4">Podmioty gospodarcze</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleAddOpen}>
            Dodaj podmiot
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
          rows={entities}
          columns={columns}
          initialState={{
            sorting: {
              sortModel: [{ field: "dateCreatedUtc", sort: "desc" }],
            },
          }}
          localeText={{
            noRowsLabel: "Brak wpisów",
            footerTotalRows: "Łączna liczba wierszy:",
          }}
          hideFooterPagination
          rowSelection={false}
          sx={{
            minHeight: entities.length === 0 ? 300 : "auto",
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

      <AddTaxBusinessEntityModal
        open={isAddModalOpen}
        onClose={handleAddClose}
        onSave={handleAddEntity}
      />

      <EditTaxBusinessEntityModal
        open={isEditModalOpen}
        onClose={handleEditClose}
        onSave={handleUpdateEntity}
        entityData={selectedEntity}
      />
    </Box>
  );
};

export default TaxBusinessEntitiesPage;
