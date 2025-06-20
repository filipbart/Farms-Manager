import React, { useState } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../auth/useAuth";
import { styled } from "@mui/material";
import DashboardNavbar from "./dashboard-navbar";
import DashboardSidebar from "./dashboard-sidebar";
import { ErrorBoundary } from "react-error-boundary";
import ErrorFallback from "../../pages/error-fallback";
import { useRouter } from "../../router/use-router";
import { RouteName } from "../../router/route-names";

const DashboardLayoutRoot = styled("div")(({ theme }) => ({
  maxWidth: "100%",
  height: "100%",
  paddingTop: 64,
  [theme.breakpoints.up("lg")]: {
    paddingLeft: 280,
  },

  backgroundColor: theme.palette.background.default,
}));

const DashboardLayout: React.FC = () => {
  const [sidebarOpened, setSidebarOpened] = useState(false);
  const { getRoute } = useRouter();
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated()) {
    return <Navigate to={getRoute(RouteName.Login)} />;
  }

  return (
    <>
      <DashboardLayoutRoot>
        <ErrorBoundary FallbackComponent={ErrorFallback}>
          <Outlet />
        </ErrorBoundary>
      </DashboardLayoutRoot>
      <DashboardNavbar onSidebarOpened={() => setSidebarOpened(true)} />
      <DashboardSidebar
        open={sidebarOpened}
        onClose={() => setSidebarOpened(false)}
      />
    </>
  );
};

export default DashboardLayout;
