import {
  Box,
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
import { ExpenseAdvancePermissionsService } from "../../../../services/expense-advance-permissions-service";
import type { ExpenseAdvanceEntity } from "../../../../models/expenses/advances/expense-advance-permissions";
import { toast } from "react-toastify";

const AdvancesListTab: React.FC = () => {
  const [employees, setEmployees] = useState<ExpenseAdvanceEntity[]>([]);
  const [loading, setLoading] = useState(false);
  const nav = useNavigate();

  const fetchEmployees = async () => {
    setLoading(true);
    try {
      const response = await ExpenseAdvancePermissionsService.getAllExpenseAdvances();
      if (response.success) {
        const data = response.responseData as any;
        // Backend może zwrócić {expenseAdvances: [...]} lub bezpośrednio tablicę
        const employeesList = data?.expenseAdvances || data;
        setEmployees(Array.isArray(employeesList) ? employeesList : []);
      } else {
        setEmployees([]);
      }
    } catch (err) {
      console.error("Błąd podczas pobierania listy pracowników", err);
      toast.error("Nie udało się pobrać listy pracowników");
      setEmployees([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployees();
  }, []);

  const handleEmployeeClick = (employeeId: string) => {
    nav(`/expenses/advances/${employeeId}`);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box mt={3}>
      <Paper sx={{ maxWidth: 700 }} variant="outlined">
        <List disablePadding>
          {employees.length > 0 ? (
            employees.map((employee, index) => (
              <div key={employee.id}>
                <ListItem disablePadding>
                  <ListItemButton
                    onClick={() => handleEmployeeClick(employee.employeeId)}
                  >
                    <ListItemText primary={employee.employeeName} />
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

export default AdvancesListTab;
