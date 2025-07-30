import React from "react";
import { Tabs, Tab, Typography, Box } from "@mui/material";
import ProductionDataWeighingsTab from "./tabs/weighings-list";
import WeightStandardsTab from "./tabs/weight-standards";

const ProductionDataWeighingsPage: React.FC = () => {
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
        {value === index && <Box p={3}>{children}</Box>}
      </div>
    );
  };

  return (
    <Box m={2}>
      <Typography variant="h4" gutterBottom>
        Masy ciała
      </Typography>
      <Box position="static" sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs value={value} onChange={handleChange} variant="fullWidth">
          <Tab
            label={
              <Typography variant="subtitle1" fontWeight={600}>
                Lista
              </Typography>
            }
          />
          <Tab
            label={
              <Typography variant="subtitle1" fontWeight={600}>
                Normy mas
              </Typography>
            }
          />
        </Tabs>
      </Box>
      <TabPanel value={value} index={0}>
        <ProductionDataWeighingsTab />
      </TabPanel>
      <TabPanel value={value} index={1}>
        <WeightStandardsTab />
      </TabPanel>
    </Box>
  );
};

export default ProductionDataWeighingsPage;
