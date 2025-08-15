import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { useReducer, useState, useMemo, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
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

const UsersPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [openAddUserModal, setOpenAddUserModal] = useState(false);

  const { users, totalRows, loading, fetchUsers } = useUsers(filters);
  const nav = useNavigate();

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelUsers");
    return saved ? JSON.parse(saved) : {};
  });

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

  const columns = useMemo(() => getUsersColumns({ deleteUser }), [deleteUser]);

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
        config={getUsersFiltersConfig()}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          rows={users}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onRowClick={(params) => {
            nav(`/settings/users/${params.id}`);
          }}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelUsers",
              JSON.stringify(model)
            );
          }}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false },
            },
          }}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) =>
            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            })
          }
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
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
          }}
          sortingMode="server"
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(UsersOrderType).find(
                (orderType) => mapUserOrderTypeToField(orderType) === sortField
              );
              dispatch({
                type: "setMultiple",
                payload: {
                  orderBy: foundOrderBy,
                  isDescending: model[0].sort === "desc",
                  page: 0,
                },
              });
            } else {
              dispatch({
                type: "setMultiple",
                payload: { orderBy: undefined, isDescending: undefined },
              });
            }
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
