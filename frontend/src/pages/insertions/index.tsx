import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  tablePaginationClasses,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import AddInsertionModal from "../../components/modals/insertions/add-insertion-modal";
import SetCycleModal from "../../components/modals/insertions/add-cycle-modal";
import {
  filterReducer,
  initialFilters,
  InsertionOrderType,
} from "../../models/insertions/insertions-filters";
import type { InsertionDictionary } from "../../models/insertions/insertion-dictionary";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { InsertionsService } from "../../services/insertions-service";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type InsertionListModel from "../../models/insertions/insertions";
import { mapInsertionOrderTypeToField } from "../../common/helpers/insertion-order-type-helper";
import EditInsertionModal from "../../components/modals/insertions/edit-insertion-modal";
import type { CycleDictModel } from "../../models/common/dictionaries";
import { getInsertionFiltersConfig } from "./filter-config.insertion";
import FiltersForm from "../../components/filters/filters-form";
import { getInsertionsColumns } from "./insertions-columns";
import {
  DataGridPremium,
  type GridRowSelectionModel,
} from "@mui/x-data-grid-premium";
import LoadingButton from "../../components/common/loading-button";

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
  const [visibilityModel, setVisibilityModel] = useState(() => {
    const saved = localStorage.getItem("columnVisibilityModelInsertions");
    return saved ? JSON.parse(saved) : {};
  });

  const [selectedRowIds, setSelectedRowIds] = useState<GridRowSelectionModel>({
    type: "include",
    ids: new Set(),
  });
  const [isWiosModalOpen, setIsWiosModalOpen] = useState(false);
  const [wiosComment, setWiosComment] = useState("");

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

  const deleteInsertion = async (id: string) => {
    try {
      setLoading(true);
      await handleApiResponse(
        () => InsertionsService.deleteInsertion(id),
        async () => {
          toast.success("Wstawienie zostało poprawnie usunięte");
          dispatch({ type: "setMultiple", payload: { page: filters.page } });
        },
        undefined,
        "Błąd podczas usuwania wstawienia"
      );
    } catch {
      toast.error("Błąd podczas usuwania wstawienia");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInsertions();
  }, [filters]);

  const handleMarkAsReportedToWios = async () => {
    setLoading(true);
    console.log(selectedRowIds.ids);
    try {
      await handleApiResponse(
        () =>
          InsertionsService.markAsReportedToWios({
            insertionIds: [...selectedRowIds.ids],
            comment: wiosComment,
          }),
        () => {
          toast.success("Pomyślnie oznaczono jako zgłoszone do WIOŚ.");
          fetchInsertions();
          setSelectedRowIds({
            type: "include",
            ids: new Set(),
          });
          setIsWiosModalOpen(false);
          setWiosComment("");
        },
        undefined,
        "Wystąpił błąd podczas zapisu."
      );
    } catch (error) {
      toast.error(`Wystąpił błąd: ${error}`);
    } finally {
      setLoading(false);
    }
  };

  const columns = useMemo(
    () =>
      getInsertionsColumns({
        setSelectedInsertion,
        deleteInsertion,
        setIsEditModalOpen,
        dispatch,
        filters,
      }),
    [dispatch, filters]
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
        <Typography variant="h4">Wstawienia</Typography>
        <Box display="flex" gap={2}>
          {selectedRowIds.ids.size > 0 && (
            <Button
              variant="contained"
              color="primary"
              onClick={() => setIsWiosModalOpen(true)}
            >
              Oznacz jako zgłoszone do WIOŚ ({selectedRowIds.ids.size})
            </Button>
          )}

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
        <DataGridPremium
          loading={loading}
          rows={insertions}
          columns={columns}
          columnVisibilityModel={visibilityModel}
          onColumnVisibilityModelChange={(model) => {
            setVisibilityModel(model);
            localStorage.setItem(
              "columnVisibilityModelInsertions",
              JSON.stringify(model)
            );
          }}
          scrollbarSize={17}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
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
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ noRowsOverlay: NoRowsOverlay }}
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
          checkboxSelection
          disableRowSelectionOnClick
          isRowSelectable={(params) => !params.row.reportedToWios}
          onRowSelectionModelChange={(newSelectionModel) => {
            setSelectedRowIds(newSelectionModel);
          }}
          rowSelectionModel={selectedRowIds}
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

      <Dialog
        open={isWiosModalOpen}
        onClose={() => setIsWiosModalOpen(false)}
        fullWidth
        maxWidth="sm"
      >
        <DialogTitle>Oznacz jako zgłoszone do WIOŚ</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Komentarz (opcjonalnie)"
            type="text"
            fullWidth
            variant="outlined"
            multiline
            rows={3}
            value={wiosComment}
            onChange={(e) => setWiosComment(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setIsWiosModalOpen(false)}>Anuluj</Button>
          <LoadingButton onClick={handleMarkAsReportedToWios} loading={loading}>
            Zatwierdź
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default InsertionsPage;
