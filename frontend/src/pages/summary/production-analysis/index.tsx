import { Box, tablePaginationClasses, Typography } from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import {
  filterReducer,
  initialFilters,
  type ProductionAnalysisDictionary,
} from "../../../models/summary/production-analysis-filters";
import type { ProductionAnalysisRowModel } from "../../../models/summary/production-analysis";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { SummaryService } from "../../../services/summary-service";
import { DataGridPremium } from "@mui/x-data-grid-premium";
import { getProductionAnalysisColumns } from "./production-analysis-columns";
import FiltersForm from "../../../components/filters/filters-form";
import { getProductionAnalysisFiltersConfig } from "./filter-config.production-analysis";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import CustomToolbarAnalysis from "../../../components/datagrid/custom-toolbar-analysis";

const SummaryProductionAnalysisPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<ProductionAnalysisDictionary>();
  const [loading, setLoading] = useState(false);
  const [analysisData, setAnalysisData] = useState<
    ProductionAnalysisRowModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem(
      "columnVisibilityModelProductionAnalysis"
    );
    return saved ? JSON.parse(saved) : {};
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

  useEffect(() => {
    const fetchDictionaries = async () => {
      try {
        await handleApiResponse(
          () => SummaryService.getDictionaries(),
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
    const fetchAnalysisData = async () => {
      setLoading(true);
      try {
        await handleApiResponse(
          () => SummaryService.getProductionAnalysis(filters),
          (data) => {
            setAnalysisData(data.responseData?.items ?? []);
            setTotalRows(data.responseData?.totalRows ?? 0);
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

  const columns = useMemo(() => getProductionAnalysisColumns(), []);

  return (
    <Box p={4}>
      <Box mb={2}>
        <Typography variant="h4">Analiza produkcyjna</Typography>
      </Box>

      <FiltersForm
        config={getProductionAnalysisFiltersConfig(
          dictionary,
          uniqueCycles,
          filters
        )}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPremium
          loading={loading}
          rows={analysisData}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelProductionAnalysis",
              JSON.stringify(model)
            );
          }}
          scrollbarSize={17}
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
          slots={{
            toolbar: CustomToolbarAnalysis,
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

export default SummaryProductionAnalysisPage;
