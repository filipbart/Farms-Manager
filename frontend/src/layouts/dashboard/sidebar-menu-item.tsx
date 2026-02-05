import {
  Collapse,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  styled,
  Box,
  useTheme,
} from "@mui/material";
import { type ReactNode } from "react";
import { MdExpandLess, MdExpandMore } from "react-icons/md";
import { Link, useLocation } from "react-router-dom";
import { NotificationPriority } from "../../models/common/notifications";
import { useAuth } from "../../auth/useAuth";
import { RouteName } from "../../router/route-names";
import { useRouter } from "../../router/use-router";

interface SidebarMenuItemProps {
  children?: any;
  icon?: ReactNode;
  title: string;
  to: string;
  isOpen?: boolean;
  onClick?: () => void;
  notificationCount?: number;
  notificationPriority?: NotificationPriority;
  badgeLabel?: string;
}

const ListItemStyle: any = styled((props) => (
  <ListItemButton
    disableGutters
    {...props}
    sx={{
      paddingRight: 2,
      "&.Mui-selected": {
        backgroundColor: (theme) => theme.palette.primary.main,
        color: (theme) => theme.palette.primary.contrastText,
      },
    }}
  />
))(({ theme }) => ({
  ...theme.typography.body2,
  height: 48,
  position: "relative",
  textTransform: "capitalize",
  color: theme.palette.secondary.main,
  "&:hover": {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.primary.contrastText,
  },
}));

const ListItemIconStyle: any = styled(ListItemIcon)({
  width: 22,
  height: 22,
  color: "inherit",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
});

const SidebarMenuItem: React.FC<SidebarMenuItemProps> = ({
  children,
  icon,
  title,
  to,
  isOpen,
  onClick,
  notificationCount,
  notificationPriority,
  badgeLabel,
}) => {
  const location = useLocation();
  const theme = useTheme();

  const { userData } = useAuth();
  const { getRoute } = useRouter();

  const userProfilePath = getRoute(RouteName.UserProfile);
  const isDisabled = !!userData?.mustChangePassword && to !== userProfilePath;

  const getBadgeColor = () => {
    switch (notificationPriority) {
      case NotificationPriority.High:
        return theme.palette.error.main;
      case NotificationPriority.Medium:
        return "#F57C00";
      case NotificationPriority.Low:
        return "#FBC02D";
      default:
        return "transparent";
    }
  };

  const renderBadge = () => {
    if (!notificationCount || notificationCount === 0) {
      return null;
    }
    return (
      <Box
        sx={{
          width: 22,
          height: 22,
          borderRadius: "50%",
          backgroundColor: getBadgeColor(),
          color: theme.palette.getContrastText(getBadgeColor()),
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          fontSize: "0.75rem",
          fontWeight: "bold",
          ml: 1,
          marginRight: 1,
        }}
      >
        {notificationCount}
      </Box>
    );
  };

  const renderLabel = () => {
    if (!badgeLabel) {
      return null;
    }
    return (
      <Box
        sx={{
          px: 1.25,
          py: 0.3,
          borderRadius: 999,
          backgroundColor: theme.palette.error.main,
          color: theme.palette.getContrastText(theme.palette.error.main),
          fontSize: "0.7rem",
          fontWeight: "bold",
          lineHeight: 1,
          ml: 1,
          marginRight: 1,
        }}
      >
        {badgeLabel}
      </Box>
    );
  };

  if (!children) {
    return (
      <ListItemStyle
        selected={location.pathname === to}
        disabled={isDisabled}
        component={Link}
        to={to}
      >
        {icon && <ListItemIconStyle>{icon}</ListItemIconStyle>}
        <ListItemText
          disableTypography
          primary={title}
          sx={{
            flexGrow: 1,
            fontSize: "1rem",
            fontWeight: location.pathname === to ? "bold" : "normal",
          }}
        />
        {renderLabel()}
        {renderBadge()}
      </ListItemStyle>
    );
  }

  const childrenActive = Array.isArray(children)
    ? !!children.find((child: any) => location.pathname === child?.props?.to)
    : false;

  return (
    <>
      <ListItemStyle
        onClick={onClick}
        selected={childrenActive || location.pathname === to}
      >
        {icon && <ListItemIconStyle>{icon}</ListItemIconStyle>}
        <ListItemText
          disableTypography
          primary={title}
          sx={{
            flexGrow: 1,
            fontSize: "1rem",
            fontWeight:
              childrenActive || location.pathname === to ? "bold" : "normal",
          }}
        />
        {renderLabel()}
        {renderBadge()}
        {isOpen ? <MdExpandLess /> : <MdExpandMore />}
      </ListItemStyle>
      <Collapse in={isOpen} timeout="auto" unmountOnExit>
        <List sx={{ pl: 2 }} component="div" disablePadding>
          {children}
        </List>
      </Collapse>
    </>
  );
};

export default SidebarMenuItem;
