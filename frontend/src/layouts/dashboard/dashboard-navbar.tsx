import React from "react";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import { IconButton, useMediaQuery } from "@mui/material";
import { IoMenu } from "react-icons/io5";

interface DashboardNavbarProps {
  onSidebarOpened: () => void;
}

const DashboardNavbar: React.FC<DashboardNavbarProps> = ({
  onSidebarOpened,
}) => {
  const handleLogout = () => {
    // Implement logout functionality here
    console.log("Logout clicked");
  };

  const lgUp = useMediaQuery((theme: any) => theme.breakpoints.up("lg"), {
    defaultMatches: true,
    noSsr: true,
  });

  return (
    <AppBar
      position="fixed"
      sx={{
        backgroundColor: (theme) => theme.palette.background.default,
        width: lgUp ? `calc(100% - 280px)` : "100%",
      }}
    >
      <Toolbar disableGutters={true} sx={{ px: 2, minHeight: 64, left: 0 }}>
        <IconButton
          onClick={onSidebarOpened}
          edge="start"
          aria-label="open menu"
          sx={{
            mr: 2,
            display: {
              xs: "inline-flex",
              lg: "none",
            },
          }}
        >
          <IoMenu size={22} />
        </IconButton>
        <div style={{ flexGrow: 1 }} />
        <Button sx={{ mr: 2 }} onClick={handleLogout} variant="text">
          Wyloguj
        </Button>
        <Avatar alt="User Avatar" />
      </Toolbar>
    </AppBar>
  );
};

export default DashboardNavbar;
