import {
  Box,
  Button,
  Grid,
  MenuItem,
  tablePaginationClasses,
  TextField,
  Typography,
} from "@mui/material";
import {
  DataGridPro,
  type GridCellParams,
  type GridColDef,
} from "@mui/x-data-grid-pro";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../../../models/common/dictionaries";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { FallenStockService } from "../../../../services/production-data/fallen-stocks-service";
import {
  filterReducer,
  initialFilters,
  type FallenStocksDictionary,
} from "../../../../models/fallen-stocks/fallen-stocks-filters";
import { useFallenStocks } from "../../../../hooks/useFallenStocks";
import { GRID_AGGREGATION_ROOT_FOOTER_ROW_ID } from "@mui/x-data-grid-premium";
import ActionsCell from "../../../../components/datagrid/actions-cell";
import AddFallenStocksModal from "../../../../components/modals/production-data/fallen-stocks/add-fallen-stocks-modal";
import EditFallenStocksModal from "../../../../components/modals/production-data/fallen-stocks/edit-fallen-stocks-modal";

const MainFallenStockPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FallenStocksDictionary>();
  const [openAddModal, setOpenAddModal] = useState(false);
  const [selectedFallenStock, setSelectedFallenStock] = useState<any | null>(
    null
  );
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const { viewModel, loading, fetchFallenStocks } = useFallenStocks(filters);

  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelFallenStock");
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
          () => FallenStockService.getDictionaries(),
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
    fetchFallenStocks();
  }, [fetchFallenStocks]);

  const deleteFallenStockRecord = async (internalGroupId: string) => {
    await handleApiResponse(
      () => FallenStockService.deleteFallenStocks(internalGroupId),
      async () => {
        toast.success("Wpis został poprawnie usunięty");
        fetchFallenStocks();
      },
      undefined,
      "Błąd podczas usuwania wpisu"
    );
  };

  const transformedData = useMemo(() => {
    if (!viewModel) return { insertionRows: [], summaryRows: [] };
    const transformRow = (row: any) => ({
      id: row.id,
      rowTitle: row.rowTitle,
      remaining: row.remaining,
      ...row.henhouseValues,
    });
    return {
      insertionRows: viewModel.insertionRows.map(transformRow),
      summaryRows: viewModel.summaryRows.map(transformRow),
    };
  }, [viewModel]);

  const columns: GridColDef[] = useMemo(() => {
    if (!viewModel?.henhouseColumns) return [];
    const titleColumn: GridColDef = {
      field: "rowTitle",
      headerName: "Wstawiono / Data zgłoszenia",
      width: 200,
      pinnable: true,

      cellClassName: "sticky-column-style",
    };
    const henhouseColumns: GridColDef[] = viewModel.henhouseColumns.map(
      (col) => ({
        field: col.id,
        headerName: col.name,
        width: 120,
        align: "center",
        headerAlign: "center",
        cellClassName: (params: GridCellParams) => {
          if (params.row.id === "summary_flock_state") {
            const value = params.value as number;
            if (value < 500) return "flock-state-low";
            if (value < 1000) return "flock-state-medium";
          }
          return "";
        },
      })
    );
    const remainingColumn: GridColDef = {
      field: "remaining",
      headerName: "Pozostało",
      width: 120,
      align: "center",
      headerAlign: "center",
    };

    const actionsColumn: GridColDef = {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 150,
      align: "center",
      headerAlign: "center",
      getActions: (params) => {
        const isSummaryRow = params.row.id.toString().startsWith("summary_");

        if (isSummaryRow) {
          return [];
        }

        const isSentToIrz = params.row.IsSentToIrz as boolean;

        const deleteHandler = isSentToIrz ? undefined : deleteFallenStockRecord;

        return [
          <ActionsCell
            key="actions"
            params={params}
            onEdit={(row) => {
              setSelectedFallenStock(row);
              setIsEditModalOpen(true);
            }}
            onDelete={deleteHandler}
          />,
        ];
      },
    };

    return [titleColumn, ...henhouseColumns, remainingColumn, actionsColumn];
  }, [viewModel]);

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
        <Typography variant="h4">Ewidencja sztuk padłych</Typography>
        <Button
          variant="contained"
          color="primary"
          onClick={() => setOpenAddModal(true)}
        >
          Dodaj nowy wpis
        </Button>
      </Box>
      <Grid container spacing={2} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 4 }}>
          <TextField
            select
            label="Ferma"
            value={filters.farmId || ""}
            onChange={(e) =>
              dispatch({ type: "set", key: "farmId", value: e.target.value })
            }
            fullWidth
            disabled={loading}
          >
            <MenuItem value="">
              <em>Wszystkie</em>
            </MenuItem>
            {dictionary?.farms.map((farm) => (
              <MenuItem key={farm.id} value={farm.id}>
                {farm.name}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <TextField
            select
            label="Cykl"
            value={filters.cycle || ""}
            onChange={(e) =>
              dispatch({ type: "set", key: "cycle", value: e.target.value })
            }
            fullWidth
            disabled={loading}
          >
            <MenuItem value="">
              <em>Wszystkie</em>
            </MenuItem>
            {uniqueCycles.map((cycle) => (
              <MenuItem
                key={cycle.id}
                value={`${cycle.identifier}-${cycle.year}`}
              >
                {`${cycle.identifier}/${cycle.year}`}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
      </Grid>
      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          rows={transformedData.insertionRows}
          columns={columns}
          hideFooter
          pinnedRows={{ bottom: transformedData.summaryRows }}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelFallenStock",
              JSON.stringify(model)
            );
          }}
          getRowClassName={(params) => {
            const classNames = [];

            if (params.id === GRID_AGGREGATION_ROOT_FOOTER_ROW_ID) {
              classNames.push("aggregated-row");
            }

            if (params.row.id === "summary_culling") {
              classNames.push("culling-row-border");
            }

            return classNames.join(" ");
          }}
          scrollbarSize={17}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
            },
            pinnedColumns: {
              left: ["rowTitle"],
            },
          }}
          rowSelection={false}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
            "& .aggregated-row": {
              fontWeight: "bold",
            },
            "& .flock-state-low": { backgroundColor: "rgba(255, 72, 66, 0.3)" },
            "& .flock-state-medium": {
              backgroundColor: "rgba(255, 170, 0, 0.3)",
            },
            "& .sticky-column-style": {
              backgroundColor: "#f5f5f5",
              fontWeight: 500,
            },

            "& .culling-row-border .MuiDataGrid-cell": {
              borderTop: "2px solid rgba(0, 0, 0, 0.23)",
            },
          }}
        />
      </Box>
      {viewModel && (
        <Typography variant="h6" align="right" mt={2}>
          Suma całkowita:{" "}
          <strong>{formatCurrencyPLN(viewModel.grandTotal)}</strong>
        </Typography>
      )}

      <EditFallenStocksModal
        open={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSave={() => {
          fetchFallenStocks();
        }}
        internalGroupId={selectedFallenStock?.id ?? null}
      />
      <AddFallenStocksModal
        open={openAddModal}
        onClose={() => setOpenAddModal(false)}
        onSave={() => {
          setOpenAddModal(false);
          fetchFallenStocks();
        }}
      />
    </Box>
  );
};

const formatCurrencyPLN = (value: number | null | undefined): string => {
  const numberToFormat = value ?? 0;

  return new Intl.NumberFormat("pl-PL").format(numberToFormat);
};

export default MainFallenStockPage;
