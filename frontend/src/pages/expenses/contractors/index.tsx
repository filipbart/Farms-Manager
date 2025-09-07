import { Box, Button, tablePaginationClasses, Typography } from "@mui/material";
import { DataGridPro } from "@mui/x-data-grid-pro";
import { useEffect, useMemo, useState } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { getExpensesContractorsColumns } from "./expenses-contractors-columns";
import { ExpensesService } from "../../../services/expenses-service";
import { toast } from "react-toastify";
import { useExpensesContractor } from "../../../hooks/expenses/useExpensesContractors";
import type { ExpenseContractorRow } from "../../../models/expenses/expenses-contractors";
import AddExpenseContractorModal from "../../../components/modals/expenses/contractors/add-expense-contractor-modal";
import NoRowsOverlay from "../../../components/datagrid/custom-norows";
import EditExpenseContractorModal from "../../../components/modals/expenses/contractors/edit-expense-contractor-modal";

const ExpensesContractorsPage: React.FC = () => {
  const {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  } = useExpensesContractor();

  const [openModal, setOpenModal] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [selectedExpenseContractor, setSelectedExpenseContractor] = useState<
    ExpenseContractorRow | undefined
  >(undefined);

  const [paginationModel, setPaginationModel] = useState(() => {
    const savedPageSize = localStorage.getItem("expensesContractorsPageSize");
    return {
      page: 0,
      pageSize: savedPageSize ? parseInt(savedPageSize, 10) : 10,
    };
  });

  const handleOpen = () => setOpenModal(true);
  const handleClose = async () => {
    setOpenModal(false);
  };

  const handleAddExpenseContractor = async () => {
    setOpenModal(false);
    await fetchExpensesContractors();
  };

  const deleteExpenseContractor = async (id: string) => {
    await handleApiResponse(
      () => ExpensesService.deleteExpenseContractor(id),
      async () => {
        toast.success("Kontrahent został poprawnie usunięty");
        fetchExpensesContractors();
      },
      undefined,
      "Błąd podczas usuwania kontrahenta"
    );
  };

  useEffect(() => {
    fetchExpensesContractors();
  }, [fetchExpensesContractors]);

  const columns = useMemo(
    () =>
      getExpensesContractorsColumns({
        setSelectedExpenseContractor,
        deleteExpenseContractor,
        setIsEditModalOpen,
      }),
    []
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
        <Typography variant="h4">Kontrahenci</Typography>
        <Box display="flex" gap={2}>
          <Button variant="contained" color="primary" onClick={handleOpen}>
            Dodaj kontrahenta
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
        <DataGridPro
          loading={loadingExpensesContractors}
          rows={expensesContractors}
          columns={columns}
          initialState={{
            columns: {
              columnVisibilityModel: { id: false, dateCreatedUtc: false },
            },
          }}
          slots={{ noRowsOverlay: NoRowsOverlay }}
          rowSelection={false}
          showToolbar={false}
          pagination
          paginationModel={paginationModel}
          onPaginationModelChange={(newModel) => {
            localStorage.setItem(
              "expensesContractorsPageSize",
              newModel.pageSize.toString()
            );
            setPaginationModel(newModel);
          }}
          pageSizeOptions={[5, 10, 25, 100, { value: -1, label: "Wszystkie" }]}
          sx={{
            [`& .${tablePaginationClasses.selectLabel}`]: { display: "block" },
            [`& .${tablePaginationClasses.input}`]: { display: "inline-flex" },
            "& .aggregated-row": {
              fontWeight: "bold",

              "& .MuiDataGrid-cell": {
                borderTop: "1px solid rgba(224, 224, 224, 1)",
                backgroundColor: "rgba(240, 240, 240, 0.7)",
              },
            },
            "& .MuiDataGrid-columnHeaders": {
              borderTop: "none",
            },
          }}
        />
      </Box>

      <EditExpenseContractorModal
        open={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        onSave={() => {
          setIsEditModalOpen(false);
          fetchExpensesContractors();
        }}
        contractorToEdit={selectedExpenseContractor}
      />

      <AddExpenseContractorModal
        open={openModal}
        onClose={handleClose}
        onSave={handleAddExpenseContractor}
      />
    </Box>
  );
};

export default ExpensesContractorsPage;
