import React from "react";
import { Route, Routes } from "react-router-dom";
import LoginPage from "./pages/login-page";
import DashboardLayout from "./layouts/dashboard/dashboard-layout";
import DashboardPage from "./pages/dashboard";
import { DashboardCtxProvider } from "./pages/dashboard/dashboard-ctx";
import InsertionsPage from "./pages/insertions";

const DefaultRouter: React.FC = () => {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/" element={<DashboardLayout />}>
        <Route
          path=""
          element={
            <DashboardCtxProvider>
              <DashboardPage />
            </DashboardCtxProvider>
          }
        />
        <Route path="/insertions" element={<InsertionsPage />} />
      </Route>
    </Routes>
  );
};

export default DefaultRouter;
