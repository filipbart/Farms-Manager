import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  type GridDataSource,
} from "@mui/x-data-grid-premium";
import { useReducer, useState, useMemo, useEffect, useCallback } from "react";
import { toast } from "react-toastify";
import FiltersForm from "../../../components/filters/filters-form";
import type { EmployeePayslipListModel } from "../../../models/employees/employees-payslips";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { useEmployeePayslips } from "../../../hooks/employees/useEmployeePayslips";
import { EmployeePayslipsService } from "../../../services/employee-payslips-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { getEmployeePayslipColumns } from "./payslips-columns";
import CustomToolbar from "../../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import {
  EmployeePayslipsOrderType,
  filterReducer,
  initialFilters,
  mapEmployeePayslipOrderTypeToField,
  type EmployeePayslipsDictionary,
  type EmployeePayslipsFilterPaginationModel,
} from "../../../models/employees/employees-payslips-filters";
import { getEmployeePayslipsFiltersConfig } from "./filter-config.employee-payslips";
import AddEmployeePayslipModal from "../../../components/modals/employees/add-employee-payslip-modal";
import EditEmployeePayslipModal from "../../../components/modals/employees/edit-employee-payslip-modal";

const EmployeePayslipsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<EmployeePayslipsDictionary>();
  const [openAddPayslipModal, setOpenAddPayslipModal] = useState(false);
  const [selectedPayslip, setSelectedPayslip] =
    useState<EmployeePayslipListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const { totalRows, loading, fetchPayslips } = useEmployeePayslips(filters);

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelEmployeePayslips");
    return saved ? JSON.parse(saved) : {};
  });

  const dataSource: GridDataSource = useMemo(
    () => ({
      getRows: async (params) => {
        // Mapowanie sortowania
        let orderBy: EmployeePayslipsOrderType | undefined;
        let isDescending: boolean | undefined;
        if (params.sortModel.length > 0) {
          const sortField = params.sortModel[0].field;
          const foundOrderBy = Object.values(EmployeePayslipsOrderType).find(
            (orderType) =>
              mapEmployeePayslipOrderTypeToField(orderType) === sortField
          );
          orderBy = foundOrderBy;
          isDescending = params.sortModel[0].sort === "desc";
        }

        const filtersForApi: EmployeePayslipsFilterPaginationModel = {
          ...filters,
          page: params.paginationModel?.page,
          pageSize: params.paginationModel?.pageSize,
          aggregationModel: params.aggregationModel,
          orderBy,
          isDescending,
        };

        try {
          const response = await EmployeePayslipsService.getPayslips(
            filtersForApi
          );

          const responseData = response.responseData;
          return {
            rows: responseData?.list.items ?? [],
            rowCount: responseData?.list.totalRows ?? 0,
            aggregateRow: responseData?.aggregation,
          };
        } catch {
          toast.error("Nie udało się pobrać danych rozliczeń.");

          return { rows: [], rowCount: 0 };
        }
      },

      /**
       * Ta funkcja mówi siatce, jak odczytać wartość z obiektu `aggregateRow`.
       * Ponieważ Twoje API zwraca klucze pasujące do nazw pól (np. 'netPay'),
       * funkcja jest bardzo prosta.
       */
      getAggregatedValue: (row, field) => {
        return row[field];
      },
    }),
    [filters]
  );

  const aggregationFunctions = {
    sum: { columnTypes: ["number"] },
    avg: {},
    min: {},
    max: {},
    size: {},
  };

  const initialState = {
    pagination: {
      paginationModel: {
        page: 0,
        pageSize: 10,
      },
    },

    sorting: {
      sortModel: [{ field: "employeeFullName", sort: "asc" as "asc" | "desc" }],
    },

    aggregation: {
      model: {
        baseSalary: "sum",
        bankTransferAmount: "sum",
        bonusAmount: "sum",
        overtimePay: "sum",
        overtimeHours: "sum",
        deductions: "sum",
        otherAllowances: "sum",
        netPay: "sum",
      },
    },
  };

  const uniqueCycles = useMemo(() => {
    if (!dictionary?.cycles) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      map.set(key, cycle);
    }
    return Array.from(map.values());
  }, [dictionary]);

  const deletePayslip = async (id: string) => {
    try {
      await handleApiResponse(
        () => EmployeePayslipsService.deletePayslip(id),
        async () => {
          toast.success("Wpis wypłaty został poprawnie usunięty");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wpisu wypłaty"
      );
    } catch {
      toast.error("Błąd podczas usuwania wpisu wypłaty");
    }
  };

  const fetchDictionaries = useCallback(async () => {
    try {
      await handleApiResponse(
        () => EmployeePayslipsService.getDictionaries(),
        (data) => setDictionary(data.responseData),
        undefined,
        "Błąd podczas pobierania słowników filtrów"
      );
    } catch {
      toast.error("Błąd podczas pobierania słowników filtrów");
    }
  }, []);

  useEffect(() => {
    fetchDictionaries();
  }, [fetchDictionaries]);

  useEffect(() => {
    fetchPayslips();
  }, [fetchPayslips]);

  const columns = useMemo(
    () =>
      getEmployeePayslipColumns({
        setSelectedPayslip,
        deletePayslip,
        setIsEditModalOpen,
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
        <Typography variant="h4">Rozliczenie wypłat</Typography>
        <Box>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenAddPayslipModal(true)}
          >
            Dodaj rozliczenie
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getEmployeePayslipsFiltersConfig(dictionary, uniqueCycles)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          columns={columns}
          //rows={payslips}
          dataSource={dataSource}
          initialState={initialState}
          aggregationFunctions={aggregationFunctions}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelEmployeePayslips",
              JSON.stringify(model)
            );
          }}
          scrollbarSize={17}
          pagination
          paginationMode="server"
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
          slots={{
            toolbar: CustomToolbar,
            noRowsOverlay: NoRowsOverlay,
          }}
          showToolbar
          getRowClassName={(params) => {
            if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
              return "aggregated-row";
            }
            return "";
          }}
          sx={{
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
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(
                EmployeePayslipsOrderType
              ).find(
                (orderType) =>
                  mapEmployeePayslipOrderTypeToField(orderType) === sortField
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

      <EditEmployeePayslipModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedPayslip(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedPayslip(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        payslipToEdit={selectedPayslip}
      />

      <AddEmployeePayslipModal
        open={openAddPayslipModal}
        onClose={() => setOpenAddPayslipModal(false)}
        onSave={() => {
          setOpenAddPayslipModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default EmployeePayslipsPage;
