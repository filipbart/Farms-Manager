import React from "react";
import { Route, Routes } from "react-router-dom";
import LoginPage from "../pages/login-page";
import DashboardLayout from "../layouts/dashboard/dashboard-layout";
import DashboardPage from "../pages/dashboard";
import { DashboardCtxProvider } from "../pages/dashboard/dashboard-ctx";
import InsertionsPage from "../pages/insertions";
import { useRouter } from "./use-router";
import { RouteName } from "./route-names";
import FarmsPage from "../pages/data/farms";
import HousesPage from "../pages/data/houses";

const DefaultRouter: React.FC = () => {
  const { getRoute } = useRouter();
  return (
    <Routes>
      <Route path={getRoute(RouteName.Login)} element={<LoginPage />} />
      <Route path="/" element={<DashboardLayout />}>
        <Route
          path=""
          element={
            <DashboardCtxProvider>
              <DashboardPage />
            </DashboardCtxProvider>
          }
        />
        <Route
          path={getRoute(RouteName.Insertions)}
          element={<InsertionsPage />}
        />
        <Route path={getRoute(RouteName.Farms)} element={<FarmsPage />} />
        <Route path={getRoute(RouteName.Houses)} element={<HousesPage />} />
      </Route>
    </Routes>
  );
};

export default DefaultRouter;
