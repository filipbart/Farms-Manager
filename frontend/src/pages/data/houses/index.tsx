import {
  Box,
  Button,
  MenuItem,
  tablePaginationClasses,
  TextField,
  Typography,
} from "@mui/material";
import { type GridColDef, type GridRenderCellParams } from "@mui/x-data-grid";
import { useCallback, useEffect, useMemo, useState } from "react";
import { FarmsService } from "../../../services/farms-service";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type { PaginateModel } from "../../../common/interfaces/paginate";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { HouseRowModel } from "../../../models/farms/house-row-model";
import Loading from "../../../components/loading/loading";
import AddHenhouseModal from "../../../components/modals/farms/add-henhouse-modal";
import { toast } from "react-toastify";
import { useFarms } from "../../../hooks/useFarms";
import { DataGridPro } from "@mui/x-data-grid-pro";
import EditHenhouseModal from "../../../components/modals/farms/edit-henhouse-modal";

const HousesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [chosenFarm, setChosenFarm] = useState<FarmRowModel>();

  const [addModalOpen, setAddModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [selectedHenhouse, setSelectedHenhouse] =
    useState<HouseRowModel | null>(null);

  const setChosenFarmCallback = useCallback((chosenFarm: FarmRowModel) => {
    setChosenFarm(chosenFarm);
    fetchHouses(chosenFarm.id);
  }, []);

  const fetchHouses = async (farmId: string) => {
    if (!farmId) return;
    setLoading(true);
    await handleApiResponse<PaginateModel<HouseRowModel>>(
      () => FarmsService.getFarmHousesAsync(farmId),
      (data) => {
        setHenhouses(data.responseData?.items ?? []);
      },
      undefined,
      "Nie udało się pobrać listy kurników"
    );
    setLoading(false);
  };

  useEffect(() => {
    fetchFarms();
  }, []);

  const handleAddOpen = () => setAddModalOpen(true);
  const handleAddClose = () => setAddModalOpen(false);
  const handleAddSave = async () => {
    setAddModalOpen(false);
    await fetchHouses(chosenFarm?.id ?? "");
  };

  const handleEditOpen = (henhouse: HouseRowModel) => {
    setSelectedHenhouse(henhouse);
    setEditModalOpen(true);
  };
  const handleEditClose = () => {
    setEditModalOpen(false);
    setSelectedHenhouse(null);
  };
  const handleEditSave = async () => {
    setEditModalOpen(false);
    setSelectedHenhouse(null);
    await fetchHouses(chosenFarm?.id ?? "");
  };

  const columns: GridColDef<HouseRowModel>[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "code", headerName: "ID Budynku", flex: 1 },
    { field: "area", headerName: "Powierzchnia (m²)", type: "number", flex: 1 },
    { field: "description", headerName: "Opis", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia rekordu",
      type: "dateTime",
      flex: 2,
      valueGetter: (value: any) => (value ? new Date(value) : null),
    },
    {
      field: "actions",
      headerName: "Akcje",
      flex: 2,
      sortable: false,
      filterable: false,
      renderCell: (params: GridRenderCellParams<any, HouseRowModel>) => (
        <div className="text-center">
          <Button variant="outlined" onClick={() => handleEditOpen(params.row)}>
            Edytuj
          </Button>
          <Button
            variant="outlined"
            color="error"
            sx={{ ml: 1 }}
            onClick={async () => {
              setLoading(true);
              await handleApiResponse(
                () => FarmsService.deleteHenhouseAsync(params.row.id),
                async () => {
                  toast.success("Kurnik został usunięty");
                  await fetchHouses(chosenFarm?.id ?? "");
                },
                undefined,
                "Nie udało się usunąć kurnika"
              );
              setLoading(false);
            }}
          >
            Usuń
          </Button>
        </div>
      ),
    },
  ];

  const data = useMemo(
    () => ({
      rows: henhouses,
      columns: columns,
    }),
    [henhouses]
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
        <Typography variant="h4">Kurniki</Typography>
        <Box display="flex" gap={2}>
          <Button
            variant="contained"
            color="primary"
            onClick={handleAddOpen}
            disabled={chosenFarm === undefined}
          >
            Dodaj kurnik
          </Button>
        </Box>
      </Box>

      <Box mb={2}>
        <Typography variant="h6">Wybierz fermę:</Typography>
        {loadingFarms ? (
          <Loading height="0" size={20} />
        ) : (
          <TextField
            sx={{ marginTop: 1 }}
            select
            name="house"
            label="Ferma"
            value={chosenFarm?.id ?? ""}
            onChange={(e) => {
              const farmId = e.target.value;
              const selectedFarm = farms.find((farm) => farm.id === farmId);
              if (selectedFarm) {
                setChosenFarmCallback(selectedFarm);
              }
            }}
            fullWidth
          >
            {farms.map((farm) => (
              <MenuItem key={farm.id} value={farm.id}>
                {farm.name}
              </MenuItem>
            ))}
          </TextField>
        )}
      </Box>

      <Box mt={4} sx={{ width: "100%", overflowX: "auto" }}>
        <DataGridPro
          loading={loading}
          {...data}
          localeText={{
            noRowsLabel:
              chosenFarm === undefined ? "Wybierz fermę" : "Brak wpisów",
            footerTotalRows: "Łączna liczba wierszy:",
          }}
          hideFooterPagination
          rowSelection={false}
          sx={{
            minHeight: henhouses.length === 0 ? 300 : "auto",
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
            "& .aggregated-row": {
              fontWeight: "bold",

              "& .MuiDataGrid-cell": {
                borderTop: "1px solid rgba(224, 224, 224, 1)",
                backgroundColor: "rgba(240, 240, 240, 0.7)",
              },
            },
            "& .MuiDataGrid-columnHeaders": { borderTop: "none" },
          }}
        />
      </Box>

      <AddHenhouseModal
        farmId={chosenFarm?.id ?? ""}
        open={addModalOpen}
        onClose={handleAddClose}
        onSave={handleAddSave}
      />

      <EditHenhouseModal
        open={editModalOpen}
        onClose={handleEditClose}
        onSave={handleEditSave}
        henhouseData={selectedHenhouse}
      />
    </Box>
  );
};

export default HousesPage;
