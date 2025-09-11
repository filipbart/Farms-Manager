import React from "react";
import { Tabs, Tab, Typography, Box, Grid } from "@mui/material";
import HatcheriesNotesPanel from "./notes";
import HatcheriesPricesPanel from "./hatcheries-prices";
import HatcheriesNamesTab from "./hatcheries-names-tab";

const HatcheriesNotesPage: React.FC = () => {
  const [value, setValue] = React.useState(0);

  const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
    setValue(newValue);
  };

  const TabPanel = (props: {
    children: React.ReactNode;
    index: number;
    value: number;
  }) => {
    const { children, index, value, ...other } = props;
    return (
      <div
        role="tabpanel"
        hidden={value !== index}
        id={`tab-panel-${index}`}
        aria-labelledby={`tab-${index}`}
        {...other}
      >
        {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
      </div>
    );
  };

  return (
    <Box m={2}>
      <Typography variant="h4" gutterBottom>
        Wylęgarnie - notatki
      </Typography>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={value}
          onChange={handleChange}
          aria-label="hatcheries tabs"
          variant="fullWidth"
        >
          <Tab label="Ceny i Notatki" />
          <Tab label="Nazwy wylęgarni" />
        </Tabs>
      </Box>
      <TabPanel value={value} index={0}>
        <Grid container spacing={4}>
          <Grid size={{ xs: 12, lg: 7 }}>
            <HatcheriesPricesPanel />
          </Grid>
          <Grid size={{ xs: 12, lg: 5 }}>
            <HatcheriesNotesPanel />
          </Grid>
        </Grid>
      </TabPanel>
      <TabPanel value={value} index={1}>
        <HatcheriesNamesTab />
      </TabPanel>
    </Box>
  );
};

export default HatcheriesNotesPage;
