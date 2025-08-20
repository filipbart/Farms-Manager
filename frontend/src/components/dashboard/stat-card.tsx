import {
  Avatar,
  Box,
  Card,
  CardContent,
  Typography,
  Tooltip,
} from "@mui/material";
import type { ReactNode } from "react";

interface StatCardProps {
  icon: ReactNode;
  title: string;
  value: string;
  color?: "primary" | "success" | "warning" | "error" | "info";
}

const StatCard: React.FC<StatCardProps> = ({
  icon,
  title,
  value,
  color = "primary",
}) => {
  return (
    <Card sx={{ height: "100%" }}>
      <CardContent sx={{ height: "100%" }}>
        <Box display="flex" alignItems="center" gap={2}>
          <Box sx={{ flex: 1, minWidth: 0 }}>
            <Typography color="text.secondary" variant="overline">
              {title}
            </Typography>
            <Tooltip title={value} placement="top" arrow>
              <Typography
                variant="h4"
                noWrap
                sx={{
                  fontSize: "clamp(1rem, 4vw, 2.125rem)",
                }}
              >
                {value}
              </Typography>
            </Tooltip>
          </Box>
          <Avatar
            sx={{
              backgroundColor: `${color}.main`,
              color: "common.white",
              height: 56,
              width: 56,
              flexShrink: 0,
            }}
          >
            {icon}
          </Avatar>
        </Box>
      </CardContent>
    </Card>
  );
};

export default StatCard;
