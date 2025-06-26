import {
  Box,
  Button,
  MenuItem,
  tablePaginationClasses,
  TextField,
  Typography,
} from "@mui/material";
import { DataGrid, type GridColDef } from "@mui/x-data-grid";
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

const HousesPage: React.FC = () => {
  const [loading, setLoading] = useState(false);

  const { farms, loadingFarms, fetchFarms } = useFarms();
  const [henhouses, setHenhouses] = useState<HouseRowModel[]>([]);
  const [chosenFarm, setChosenFarm] = useState<FarmRowModel>();

  const setChosenFarmCallback = useCallback((chosenFarm: FarmRowModel) => {
    setChosenFarm(chosenFarm);
    fetchHouses(chosenFarm.id);
  }, []);

  const fetchHouses = async (farmId: string) => {
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

  const columns: GridColDef[] = [
    { field: "name", headerName: "Nazwa", flex: 1 },
    { field: "code", headerName: "ID Budynku", flex: 1 },
    { field: "area", headerName: "Powierzchnia (m²)", flex: 1 },
    { field: "desc", headerName: "Opis", flex: 1 },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia rekordu",
      type: "dateTime",
      flex: 2,
      valueGetter: (params: any) => {
        return params ? new Date(params) : null;
      },
    },
    {
      field: "actions",
      headerName: "Akcje",
      flex: 2,
      sortable: false,
      filterable: false,
      renderCell: (params: any) => (
        <div className="text-center">
          <Button variant="outlined">Edytuj</Button>
          <Button
            variant="outlined"
            color="error"
            sx={{ ml: 1 }}
            onClick={async () => {
              setLoading(true);
              await handleApiResponse(
                () => FarmsService.deleteHenhouseAsync(params.row.id),
                () => toast.success("Kurnik został usunięty"),
                undefined,
                "Nie udało się usunąć kurnika"
              );
              await fetchHouses(chosenFarm?.id ?? "");
              setLoading(false);
            }}
          >
            Usuń
          </Button>
        </div>
      ),
    },
  ];

  const [openModal, setOpenModal] = useState(false);

  const handleOpen = () => setOpenModal(true);
  const handleClose = async () => {
    setOpenModal(false);
  };

  const handleAddHenhouse = async () => {
    setOpenModal(false);
    await fetchHouses(chosenFarm?.id ?? "");
  };

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
            onClick={handleOpen}
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

      <Box
        mt={4}
        sx={{
          width: "100%",
          overflowX: "auto",
        }}
      >
        <DataGrid
          loading={loading}
          {...data}
          columns={columns}
          localeText={{
            noRowsLabel:
              chosenFarm === undefined ? "Wybierz fermę" : "Brak wpisów",
          }}
          hideFooterPagination={true}
          rowSelection={false}
          showToolbar={false}
          sx={{
            minHeight: henhouses.length === 0 ? 300 : "auto",
            [`& .${tablePaginationClasses.selectLabel}`]: {
              display: "block",
            },
            [`& .${tablePaginationClasses.input}`]: {
              display: "inline-flex",
            },
            "& .MuiDataGrid-columnHeaders": {
              borderTop: "none",
            },
          }}
        />
      </Box>

      <AddHenhouseModal
        farmId={chosenFarm?.id ?? ""}
        open={openModal}
        onClose={handleClose}
        onSave={handleAddHenhouse}
      />
    </Box>
  );
};

export default HousesPage;
