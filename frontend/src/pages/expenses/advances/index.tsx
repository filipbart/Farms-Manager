import { Box, Tab, Tabs, Typography } from "@mui/material";
import { useState } from "react";
import AdvanceCategoriesTab from "./tabs/categories-tab";
import AdvancesListTab from "./tabs/list-tab";

const ExpenseAdvancesPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState(0);

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  return (
    <Box p={4}>
      <Typography variant="h4" mb={2}>
        Ewidencja zaliczek
      </Typography>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs value={activeTab} onChange={handleTabChange} variant="fullWidth">
          <Tab label="Lista pracowników" />
          <Tab label="Kategorie zaliczek" />
        </Tabs>
      </Box>

      {activeTab === 0 && <AdvancesListTab />}
      {activeTab === 1 && <AdvanceCategoriesTab />}
    </Box>
  );
};

export default ExpenseAdvancesPage;
