import {
  Avatar,
  Box,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Paper,
  Chip,
  Typography,
  ListItemButton,
} from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import {
  FaFileInvoiceDollar,
  FaReceipt,
  FaUserSlash,
  FaBell,
  FaBellSlash,
} from "react-icons/fa6";
import type { NotificationPriority } from "../../models/common/notifications";
import type {
  NotificationType,
  DashboardNotificationItem,
} from "../../models/dashboard/dashboard";

const notificationIcons: Record<NotificationType, React.ReactElement> = {
  SaleInvoice: <FaFileInvoiceDollar />,
  FeedInvoice: <FaReceipt />,
  EmployeeContract: <FaUserSlash />,
  EmployeeReminder: <FaBell />,
};

const priorityConfig: Record<
  NotificationPriority,
  { label: string; color: "error" | "warning" | "info" }
> = {
  High: { label: "Wysoki", color: "error" },
  Medium: { label: "Średni", color: "warning" },
  Low: { label: "Niski", color: "info" },
};

const getLinkPath = (notification: DashboardNotificationItem): string => {
  switch (notification.type) {
    case "SaleInvoice":
      return `/sales/invoices`;
    case "FeedInvoice":
      return `/feed/invoices`;
    case "EmployeeContract":
    case "EmployeeReminder":
      return `/employees/${notification.sourceId}`;
    default:
      return "#";
  }
};

interface DashboardNotificationsProps {
  notifications: DashboardNotificationItem[];
}

export function DashboardNotifications({
  notifications,
}: DashboardNotificationsProps) {
  return (
    <Paper
      sx={{ p: 2, height: "100%", display: "flex", flexDirection: "column" }}
    >
      <Typography variant="h6" mb={2}>
        Najbliższe terminy i powiadomienia
      </Typography>

      {notifications.length === 0 ? (
        <Box
          sx={{
            flexGrow: 1,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            color: "text.secondary",
            gap: 1,
          }}
        >
          <FaBellSlash size={36} />
          <Typography>Brak nadchodzących terminów</Typography>
        </Box>
      ) : (
        <List sx={{ overflowY: "auto", p: 0 }}>
          {notifications.map((notification, index) => {
            const config = priorityConfig[notification.priority];
            return (
              <ListItem
                key={index}
                secondaryAction={
                  <Chip
                    label={config.label}
                    color={config.color}
                    size="small"
                  />
                }
                disablePadding
              >
                <ListItemButton
                  component={RouterLink}
                  to={getLinkPath(notification)}
                  sx={{ pr: "90px" }}
                >
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: `${config.color}.main` }}>
                      {notificationIcons[notification.type]}
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText
                    primary={notification.description}
                    slotProps={{
                      primary: {
                        fontSize: "0.9rem",
                        whiteSpace: "nowrap",
                        overflow: "hidden",
                        textOverflow: "ellipsis",
                      },
                    }}
                  />
                </ListItemButton>
              </ListItem>
            );
          })}
        </List>
      )}
    </Paper>
  );
}
