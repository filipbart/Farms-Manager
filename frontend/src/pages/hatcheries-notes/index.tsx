import { Box, Grid, Typography } from "@mui/material";
import HatcheriesPricesPanel from "./hatcheries-prices";
import HatcheriesNotesPanel from "./notes";

const HatcheriesNotesPage: React.FC = () => {
  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>
        Wylęgarnie
      </Typography>
      <Grid container spacing={4}>
        <Grid size={{ xs: 12, lg: 7 }}>
          <HatcheriesPricesPanel />
        </Grid>

        <Grid size={{ xs: 12, lg: 5 }}>
          <HatcheriesNotesPanel />
        </Grid>
      </Grid>
    </Box>
  );
};

export default HatcheriesNotesPage;
