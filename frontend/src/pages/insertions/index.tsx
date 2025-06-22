import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGrid, type GridColDef, type GridRowsProp } from "@mui/x-data-grid";
import { useState } from "react";
import { useDemoData } from "@mui/x-data-grid-generator";
import CustomToolbar from "../../components/datagrid/custom-toolbar";
import NoRowsOverlay from "../../components/datagrid/custom-norows";
import AddInsertionModal from "../../components/modals/insertions/add-insertion-modal";
import SetCycleModal from "../../components/modals/insertions/add-cycle-modal";

const columns: GridColDef[] = [
  { field: "id", headerName: "ID", width: 70 },
  { field: "name", headerName: "Nazwa", flex: 1 },
  { field: "date", headerName: "Data", flex: 1 },
];

const initialRows: GridRowsProp = [
  { id: 1, name: "Wpis A", date: "2025-06-09" },
  { id: 2, name: "Wpis B", date: "2025-06-08" },
  { id: 3, name: "Wpis C", date: "2025-06-07" },
  { id: 4, name: "Testowy", date: "2025-06-06" },
];

const InsertionsPage: React.FC = () => {
  const { data, loading } = useDemoData({
    dataSet: "Commodity",
    rowLength: 100,
    maxColumns: 5,
  });
  const [rows, setRows] = useState(initialRows);
  const [openModal, setOpenModal] = useState(false);
  const [openCycleModal, setOpenCycleModal] = useState(false);

  const handleOpen = () => setOpenModal(true);
  const handleOpenCycle = () => setOpenCycleModal(true);
  const handleClose = () => setOpenModal(false);
  const handleCloseCycle = () => setOpenCycleModal(false);

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
          <Button variant="contained" color="primary" onClick={handleOpen}>
            Dodaj nowy wpis
          </Button>
          <Button variant="outlined" color="primary" onClick={handleOpenCycle}>
            Nowy cykl
          </Button>
        </Box>
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
          localeText={{
            paginationRowsPerPage: "Wierszy na stronę:",
            paginationDisplayedRows: ({ from, to, count }) =>
              `${from} do ${to} z ${count}`,
          }}
          initialState={{
            ...data.initialState,
            pagination: { paginationModel: { pageSize: 5 } },
          }}
          rowSelection={false}
          pageSizeOptions={[5, 10, 25, { value: -1, label: "Wszystkie" }]}
          slots={{ toolbar: CustomToolbar, noRowsOverlay: NoRowsOverlay }}
          // slotProps={{
          //   toolbar: { withSearch: true },
          // }}
          showToolbar
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: {
              display: "block",
            },
            [`& .${tablePaginationClasses.input}`]: {
              display: "inline-flex",
            },
          }}
        />
      </Box>

      <AddInsertionModal open={openModal} onClose={handleClose} />
      <SetCycleModal open={openCycleModal} onClose={handleCloseCycle} />
    </Box>
  );
};

export default InsertionsPage;
