import { useState } from "react";
import {
  Paper,
  Typography,
  Box,
  Collapse,
  IconButton,
  Divider,
} from "@mui/material";
import { MdExpandMore, MdExpandLess } from "react-icons/md";
import type { DashboardChickenHousesStatus } from "../../models/dashboard/dashboard";
import dayjs from "dayjs";

interface ChickenHousesStatusProps {
  data?: DashboardChickenHousesStatus;
}

export const ChickenHousesStatus: React.FC<ChickenHousesStatusProps> = ({
  data,
}) => {
  const [isExpanded, setIsExpanded] = useState(false);

  const calculateDaysSinceInsertion = (insertionDate?: string): number | null => {
    if (!insertionDate) return null;
    const today = dayjs();
    const insertion = dayjs(insertionDate);
    return today.diff(insertion, "day");
  };

  const activeHenhousesCount = data?.farms.reduce((count, farm) => {
    return count + farm.henhouses.filter((h) => h.chickenCount > 0).length;
  }, 0) ?? 0;

  return (
    <Paper
      sx={{
        p: 2,
        height: "100%",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          cursor: "pointer",
        }}
        onClick={() => setIsExpanded(!isExpanded)}
      >
        <Box>
          <Typography variant="h6">
            Aktualnie masz {activeHenhousesCount} kurników w obsadzie
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mt: 0.5 }}>
            Łącznie sztuk w obsadzie:{" "}
            <strong>
              {data?.totalChickenCount.toLocaleString("pl-PL") ?? 0}
            </strong>
          </Typography>
        </Box>
        <IconButton>
          {isExpanded ? <MdExpandLess /> : <MdExpandMore />}
        </IconButton>
      </Box>

      <Collapse in={isExpanded}>
        <Divider sx={{ my: 2 }} />
        <Box sx={{ flexGrow: 1, overflowY: "auto", maxHeight: 400 }}>
          {data?.farms.map((farm) => {
            const activeHenhouses = farm.henhouses.filter(
              (h) => h.chickenCount > 0
            );
            if (activeHenhouses.length === 0) return null;
            return (
              <Box key={farm.name} mb={2}>
                <Typography
                  variant="subtitle1"
                  sx={{ fontWeight: "bold", mb: 0.5 }}
                >
                  {farm.name}
                </Typography>
                <Box sx={{ pl: 2 }}>
                  {activeHenhouses.map((henhouse) => {
                    const daysSinceInsertion = calculateDaysSinceInsertion(
                      henhouse.insertionDate
                    );
                    return (
                      <Typography
                        key={henhouse.name}
                        variant="body2"
                        color="text.secondary"
                        sx={{ mb: 0.5 }}
                      >
                        {henhouse.name}:{" "}
                        <strong>
                          {henhouse.chickenCount.toLocaleString("pl-PL")} szt.
                        </strong>
                        {daysSinceInsertion !== null && (
                          <span style={{ marginLeft: "8px" }}>
                            Doba {daysSinceInsertion}
                          </span>
                        )}
                      </Typography>
                    );
                  })}
                </Box>
              </Box>
            );
          })}
        </Box>
      </Collapse>
    </Paper>
  );
};
