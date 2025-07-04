import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { useEffect, useMemo, useReducer, useState } from "react";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import AddInsertionModal from "../../components/modals/insertions/add-insertion-modal";
import SetCycleModal from "../../components/modals/insertions/add-cycle-modal";
import dayjs from "dayjs";

import {
  InsertionOrderType,
  type InsertionsFilterPaginationModel,
} from "../../models/insertions/insertions-filters";
import type { InsertionDictionary } from "../../models/insertions/insertion-dictionary";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { InsertionsService } from "../../services/insertions-service";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type InsertionListModel from "../../models/insertions/insertions";
import { mapInsertionOrderTypeToField } from "../../common/helpers/insertion-order-type-helper";
import Loading from "../../components/loading/loading";
import EditInsertionModal from "../../components/modals/insertions/edit-insertion-modal";
import type { CycleDictModel } from "../../models/common/dictionaries";
import { getInsertionFiltersConfig } from "./filter-config.insertion";
import FiltersForm from "../../components/filters/filters-form";

const initialFilters: InsertionsFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  hatcheryIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
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
  const [insertions, setInsertions] = useState<InsertionListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [selectedInsertion, setSelectedInsertion] =
    useState<InsertionListModel | null>(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

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
      valueGetter: (params: any) => dayjs(params.value).format("YYYY-MM-DD"),
    },
    { field: "quantity", headerName: "Sztuki wstawione", flex: 1 },
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    { field: "bodyWeight", headerName: "Śr. masa ciała", flex: 1 },
    {
      field: "sendToIrz",
      headerName: "Wyślij do IRZplus",
      flex: 1,

      minWidth: 200,
      type: "actions",
      renderCell: (params) => {
        const { isSentToIrz, dateIrzSentUtc } = params.row;
        const [loadingSendToIrz, setLoadingSendToIrz] = useState(false);
        const handleSendToIrz = async (data: {
          internalGroupId?: string;
          insertionId?: string;
        }) => {
          setLoadingSendToIrz(true);
          await handleApiResponse(
            () => InsertionsService.sendToIrzPlus(data),
            async () => {
              toast.success("Wysłano do IRZplus");
              dispatch({
                type: "setMultiple",
                payload: { page: filters.page },
              });
              setLoadingSendToIrz(false);
            },
            undefined,
            "Wystąpił błąd podczas wysyłania do IRZplus"
          );
          setLoadingSendToIrz(false);
        };
        if (dateIrzSentUtc) {
          const formattedDate = new Date(dateIrzSentUtc).toLocaleString(
            "pl-PL",
            {
              dateStyle: "short",
              timeStyle: "short",
            }
          );

          return (
            <Typography variant="body2" sx={{ whiteSpace: "nowrap" }}>
              Wysłano - {formattedDate}
            </Typography>
          );
        }

        if (isSentToIrz) {
          return null;
        }

        return (
          <Box
            sx={{
              display: "flex",
              flexDirection: "row",
              alignItems: "center",
              gap: 1,
              flexWrap: "nowrap",
            }}
          >
            {loadingSendToIrz ? (
              <Loading height="0" size={10} />
            ) : (
              <>
                <Button
                  variant="contained"
                  color="error"
                  size="small"
                  onClick={() =>
                    handleSendToIrz({ insertionId: params.row.id })
                  }
                >
                  Osobno
                </Button>

                <Button
                  variant="outlined"
                  color="error"
                  size="small"
                  onClick={() =>
                    handleSendToIrz({
                      internalGroupId: params.row.internalGroupId,
                    })
                  }
                >
                  Z grupą
                </Button>
              </>
            )}
          </Box>
        );
      },
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedInsertion(params.row);
            setIsEditModalOpen(true);
          }}
        >
          Edytuj
        </Button>,
      ],
    },

    {
      field: "documentNumber",
      headerName: "Numer dokumentu IRZplus",
      flex: 1,
      renderCell: (params) => {
        return params.value ? params.value : "Brak numeru";
      },
    },
    { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
  ];

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
          () => InsertionsService.getDictionaries(),
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
    const fetchInsertions = async () => {
      setLoading(true);
      try {
        await handleApiResponse<PaginateModel<InsertionListModel>>(
          () => InsertionsService.getInsertions(filters),
          (data) => {
            setInsertions(data.responseData?.items ?? []);
            setTotalRows(data.responseData?.totalRows ?? 0);
          },
          undefined,
          "Błąd podczas pobierania wstawień"
        );
      } catch {
        toast.error("Błąd podczas pobierania wstawień");
      } finally {
        setLoading(false);
      }
    };
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
        config={getInsertionFiltersConfig(dictionary, uniqueCycles, filters)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={insertions}
          columns={columns}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
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
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
          }}
          sortingMode="server"
          onSortModelChange={(model) => {
            if (model.length > 0) {
              const sortField = model[0].field;
              const foundOrderBy = Object.values(InsertionOrderType).find(
                (orderType) =>
                  mapInsertionOrderTypeToField(orderType) === sortField
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
      <EditInsertionModal
        open={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedInsertion(null);
        }}
        onSave={() => {
          setIsEditModalOpen(false);
          setSelectedInsertion(null);
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        }}
        insertion={selectedInsertion}
      />

      <AddInsertionModal
        open={openModal}
        onClose={() => setOpenModal(false)}
        onSave={() => {
          setOpenModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
      <SetCycleModal
        open={openCycleModal}
        onClose={() => setOpenCycleModal(false)}
      />
    </Box>
  );
};

export default InsertionsPage;
