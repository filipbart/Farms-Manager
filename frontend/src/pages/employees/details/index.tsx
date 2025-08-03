import {
  Box,
  Tab,
  Tabs,
  Typography,
  CircularProgress,
  Button,
} from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { EmployeesService } from "../../../services/employees-service";
import type { EmployeeDetailsModel } from "../../../models/employees/employees";
import EmployeeInfoTab from "./tabs/details-info-tab";
import EmployeeFilesTab from "./tabs/files-tab";
import EmployeeRemindersTab from "./tabs/reminders-tab";
import { EmployeeContext } from "../../../context/employee-context";

const EmployeeDetailsPage: React.FC = () => {
  const { employeeId } = useParams();
  const [employee, setEmployee] = useState<EmployeeDetailsModel>();
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState(0);
  const nav = useNavigate();

  const fetchEmployee = async () => {
    if (!employeeId) return;
    setLoading(true);
    try {
      const response = await EmployeesService.getEmployeeDetails(employeeId);
      if (response.success) {
        setEmployee(response.responseData);
      } else {
        setEmployee(undefined);
      }
    } catch (err) {
      console.error("Błąd podczas pobierania danych pracownika", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployee();
  }, [employeeId]);

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  if (loading && !employee) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  if (!employee) {
    return <Typography p={5}>Nie znaleziono pracownika.</Typography>;
  }

  return (
    <EmployeeContext.Provider
      value={{
        employee,
        refetch: fetchEmployee,
        loading,
      }}
    >
      <Box className="m-5 p-5 xs:m-3 xs:p-3 text-darkfont">
        <Box
          mb={2}
          display="flex"
          flexDirection={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          alignItems={{ xs: "flex-start", sm: "center" }}
          gap={2}
        >
          <Typography variant="h4" mb={2}>
            Szczegóły pracownika {employee.fullName}
          </Typography>

          <Button
            variant="outlined"
            color="inherit"
            onClick={() => nav("/employees")}
          >
            Cofnij do listy
          </Button>
        </Box>
        <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
          <Tabs
            value={activeTab}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
          >
            <Tab label="Informacje o pracowniku" />
            <Tab label="Pliki" />
            <Tab label="Przypomnienia" />
          </Tabs>
        </Box>

        <Box mt={3}>
          {activeTab === 0 && <EmployeeInfoTab />}
          {activeTab === 1 && <EmployeeFilesTab />}
          {activeTab === 2 && <EmployeeRemindersTab />}
        </Box>
      </Box>
    </EmployeeContext.Provider>
  );
};

export default EmployeeDetailsPage;
