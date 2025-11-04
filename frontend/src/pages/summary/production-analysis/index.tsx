import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import {
  filterReducer,
  initialFilters,
  type AnalysisDictionary,
} from "../../../models/summary/analysis-filters";
import type { ProductionAnalysisRowModel, ProductionAnalysisSummaryModel } from "../../../models/summary/production-analysis";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { SummaryService } from "../../../services/summary-service";
import {
  DataGridPremium,
  GRID_AGGREGATION_ROOT_FOOTER_ROW_ID,
  useGridApiRef,
  type GridState,
  type GridToolbarProps,
} from "@mui/x-data-grid-premium";
import { getProductionAnalysisColumns } from "./production-analysis-columns";
import FiltersForm from "../../../components/filters/filters-form";
import { getSummaryAnalysisFiltersConfig } from "../filter-config.summary-analysis";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbarAnalysis from "../../../components/datagrid/custom-toolbar-analysis";
import {
  ColumnsViewsService,
  ColumnViewType,
  type ColumnViewRow,
} from "../../../services/columns-views-service";

const SummaryProductionAnalysisPage: React.FC = () => {
  const [filters, dispatch] = useReducer(
    filterReducer,
    initialFilters,
    (init) => {
      const savedPageSize = localStorage.getItem(
        "summaryProductionAnalysisPageSize"
      );
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
  const [analysisData, setAnalysisData] = useState<
    ProductionAnalysisRowModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [summaryData, setSummaryData] = useState<ProductionAnalysisSummaryModel | null>(null);

  const apiRef = useGridApiRef();
  const [savedViews, setSavedViews] = useState<ColumnViewRow[]>([]);
  const [selectedView, setSelectedView] = useState<string>("");

  const [initialGridState] = useState(() => {
    const savedState = localStorage.getItem("productionAnalysisGridState");
    return savedState
      ? JSON.parse(savedState)
      : {
          pinnedColumns: {
            left: ["cycleText", "farmName", "henhouseName", "hatcheryName"],
          },
        };
  });

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

  const loadViews = async () => {
    await handleApiResponse(
      () =>
        ColumnsViewsService.getColumnsViews(
          ColumnViewType.SummaryProductionAnalysis
        ),
      (data) => {
        setSavedViews(data.responseData?.items ?? []);
      },
      undefined,
      "Błąd podczas pobierania zapisanych widoków"
    );
  };

  useEffect(() => {
    const fetchInitialData = async () => {
      try {
        await handleApiResponse(
          () => SummaryService.getDictionaries(),
          (data) => setDictionary(data.responseData),
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania danych inicjalizujących.");
      }
    };
    loadViews();
    fetchInitialData();
  }, []);

  useEffect(() => {
    const fetchAnalysisData = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => SummaryService.getProductionAnalysis(filters),
          (data) => {
            setAnalysisData(data.responseData?.items ?? []);
            setTotalRows(data.responseData?.totalRows ?? 0);
            setSummaryData(data.responseData?.summary ?? null);
          },
          undefined,
          "Błąd podczas pobierania danych analitycznych"
        );
      } catch {
        toast.error("Błąd podczas pobierania danych analitycznych");
      } finally {
        setLoading(false);
      }
    };
    fetchAnalysisData();
  }, [filters]);

  const handleSaveView = async (name: string) => {
    const currentState = apiRef.current?.exportState();
    await handleApiResponse(
      () =>
        ColumnsViewsService.addColumnView({
          name: name.trim(),
          state: JSON.stringify(currentState),
          type: ColumnViewType.SummaryProductionAnalysis,
        }),
      async () => {
        toast.success(`Widok "${name}" został zapisany.`);
        await loadViews();
      },
      undefined,
      "Nie udało się zapisać widoku."
    );
  };

  const handleLoadView = (viewId: string) => {
    const viewToLoad = savedViews.find((v) => v.id === viewId);
    if (!viewToLoad) return;
    try {
      apiRef.current?.restoreState(JSON.parse(viewToLoad.state));
      setSelectedView(viewId);
      toast.info(`Wczytano widok "${viewToLoad.name}".`);
    } catch {
      toast.error("Błąd podczas wczytywania widoku.");
    }
  };

  const handleDeleteView = async (id: string) => {
    await handleApiResponse(
      () => ColumnsViewsService.deleteColumnView(id), // Corrected method name
      () => {
        toast.success("Widok został usunięty.");
        if (selectedView === id) {
          setSelectedView("");
        }
        loadViews();
      },
      undefined,
      "Nie udało się usunąć widoku."
    );
  };

  const columnStats = useMemo(() => {
    if (analysisData.length === 0) return {};

    const stats: {
      [key in keyof ProductionAnalysisRowModel]?: {
        min: number;
        max: number;
        avg: number;
      };
    } = {};
    const keys: (keyof ProductionAnalysisRowModel)[] = [
      "combinedAvgWeight",
      "totalSaleAvgWeightDeviation",
      "partSaleAvgWeightDeviation",
      "deadPctCycle",
      "defectivePctCycle",
      "deadAndDefectivePctCycle",
      "fcrWithLosses",
      "fcrWithoutLosses",
      "eww",
    ];

    for (const key of keys) {
      const values = analysisData
        .map((row) => row[key] as number)
        .filter((v) => v !== null && !isNaN(v));
      if (values.length > 0) {
        const min = Math.min(...values);
        const max = Math.max(...values);
        const avg = values.reduce((a, b) => a + b, 0) / values.length;
        stats[key] = { min, max, avg };
      }
    }
    return stats;
  }, [analysisData]);

  const columns = useMemo(
    () => getProductionAnalysisColumns(columnStats),
    [columnStats]
  );

  const summaryRow = useMemo(() => {
    if (!summaryData) return null;
    
    return {
      id: 'sum',
      cycleText: 'Suma',
      farmName: '',
      henhouseName: '',
      hatcheryName: '',
      insertionDate: '',
      isSummaryRow: true,
      ...summaryData,
      partSaleAvgWeightDeviation: null,
      totalSaleAvgWeightDeviation: null,
      partSaleDate: null,
      totalSaleDate: null,
    } as ProductionAnalysisRowModel;
  }, [summaryData]);

  // Pass the state and handlers down to the toolbar component
  const SummaryProductionToolbar = (props: GridToolbarProps) => {
    return (
      <CustomToolbarAnalysis
        {...props}
        savedViews={savedViews}
        selectedView={selectedView}
        onLoadView={handleLoadView}
        onSaveView={handleSaveView}
        onDeleteView={handleDeleteView}
      />
    );
  };

  return (
    <Box p={4}>
      <Box mb={2}>
        <Typography variant="h4">Analiza produkcyjna</Typography>
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
          apiRef={apiRef} // Attach the apiRef to the grid
          loading={loading}
          rows={analysisData}
          columns={columns}
          scrollbarSize={17}
          initialState={initialGridState}
          pinnedRows={summaryRow ? { bottom: [summaryRow] } : undefined}
          isRowSelectable={(params) => !params.row.isSummaryRow}
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
              "productionAnalysisGridState",
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
            localStorage.setItem(
              "summaryProductionAnalysisPageSize",
              pageSize.toString()
            );

            dispatch({
              type: "setMultiple",
              payload: { page, pageSize },
            });
          }}
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{
            toolbar: SummaryProductionToolbar,
            noRowsOverlay: NoRowsOverlay,
          }}
          showToolbar
          getRowClassName={(params) => {
            if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID || params.row.isSummaryRow) {
              return "aggregated-row";
            }
            return "";
          }}
          sx={{
            "& .cell-good": {
              backgroundColor: "rgba(0, 255, 0, 0.1)",
            },
            "& .cell-bad": {
              backgroundColor: "rgba(255, 0, 0, 0.1)",
            },
            "& .cell-neutral": {
              backgroundColor: "rgba(255, 255, 0, 0.1)",
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
        />
      </Box>
    </Box>
  );
};

export default SummaryProductionAnalysisPage;
