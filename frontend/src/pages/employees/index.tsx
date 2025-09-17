import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import {
  EmployeesOrderType,
  filterReducer,
  initialFilters,
  mapEmployeeOrderTypeToField,
  type EmployeesDictionary,
} from "../../models/employees/employees-filters";
import { useEmployees } from "../../hooks/employees/useEmployees";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { EmployeesService } from "../../services/employees-service";
import { getEmployeesColumns } from "./employees-columns";
import FiltersForm from "../../components/filters/filters-form";
import { getEmployeesFiltersConfig } from "./filter-config.employees";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import { useNavigate } from "react-router-dom";
import AddEmployeeModal from "../../components/modals/employees/add-employee-modal";
import type { EmployeeListModel } from "../../models/employees/employees";
import {
  DataGridPremium,
  type GridRowParams,
  type GridState,
} from "@mui/x-data-grid-premium";
import {
  getSortOptionsFromGridModel,
  initializeFiltersFromLocalStorage,
} from "../../utils/grid-state-helper";

const EmployeesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) =>
      initializeFiltersFromLocalStorage(
        init,
        "employeesGridState",
        "employeesPageSize",
        EmployeesOrderType,
        mapEmployeeOrderTypeToField
      )
  );
  const [dictionary, setDictionary] = useState<EmployeesDictionary>();
  const [openAddEmployeeModal, setOpenAddEmployeeModal] = useState(false);

  const { employees, totalRows, loading, fetchEmployees } =
    useEmployees(filters);
  const nav = useNavigate();

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("employeesGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          columns: {
            columnVisibilityModel: { dateCreatedUtc: false },
          },
        };
  });

  const deleteEmployee = useCallback(
    async (id: string) => {
      await handleApiResponse(
        () => EmployeesService.deleteEmployee(id),
        () => {
          toast.success("Pracownik został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        },
        undefined,
        "Błąd podczas usuwania pracownika"
      );
    },
    [dispatch]
  );

  const getRowClassName = useCallback(
    (params: GridRowParams<EmployeeListModel>): string => {
      return params.row.upcomingDeadline ? "deadline-row" : "";
    },
    []
  );

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => EmployeesService.getDictionaries(),
          (data) => setDictionary(data.responseData),
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania słowników filtrów");
      }
    };
    fetchDictionaries();
  }, []);

  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees]);

  const columns = useMemo(
    () => getEmployeesColumns({ deleteEmployee }),
    [deleteEmployee]
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
        <Typography variant="h4">Kadry</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddEmployeeModal(true)}
          >
            Dodaj pracownika
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getEmployeesFiltersConfig(dictionary)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={employees}
          columns={columns}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
            };
            localStorage.setItem(
              "employeesGridState",
              JSON.stringify(stateToSave)
            );
          }}
          onRowClick={(params) => {
            nav(`/employees/${params.id}`);
          }}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem("employeesPageSize", pageSize.toString());

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
          getRowClassName={getRowClassName}
          sx={{
            "& .deadline-row .MuiDataGrid-cell": {
              backgroundColor: "#ffebf4",
            },
            "& .deadline-row:hover .MuiDataGrid-cell": {
              backgroundColor: "#ffcde2",
            },
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
              EmployeesOrderType,
              mapEmployeeOrderTypeToField
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

      <AddEmployeeModal
        open={openAddEmployeeModal}
        onClose={() => setOpenAddEmployeeModal(false)}
        onSave={() => {
          setOpenAddEmployeeModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default EmployeesPage;
