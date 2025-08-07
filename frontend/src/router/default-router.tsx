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
import SalesPage from "../pages/sales";
import SlaughterhousesPage from "../pages/data/slaughterhouse";
import SaleFieldsSettingsPage from "../pages/sales/fields-settings";
import FeedsPricePage from "../pages/feeds/prices";
import FeedsDeliveriesPage from "../pages/feeds/deliveries";
import FeedsPaymentsPage from "../pages/feeds/payments";
import ExpensesTypesPage from "../pages/expenses/types";
import ExpensesContractorsPage from "../pages/expenses/contractors";
import ExpenseProductionPage from "../pages/expenses/production";
import ProductionDataFailuresPage from "../pages/production-data/failures";
import ProductionDataRemainingFeedPage from "../pages/production-data/remaining-feed";
import ProductionDataTransferFeedPage from "../pages/production-data/transfer-feed";
import ProductionDataWeighingsPage from "../pages/production-data/weighings";
import GasDeliveriesPage from "../pages/gas/deliveries";
import GasConsumptionsPage from "../pages/gas/consumptions";
import HatcheriesNotesPage from "../pages/hatcheries-notes";
import EmployeesPage from "../pages/employees";
import EmployeeDetailsPage from "../pages/employees/details";
import EmployeePayslipsPage from "../pages/employees/payslips";
import UserProfilePage from "../pages/user-profile";
import SalesInvoicesPage from "../pages/sales/invoices";
import SettingsCyclesPage from "../pages/settings/cycle-settings";
import ExpenseAdvancesPage from "../pages/expenses/advances";
import ExpenseAdvanceDetailsPage from "../pages/expenses/advances/details";
import UtilizationPlantsPage from "../pages/data/utilization-plants";

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
          path={getRoute(RouteName.UtilizationPlants)}
          element={<UtilizationPlantsPage />}
        />
        <Route
          path={getRoute(RouteName.UserProfile)}
          element={<UserProfilePage />}
        />
        <Route
          path={getRoute(RouteName.SalesInvoices)}
          element={<SalesInvoicesPage />}
        />
        <Route
          path={getRoute(RouteName.SalesFieldsSettings)}
          element={<SaleFieldsSettingsPage />}
        />
        <Route
          path={getRoute(RouteName.FeedsDeliveries)}
          element={<FeedsDeliveriesPage />}
        />
        <Route
          path={getRoute(RouteName.FeedsPrices)}
          element={<FeedsPricePage />}
        />
        <Route
          path={getRoute(RouteName.FeedsPayments)}
          element={<FeedsPaymentsPage />}
        />
        <Route
          path={getRoute(RouteName.ExpensesProduction)}
          element={<ExpenseProductionPage />}
        />
        <Route
          path={getRoute(RouteName.ExpensesAdvances)}
          element={<ExpenseAdvancesPage />}
        />
        <Route
          path={getRoute(RouteName.ExpensesAdvancesDetails)}
          element={<ExpenseAdvanceDetailsPage />}
        />
        <Route
          path={getRoute(RouteName.ExpensesTypes)}
          element={<ExpensesTypesPage />}
        />
        <Route
          path={getRoute(RouteName.ExpensesContractors)}
          element={<ExpensesContractorsPage />}
        />
        <Route
          path={getRoute(RouteName.ProductionDataFailures)}
          element={<ProductionDataFailuresPage />}
        />
        <Route
          path={getRoute(RouteName.ProductionDataRemainingFeed)}
          element={<ProductionDataRemainingFeedPage />}
        />
        <Route
          path={getRoute(RouteName.ProductionDataTransferFeed)}
          element={<ProductionDataTransferFeedPage />}
        />
        <Route
          path={getRoute(RouteName.ProductionDataWeighings)}
          element={<ProductionDataWeighingsPage />}
        />
        <Route
          path={getRoute(RouteName.GasDeliveries)}
          element={<GasDeliveriesPage />}
        />
        <Route
          path={getRoute(RouteName.GasConsumptions)}
          element={<GasConsumptionsPage />}
        />
        <Route
          path={getRoute(RouteName.HatcheriesNotes)}
          element={<HatcheriesNotesPage />}
        />
        <Route
          path={getRoute(RouteName.Employees)}
          element={<EmployeesPage />}
        />
        <Route
          path={getRoute(RouteName.EmployeesDetails)}
          element={<EmployeeDetailsPage />}
        />
        <Route
          path={getRoute(RouteName.EmployeesPayslips)}
          element={<EmployeePayslipsPage />}
        />
        <Route
          path={getRoute(RouteName.SettingsCycles)}
          element={<SettingsCyclesPage />}
        />
      </Route>
    </Routes>
  );
};

export default DefaultRouter;
