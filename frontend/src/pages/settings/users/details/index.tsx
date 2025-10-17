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
import { UsersService } from "../../../../services/users-service";
import type { UserDetailsModel } from "../../../../models/users/users";
import UserInfoTab from "./tabs/info-tab";
import PermissionsTab from "./tabs/permissions-tab";
import FarmsTab from "./tabs/farms-tab";
import ExpenseAdvancesTab from "./tabs/expense-advances-tab";

const UserDetailsPage: React.FC = () => {
  const { userId } = useParams<{ userId: string }>();
  const [user, setUser] = useState<UserDetailsModel>();
  const [loading, setLoading] = useState(true);
  const nav = useNavigate();

  const [activeTab, setActiveTab] = useState(0);

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  const fetchUser = async () => {
    if (!userId) return;
    setLoading(true);
    try {
      const response = await UsersService.getUserDetails(userId);
      if (response.success) {
        setUser(response.responseData);
      } else {
        setUser(undefined);
      }
    } catch (err) {
      console.error("Błąd podczas pobierania danych użytkownika", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUser();
  }, [userId]);

  if (loading && !user) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  if (!user) {
    return <Typography p={5}>Nie znaleziono użytkownika.</Typography>;
  }

  return (
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
          Szczegóły użytkownika: {user.name}
        </Typography>

        <Button
          variant="outlined"
          color="inherit"
          onClick={() => nav("/settings/users")}
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
          <Tab label="Informacje o użytkowniku" />
          <Tab label="Dostęp do ferm" />
          <Tab label="Uprawnienia" />
          <Tab label="Ewidencje zaliczek" />
        </Tabs>
      </Box>

      <Box mt={3}>
        {activeTab === 0 && <UserInfoTab user={user} refetch={fetchUser} />}
        {activeTab === 1 && <FarmsTab user={user} refetch={fetchUser} />}
        {activeTab === 2 && <PermissionsTab user={user} refetch={fetchUser} />}
        {activeTab === 3 && <ExpenseAdvancesTab user={user} refetch={fetchUser} />}
      </Box>
    </Box>
  );
};

export default UserDetailsPage;
