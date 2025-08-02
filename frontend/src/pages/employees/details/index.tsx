import { Box, Tab, Tabs, Typography, CircularProgress } from "@mui/material";
import { createContext, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { EmployeesService } from "../../../services/employees-service";
import type { EmployeeDetailsModel } from "../../../models/employees/employees";
import EmployeeInfoTab from "./tabs/details-info-tab";
import EmployeeFilesTab from "./tabs/files-tab";
import EmployeeRemindersTab from "./tabs/reminders-tab";

type EmployeeContextType = {
  employee: EmployeeDetailsModel | undefined;
  refetch: () => void;
  loading: boolean;
};

export const EmployeeContext = createContext<EmployeeContextType>({
  employee: undefined,
  refetch: () => {},
  loading: false,
});

const EmployeeDetailsPage: React.FC = () => {
  const { id: employeeId } = useParams<{ id: string }>();
  const [employee, setEmployee] = useState<EmployeeDetailsModel>();
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState(0);

  const fetchEmployee = async () => {
    if (!employeeId) return;
    setLoading(true);
    try {
      const response = await EmployeesService.getEmployeeById(employeeId);
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
        <Typography variant="h4" mb={2}>
          {employee.fullName}
        </Typography>

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
