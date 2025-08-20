import { Avatar, Box, Card, CardContent, Typography } from "@mui/material";
import type { ReactNode } from "react";

interface StatCardProps {
  icon: ReactNode;
  title: string;
  value: string;
  trend?: string;
  color?: "primary" | "success" | "warning" | "error" | "info";
}

const StatCard: React.FC<StatCardProps> = ({
  icon,
  title,
  value,
  trend,
  color = "primary",
}) => {
  return (
    <Card sx={{ height: "100%" }}>
      <CardContent>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Box>
            <Typography color="text.secondary" variant="overline">
              {title}
            </Typography>
            <Typography variant="h4">{value}</Typography>
          </Box>
          <Avatar
            sx={{
              backgroundColor: `${color}.main`,
              height: 56,
              width: 56,
            }}
          >
            {icon}
          </Avatar>
        </Box>
        {trend && (
          <Typography color="text.secondary" variant="caption" mt={1}>
            {trend}
          </Typography>
        )}
      </CardContent>
    </Card>
  );
};

export default StatCard;
