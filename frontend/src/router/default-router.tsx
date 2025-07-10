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
import HatcheriesPage from "../pages/data/hatcheries";
import SettingsPageTab from "../pages/settings";
import SalesPage from "../pages/sales";
import SlaughterhousesPage from "../pages/data/slaughterhouse";
import SaleFieldsSettingsPage from "../pages/sales/fields-settings";
import FeedsPricePage from "../pages/feeds/prices";
import FeedsDeliversPage from "../pages/feeds/delivers";

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
        <Route path={getRoute(RouteName.SalesList)} element={<SalesPage />} />

        <Route path={getRoute(RouteName.Farms)} element={<FarmsPage />} />
        <Route path={getRoute(RouteName.Houses)} element={<HousesPage />} />
        <Route
          path={getRoute(RouteName.Hatcheries)}
          element={<HatcheriesPage />}
        />
        <Route
          path={getRoute(RouteName.Slaughterhouses)}
          element={<SlaughterhousesPage />}
        />
        <Route
          path={getRoute(RouteName.Settings)}
          element={<SettingsPageTab />}
        />
        <Route
          path={getRoute(RouteName.SalesFieldsSettings)}
          element={<SaleFieldsSettingsPage />}
        />
        <Route
          path={getRoute(RouteName.FeedsDelivers)}
          element={<FeedsDeliversPage />}
        />
        <Route
          path={getRoute(RouteName.FeedsPrices)}
          element={<FeedsPricePage />}
        />
      </Route>
    </Routes>
  );
};

export default DefaultRouter;
