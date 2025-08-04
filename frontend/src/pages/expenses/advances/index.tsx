import {
  Box,
  Typography,
  List,
  ListItem,
  ListItemText,
  ListItemButton,
  Paper,
  CircularProgress,
  Divider,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { ExpensesService } from "../../../services/expenses-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";

const ExpenseAdvancesPage: React.FC = () => {
  const [employees, setEmployees] = useState<EAdvanceListModel[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchEmployeesWithAdvances = async () => {
      setLoading(true);
      try {
        // TODO: Zaimplementuj metodę w serwisie do pobierania listy pracowników z zaliczkami
        await handleApiResponse(
          () => ExpensesService.getEmployeesWithAdvances(),
          (data) => {
            setEmployees(data.responseData ?? []);
          },
          undefined,
          "Błąd podczas pobierania ewidencji zaliczek"
        );
      } catch {
        toast.error("Błąd podczas pobierania ewidencji zaliczek");
      } finally {
        setLoading(false);
      }
    };

    fetchEmployeesWithAdvances();
  }, []);

  const handleEmployeeClick = (employeeId: string) => {
    // Przekierowanie do strony ze szczegółami zaliczek danego pracownika
    navigate(`/expenses/advances/${employeeId}`);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>
        Ewidencja zaliczek
      </Typography>

      <Paper sx={{ maxWidth: 700 }} variant="outlined">
        <List disablePadding>
          {employees.length > 0 ? (
            employees.map((employee, index) => (
              <div key={employee.id}>
                <ListItem disablePadding>
                  <ListItemButton
                    onClick={() => handleEmployeeClick(employee.id)}
                  >
                    <ListItemText primary={employee.fullName} />
                  </ListItemButton>
                </ListItem>
                {index < employees.length - 1 && <Divider />}
              </div>
            ))
          ) : (
            <ListItem>
              <ListItemText secondary="Brak pracowników w ewidencji zaliczek." />
            </ListItem>
          )}
        </List>
      </Paper>
    </Box>
  );
};

export default ExpenseAdvancesPage;
