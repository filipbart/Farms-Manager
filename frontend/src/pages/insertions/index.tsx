import {
  Box,
  Button,
  Grid as MuiGrid,
  MenuItem,
  tablePaginationClasses,
  TextField,
  Typography,
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
import { DatePicker } from "@mui/x-date-pickers";
import { useEffect, useState } from "react";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import AddInsertionModal from "../../components/modals/insertions/add-insertion-modal";
import SetCycleModal from "../../components/modals/insertions/add-cycle-modal";
//@ts-ignore
import dayjs from "dayjs";
import { Dayjs } from "dayjs";

import type { InsertionsFilterPaginationModel } from "../../models/insertions/insertions-filters";
import type {
  CycleDictModel,
  InsertionDictionary,
} from "../../models/insertions/insertion-dictionary";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { InsertionsService } from "../../services/insertions-service";
import { toast } from "react-toastify";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type InsertionListModel from "../../models/insertions/insertions";

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
    valueGetter: (params: any) => {
      return params ? dayjs(params).format("YYYY-MM-DD") : null;
    },
  },
  { field: "quantity", headerName: "Sztuki wstawione", flex: 1 },
  { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
  { field: "bodyWeight", headerName: "Śr. masa ciała", flex: 1 },
  { field: "dateCreatedUtc", headerName: "Data utworzenia wpisu", flex: 1 },
];

const InsertionsPage: React.FC = () => {
  const [filters, setFilters] = useState<InsertionsFilterPaginationModel>({
    farmIds: [],
    cycles: [],
    henhouseIds: [],
    hatcheryIds: [],
    dateSince: "",
    dateTo: "",
  });

  const [dictionary, setDictionary] = useState<InsertionDictionary>();
  const [openModal, setOpenModal] = useState(false);
  const [openCycleModal, setOpenCycleModal] = useState(false);
  const [loading, setLoading] = useState(false);
  const [insertions, setInsertions] = useState<InsertionListModel[]>();

  const getUniqueCycles = (): CycleDictModel[] => {
    if (!dictionary) return [];

    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) {
        map.set(key, cycle);
      }
    }
    return Array.from(map.values());
  };

  const fetchInsertions = async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<InsertionListModel>>(
        () => InsertionsService.getInsertions(filters),
        (data) => setInsertions(data.responseData?.items),
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

  const handleMultiSelectChange = (
    key: keyof InsertionsFilterPaginationModel,
    values: string[]
  ) => {
    setFilters((prev) => ({ ...prev, [key]: values }));
  };

  const handleDateChange = (
    key: "dateSince" | "dateTo",
    value: Dayjs | null
  ) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value ? value.format("YYYY-MM-DD") : "",
    }));
  };

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

      <Box mb={2}>
        <Typography variant="h6" mb={1}>
          Filtry
        </Typography>
        <MuiGrid container spacing={2}>
          {/*@ts-ignore*/}
          <MuiGrid item xs={12} sm={6} md={3}>
            <TextField
              label="Ferma"
              select
              slotProps={{ select: { multiple: true } }}
              sx={{ minWidth: 200 }}
              fullWidth
              value={filters.farmIds}
              onChange={(e) =>
                handleMultiSelectChange(
                  "farmIds",
                  Array.from(e.target.value as unknown as string[])
                )
              }
              disabled={!dictionary}
            >
              {dictionary?.farms.map((farm) => (
                <MenuItem key={farm.id} value={farm.id}>
                  {farm.name}
                </MenuItem>
              )) || <MenuItem disabled>Ładowanie...</MenuItem>}
            </TextField>
          </MuiGrid>
          {/*@ts-ignore*/}
          <MuiGrid item xs={12} sm={6} md={3}>
            <TextField
              label="Identyfikator (cykl)"
              select
              slotProps={{ select: { multiple: true } }}
              sx={{ minWidth: 200 }}
              fullWidth
              value={filters.cycles.map((c) => `${c.identifier}-${c.year}`)}
              onChange={(e) => {
                const selectedValues = Array.from(
                  e.target.value as unknown as string[]
                );
                const selectedCycles = getUniqueCycles().filter((cycle) =>
                  selectedValues.includes(`${cycle.identifier}-${cycle.year}`)
                );
                setFilters((prev) => ({
                  ...prev,
                  cycles: selectedCycles,
                }));
              }}
              disabled={!dictionary}
            >
              {getUniqueCycles().map((cycle) => (
                <MenuItem
                  key={`${cycle.identifier}-${cycle.year}`}
                  value={`${cycle.identifier}-${cycle.year}`}
                >
                  {cycle.identifier.toString().padStart(2, "0")}/{cycle.year}
                </MenuItem>
              ))}
            </TextField>
          </MuiGrid>
          {/*@ts-ignore*/}
          <MuiGrid item xs={12} sm={6} md={3}>
            <TextField
              label="Kurnik"
              select
              slotProps={{ select: { multiple: true } }}
              fullWidth
              sx={{ minWidth: 200 }}
              value={filters.henhouseIds}
              onChange={(e) =>
                handleMultiSelectChange(
                  "henhouseIds",
                  Array.from(e.target.value as unknown as string[])
                )
              }
              disabled={filters.farmIds.length === 0 || !dictionary}
            >
              {filters.farmIds.length === 0 ? (
                <MenuItem disabled>Wybierz fermę najpierw</MenuItem>
              ) : (
                dictionary?.farms
                  .filter((farm) => filters.farmIds.includes(farm.id))
                  .flatMap((farm) =>
                    farm.henhouses.map((henhouse) => (
                      <MenuItem key={henhouse.id} value={henhouse.id}>
                        {henhouse.name}
                      </MenuItem>
                    ))
                  )
              )}
            </TextField>
          </MuiGrid>
          {/*@ts-ignore*/}
          <MuiGrid item xs={12} sm={6} md={3}>
            <TextField
              label="Wylęgarnia"
              select
              slotProps={{ select: { multiple: true } }}
              sx={{ minWidth: 200 }}
              fullWidth
              value={filters.hatcheryIds}
              onChange={(e) =>
                handleMultiSelectChange(
                  "hatcheryIds",
                  Array.from(e.target.value as unknown as string[])
                )
              }
              disabled={!dictionary}
            >
              {dictionary?.hatcheries.map((hatchery) => (
                <MenuItem key={hatchery.id} value={hatchery.id}>
                  {hatchery.name}
                </MenuItem>
              )) || <MenuItem disabled>Ładowanie...</MenuItem>}
            </TextField>
          </MuiGrid>
        </MuiGrid>
        <MuiGrid container spacing={2} mt={2}>
          {/* @ts-ignore */}
          <MuiGrid item xs={12} sm={6} md={3}>
            <DatePicker
              label="Data od"
              value={filters.dateSince ? dayjs(filters.dateSince) : null}
              onChange={(val) => handleDateChange("dateSince", val)}
              slotProps={{
                textField: { fullWidth: true },
                actionBar: { actions: ["clear"] },
              }}
            />
          </MuiGrid>
          {/* @ts-ignore */}
          <MuiGrid item xs={12} sm={6} md={3}>
            <DatePicker
              label="Data do"
              value={filters.dateTo ? dayjs(filters.dateTo) : null}
              onChange={(val) => handleDateChange("dateTo", val)}
              slotProps={{
                textField: { fullWidth: true },
                actionBar: { actions: ["clear"] },
              }}
            />
          </MuiGrid>
        </MuiGrid>
      </Box>

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGrid
          loading={loading}
          rows={insertions}
          columns={columns}
          initialState={{
            ...columns,
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
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
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
