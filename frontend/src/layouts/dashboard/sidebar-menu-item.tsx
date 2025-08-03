import {
  Collapse,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  styled,
} from "@mui/material";
import { type ReactNode } from "react";
import { MdExpandLess, MdExpandMore } from "react-icons/md";
import { Link, useLocation } from "react-router-dom";

interface SidebarMenuItemProps {
  children?: any;
  icon?: ReactNode;
  title: string;
  to: string;
  isOpen?: boolean;
  onClick?: () => void;
}

const ListItemStyle: any = styled((props) => (
  <ListItemButton
    disableGutters
    {...props}
    sx={{
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
}) => {
  const location = useLocation();

  if (!children) {
    return (
      <ListItemStyle
        selected={location.pathname === to}
        component={Link}
        to={to}
      >
        {icon && <ListItemIconStyle>{icon}</ListItemIconStyle>}
        <ListItemText
          disableTypography
          primary={title}
          sx={{
            fontSize: "1rem",
            fontWeight: location.pathname === to ? "bold" : "normal",
          }}
        />
      </ListItemStyle>
    );
  }

  let childrenActive = false;
  if (Array.isArray(children)) {
    childrenActive = !!children.find(
      (child: any) => location.pathname === child.props.to
    );
  }

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
            fontSize: "1rem",
            fontWeight:
              childrenActive || location.pathname === to ? "bold" : "normal",
          }}
        />
        {isOpen ? (
          <MdExpandLess className="mr-3" />
        ) : (
          <MdExpandMore className="mr-3" />
        )}
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
