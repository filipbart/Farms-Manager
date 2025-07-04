import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { type GridColDef, DataGrid } from "@mui/x-data-grid";
import dayjs from "dayjs";
import { useReducer, useState, useMemo, useEffect } from "react";
import { toast } from "react-toastify";
import { mapSaleOrderTypeToField } from "../../common/helpers/sale-order-type-helper";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import FiltersForm from "../../components/filters/filters-form";
import type { CycleDictModel } from "../../models/common/dictionaries";
import type SalesListModel from "../../models/sales/sales";
import type { SalesDictionary } from "../../models/sales/sales-dictionary";
import {
  type SalesFilterPaginationModel,
  SalesOrderType,
} from "../../models/sales/sales-filters";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { getSaleFiltersConfig } from "./filter-config.sales";
import AddSaleModal from "../../components/modals/sales/add-sale-modal/add-sale-modal";
import { SalesService } from "../../services/sales-service";
import Loading from "../../components/loading/loading";
import type { PaginateModel } from "../../common/interfaces/paginate";

const initialFilters: SalesFilterPaginationModel = {
  farmIds: [],
  cycles: [],
  henhouseIds: [],
  slaughterhouseIds: [],
  dateSince: "",
  dateTo: "",
  page: 0,
  pageSize: 10,
};

function filterReducer(
  state: SalesFilterPaginationModel,
  action:
    | { type: "set"; key: keyof SalesFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<SalesFilterPaginationModel> }
): SalesFilterPaginationModel {
  switch (action.type) {
    case "set":
      return { ...state, [action.key]: action.value };
    case "setMultiple":
      return { ...state, ...action.payload };
    default:
      return state;
  }
}

const SalesPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<SalesDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [sales, setSales] = useState<SalesListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  //   const [selectedInsertion, setSelectedInsertion] =
  //     useState<SalesListModel | null>(null);
  //   const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const columns: GridColDef[] = [
    { field: "id", headerName: "Id", width: 70 },
    { field: "cycleText", headerName: "Identyfikator", flex: 1 },
    { field: "farmName", headerName: "Ferma", flex: 1 },
    { field: "henhouseName", headerName: "Kurnik", flex: 1 },
    {
      field: "saleDate",
      headerName: "Data sprzedaży",
      flex: 1,
      type: "string",
      valueGetter: (params: any) => dayjs(params.value).format("YYYY-MM-DD"),
    },
    { field: "typeDesc", headerName: "Typ sprzedaży", flex: 1 },
    { field: "weight", headerName: "Waga ubojni [kg]", flex: 1 },
    { field: "quantity", headerName: "Ilość sztuk ubojnia [szt]", flex: 1 },
    { field: "confiscatedWeight", headerName: "Konfiskaty [kg]", flex: 1 },
    { field: "confiscatedCount", headerName: "Konfiskaty [szt]", flex: 1 },
    { field: "deadWeight", headerName: "Kurczęta martwe [kg]", flex: 1 },
    { field: "deadCount", headerName: "Kurczęta martwe [szt]", flex: 1 },
    { field: "farmerWeight", headerName: "Waga producenta [kg]", flex: 1 },
    { field: "basePrice", headerName: "Cena bazowa [zł]", flex: 1 },
    { field: "priceWithExtras", headerName: "Cena z dodatkami [zł]", flex: 1 },
    { field: "otherExtras", headerName: "Inne dodatki", flex: 1 },
    { field: "comment", headerName: "Komentarz", flex: 1 },

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
          // await handleApiResponse(
          //   () => InsertionsService.sendToIrzPlus(data),
          //   async () => {
          //     toast.success("Wysłano do IRZplus");
          //     dispatch({
          //       type: "setMultiple",
          //       payload: { page: filters.page },
          //     });
          //     setLoadingSendToIrz(false);
          //   },
          //   undefined,
          //   "Wystąpił błąd podczas wysyłania do IRZplus"
          // );
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
            // setSelectedInsertion(params.row);
            // setIsEditModalOpen(true);
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
          () => SalesService.getDictionaries(),
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
        await handleApiResponse<PaginateModel<SalesListModel>>(
          () => SalesService.getSales(filters),
          (data) => {
            setSales(data.responseData?.items ?? []);
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
        <Typography variant="h4">Sprzedaże</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setOpenModal(true)}
          >
            Dodaj nowy wpis
          </Button>
        </Box>
      </Box>

      <FiltersForm
        config={getSaleFiltersConfig(dictionary, uniqueCycles, filters)}
        filters={filters}
        dispatch={dispatch}
      />

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={sales}
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
              const foundOrderBy = Object.values(SalesOrderType).find(
                (orderType) => mapSaleOrderTypeToField(orderType) === sortField
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
      {/* <EditInsertionModal
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
      /> */}

      <AddSaleModal
        open={openModal}
        onClose={() => setOpenModal(false)}
        onSave={() => {
          setOpenModal(false);
          dispatch({ type: "setMultiple", payload: { page: 0 } });
        }}
      />
    </Box>
  );
};

export default SalesPage;
