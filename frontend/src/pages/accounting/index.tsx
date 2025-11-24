import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../models/common/dictionaries";
import {
  DataGridPremium,
  type GridState,
  useGridApiRef,
} from "@mui/x-data-grid-premium";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import {
  filterReducer,
  initialFilters,
  type AnalysisDictionary,
} from "../../models/summary/analysis-filters";
import FiltersForm from "../../components/filters/filters-form";
import { getSummaryAnalysisFiltersConfig } from "../summary/filter-config.summary-analysis";

const AccountingPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) => {
      const savedPageSize = localStorage.getItem("accountingPageSize");
      return {
        ...init,
        pageSize: savedPageSize
          ? parseInt(savedPageSize, 10)
          : init.pageSize ?? 10,
      };
    }
  );
  const [dictionary, setDictionary] = useState<AnalysisDictionary>();
  const [loading, setLoading] = useState(false);
  const [accountingData, setAccountingData] = useState<any[]>([]);
  const [totalRows, setTotalRows] = useState(0);

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("accountingGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          pinnedColumns: {
            left: ["cycleText", "farmName"],
          },
        };
  });

  const apiRef = useGridApiRef();

  const uniqueCycles = useMemo(() => {
    if (!dictionary) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) {
        map.set(key, cycle);
      }
    }
    return Array.from(map.values());
  }, [dictionary]);

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        // TODO: Replace with actual accounting service when available
        setDictionary({
          cycles: [],
          farms: [],
          hatcheries: [],
        });
      } catch {
        toast.error("Błąd podczas pobierania danych inicjalizujących.");
      }
    };
    fetchInitialData();
  }, []);

  useEffect(() => {
    const fetchAccountingData = async () => {
      // Don't fetch data if no farms selected
      if (!filters.farmIds || filters.farmIds.length === 0) {
        setAccountingData([]);
        setTotalRows(0);
        return;
      }

      setLoading(true);
      try {
        // TODO: Replace with actual accounting service when available
        setAccountingData([]);
        setTotalRows(0);
      } catch {
        toast.error("Błąd podczas pobierania danych księgowych");
      } finally {
        setLoading(false);
      }
    };
    fetchAccountingData();
  }, [filters]);

  const columns = useMemo(
    () => [
      {
        field: "id",
        headerName: "ID",
        width: 100,
      } as const,
      {
        field: "description",
        headerName: "Opis",
        width: 300,
        flex: 1,
      } as const,
      {
        field: "amount",
        headerName: "Kwota",
        width: 150,
        type: "number" as const,
      },
      {
        field: "date",
        headerName: "Data",
        width: 150,
      } as const,
    ],
    []
  );

  return (
    <Box p={4}>
      <Box mb={2}>
        <Typography variant="h4">Księgowość</Typography>
      </Box>

      <FiltersForm
        config={getSummaryAnalysisFiltersConfig(
          dictionary,
          uniqueCycles,
          filters
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          apiRef={apiRef}
          loading={loading}
          rows={accountingData}
          columns={columns}
          scrollbarSize={17}
          initialState={initialGridState}
          onStateChange={(newState: GridState) => {
            const stateToSave = {
              columns: newState.columns,
              sorting: newState.sorting,
              filter: newState.filter,
              aggregation: newState.aggregation,
              pinnedColumns: newState.pinnedColumns,
              rowGrouping: newState.rowGrouping,
            };
            localStorage.setItem(
              "accountingGridState",
              JSON.stringify(stateToSave)
            );
          }}
          paginationMode="server"
          pagination
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: filters.page ?? 0,
          }}
          onPaginationModelChange={({ page, pageSize }) => {
            localStorage.setItem("accountingPageSize", pageSize.toString());

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{
            noRowsOverlay: NoRowsOverlay,
          }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
        />
      </Box>
    </Box>
  );
};

export default AccountingPage;
