import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useEffect, useMemo, useReducer, useState } from "react";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import AddInsertionModal from "../../components/modals/insertions/add-insertion-modal";
import SetCycleModal from "../../components/modals/insertions/add-cycle-modal";
//@ts-ignore
import dayjs from "dayjs";

import {
  InsertionOrderType,
  type InsertionsFilterPaginationModel,
} from "../../models/insertions/insertions-filters";
import type {
  CycleDictModel,
  InsertionDictionary,
} from "../../models/insertions/insertion-dictionary";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { InsertionsService } from "../../services/insertions-service";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type InsertionListModel from "../../models/insertions/insertions";
import FiltersForm from "./filter-form";
import { mapInsertionOrderTypeToField } from "../../common/helpers/insertion-order-type-helper";

const columns: GridColDef[] = [
  { field: "id", headerName: "Id", width: 70 },
  { field: "cycleText", headerName: "Identyfikator", flex: 1 },
  { field: "farmName", headerName: "Ferma", flex: 1 },
  { field: "henhouseName", headerName: "Kurnik", flex: 1 },
  {
    field: "insertionDate",
    headerName: "Data wstawienia",
    flex: 1,
    type: "string",
    valueGetter: (params: any) => params && dayjs(params).format("YYYY-MM-DD"),
  },
  { field: "quantity", headerName: "Sztuki wstawione", flex: 1 },
  { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
  { field: "bodyWeight", headerName: "Śr. masa ciała", flex: 1 },
  { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
];

const initialFilters: InsertionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
};

function filterReducer(
  state: InsertionsFilterPaginationModel,
  action:
    | { type: "set"; key: keyof InsertionsFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<InsertionsFilterPaginationModel> }
): InsertionsFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

const InsertionsPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<InsertionDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [openCycleModal, setOpenCycleModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [insertions, setInsertions] = useState<InsertionListModel[]>();
  const [totalRows, setTotalRows] = useState(0);

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

  const fetchInsertions = async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<InsertionListModel>>(
        () => InsertionsService.getInsertions(filters),
        (data) => {
          setInsertions(data.responseData?.items);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania wstawień"
      );
    } catch (error) {
      toast.error("Błąd podczas pobierania wstawień");
    } finally {
      setLoading(false);
    }
  };

  const fetchDictionaries = async () => {
    try {
      await handleApiResponse(
        () => InsertionsService.getDictionaries(),
        (data) => setDictionary(data.responseData),
        undefined,
        "Błąd podczas pobierania słowników filtrów"
      );
    } catch (error) {
      toast.error("Błąd podczas pobierania słowników filtrów");
    }
  };

  useEffect(() => {
    fetchDictionaries();
  }, []);

  useEffect(() => {
    fetchInsertions();
  }, [filters]);

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
        <Typography variant="h4">Wstawienia</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenModal(true)}
          >
            Dodaj nowy wpis
          </Button>
          <Button
            variant="outlined"
            color="primary"
            onClick={() => setOpenCycleModal(true)}
          >
            Nowy cykl
          </Button>
        </Box>
      </Box>

      <FiltersForm
        dictionary={dictionary}
        filters={filters}
        dispatch={dispatch}
        uniqueCycles={uniqueCycles}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={insertions}
          columns={columns}
          initialState={{
            columns: {
              columnVisibilityModel: {
                id: false,
              },
            },
          }}
          localeText={{
            paginationRowsPerPage: "Wierszy na stronę:",
            paginationDisplayedRows: ({ from, to, count }) =>
              `${from} do ${to} z ${count}`,
          }}
          paginationMode="server"
          paginationModel={{
            pageSize: filters.pageSize ?? 10,
            page: (filters.pageNumber ?? 1) - 1,
          }}
          onPaginationModelChange={({ page, pageSize }) =>
            dispatch({
              type: "setMultiple",
              payload: {
                pageNumber: page + 1,
                pageSize,
              },
            })
          }
          rowCount={totalRows}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
          sortingMode="server"
          sortModel={
            filters.orderBy
              ? [
                  {
                    field: mapInsertionOrderTypeToField(filters.orderBy),
                    sort: filters.isDescending ? "desc" : "asc",
                  },
                ]
              : []
          }
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;

              // Znajdź enum dla pola field
              const foundOrderBy = Object.values(InsertionOrderType).find(
                (orderType) =>
                  mapInsertionOrderTypeToField(orderType) === sortField
              );

              dispatch({
                type: "setMultiple",
                payload: {
                  orderBy: foundOrderBy ?? undefined,
                  isDescending: model[0].sort === "desc",
                },
              });
            } else {
              dispatch({
                type: "setMultiple",
                payload: {
                  orderBy: undefined,
                  isDescending: undefined,
                },
              });
            }
          }}
        />
      </Box>

      <AddInsertionModal open={openModal} onClose={() => setOpenModal(false)} />
      <SetCycleModal
        open={openCycleModal}
        onClose={() => setOpenCycleModal(false)}
      />
    </Box>
  );
};

export default InsertionsPage;
