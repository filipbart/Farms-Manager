import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useReducer, useState, useMemo, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import { getUsersColumns } from "./users-columns";
import FiltersForm from "../../../components/filters/filters-form";
import { useUsers } from "../../../hooks/useUsers";
import {
  filterReducer,
  initialFilters,
  mapUserOrderTypeToField,
  UsersOrderType,
} from "../../../models/users/users-filters";
import { getUsersFiltersConfig } from "./filter-config.users";
import { UsersService } from "../../../services/users-service";
import AddUserModal from "../../../components/modals/users/add-user-modal";
import { DataGridPremium } from "@mui/x-data-grid-premium";
import { useAuth } from "../../../auth/useAuth";
import { getSortOptionsFromGridModel } from "../../../utils/grid-state-helper";

const UsersPage: React.FC = () => {
  const { userData } = useAuth();
  const isAdmin = userData?.isAdmin ?? false;
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [openAddUserModal, setOpenAddUserModal] = useState(false);

  const { users, totalRows, loading, fetchUsers } = useUsers(filters);
  const nav = useNavigate();

  const initialGridState = {
    columns: {
      columnVisibilityModel: { dateCreatedUtc: false },
    },
  };

  const deleteUser = useCallback(
    async (id: string) => {
      await handleApiResponse(
        () => UsersService.deleteUser(id),
        () => {
          toast.success("Użytkownik został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania użytkownika"
      );
    },
    [dispatch]
  );

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  const columns = useMemo(
    () => getUsersColumns({ deleteUser, isAdmin }),
    [deleteUser, isAdmin]
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
        <Typography variant="h4">Użytkownicy</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddUserModal(true)}
          >
            Dodaj użytkownika
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getUsersFiltersConfig(isAdmin)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={users}
          columns={columns}
          initialState={initialGridState}
          onRowClick={(params) => {
            nav(`/settings/users/${params.id}`);
          }}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            "& .MuiDataGrid-row:not(.deadline-row):hover .MuiDataGrid-cell": {
              backgroundColor: "#f5f5f5",
            },
            "& .MuiDataGrid-row:hover": {
              cursor: "pointer",
            },
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
          sortingMode="server"
          onSortModelChange={(model) => {
            const sortOptions = getSortOptionsFromGridModel(
              model,
              UsersOrderType,
              mapUserOrderTypeToField
            );
            const payload =
              model.length > 0
                ? { ...sortOptions, page: 0 }
                : { ...sortOptions };

            dispatch({
              type: "setMultiple",
              payload,
            });
          }}
        />
      </Box>

      <AddUserModal
        open={openAddUserModal}
        onClose={() => setOpenAddUserModal(false)}
        onSave={() => {
          setOpenAddUserModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default UsersPage;
