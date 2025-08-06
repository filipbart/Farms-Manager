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
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useEmployees } from "../../../../hooks/employees/useEmployees";

const AdvancesListTab: React.FC = () => {
  const { employees, loading, fetchEmployees } = useEmployees({
    pageSize: -1,
    farmIds: [],
    dateSince: "",
    dateTo: "",
  });

  const navigate = useNavigate();

  useEffect(() => {
    fetchEmployees();
  }, []);

  const handleEmployeeClick = (employeeId: string) => {
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
    <Box mt={3}>
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

export default AdvancesListTab;
