import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { useReducer, useState, useMemo, useEffect } from "react";
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
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import { useNavigate } from "react-router-dom";
import AddEmployeeModal from "../../components/modals/employees/add-employee-modal";

const EmployeesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<EmployeesDictionary>();
  const [openAddEmployeeModal, setOpenAddEmployeeModal] = useState(false);

  const { employees, totalRows, loading, fetchEmployees } =
    useEmployees(filters);
  const nav = useNavigate();

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelEmployees");
    return saved ? JSON.parse(saved) : {};
  });

  const deleteEmployee = async (id: string) => {
    try {
      await handleApiResponse(
        () => EmployeesService.deleteEmployee(id),
        async () => {
          toast.success("Pracownik został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania pracownika"
      );
    } catch {
      toast.error("Błąd podczas usuwania pracownika");
    }
  };

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
    () =>
      getEmployeesColumns({
        deleteEmployee,
      }),
    []
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
        <DataGridPro
          loading={loading}
          rows={employees}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onRowClick={(params) => {
            nav(`/employees/${params.id}`);
          }}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelEmployees",
              JSON.stringify(model)
            );
          }}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false },
            },
          }}
          localeText={{
            paginationRowsPerPage: "Wierszy na stronę:",
            paginationDisplayedRows: ({ from, to, count }) =>
              `${from} do ${to} z ${count}`,
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
              const foundOrderBy = Object.values(EmployeesOrderType).find(
                (orderType) =>
                  mapEmployeeOrderTypeToField(orderType) === sortField
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
