import React, { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import {
  Box,
  Typography,
  List,
  Drawer,
  useMediaQuery,
  Avatar,
} from "@mui/material";
import LogoWhite from "../../assets/logo_white.png";
import SidebarMenuItem from "./sidebar-menu-item";
import { useAuth } from "../../auth/useAuth";
import { useNotifications } from "../../context/notification-context";
import { IoCard, IoHome } from "react-icons/io5";
import {
  FaAddressCard,
  FaBuilding,
  FaBurn,
  FaCalculator,
  FaChartLine,
  FaClone,
  FaDatabase,
  FaFileInvoiceDollar,
  FaHandHoldingUsd,
  FaHandshake,
  FaIndustry,
  FaList,
  FaMoneyBill,
  FaTruck,
  FaUserCog,
  FaWarehouse,
  FaWeight,
} from "react-icons/fa";
import {
  FaArrowTrendUp,
  FaBuildingWheat,
  FaGear,
  FaHouse,
  FaJarWheat,
} from "react-icons/fa6";
import {
  MdBarChart,
  MdCalendarViewWeek,
  MdDashboard,
  MdFactory,
  MdNotes,
  MdPayments,
  MdPeopleAlt,
  MdPropane,
  MdSettings,
  MdTimelapse,
  MdTrendingDown,
} from "react-icons/md";
import { PiFarmFill } from "react-icons/pi";
import { usePermissions } from "../../context/permission-context";

interface DashboardSidebarProps {
  open: boolean;
  onClose: () => void;
}

const DashboardSidebar: React.FC<DashboardSidebarProps> = ({
  open,
  onClose,
}) => {
  const location = useLocation();
  const lgUp = useMediaQuery((theme: any) => theme.breakpoints.up("lg"));
  const auth = useAuth();
  const { notifications } = useNotifications();
  const { hasPermission } = usePermissions();

  const [openItems, setOpenItems] = useState<string[]>([]);

  const handleItemClick = (title: string, isParent: boolean = false) => {
    setOpenItems((prev) => {
      const isOpen = prev.includes(title);

      if (isParent) {
        return isOpen ? [] : [title];
      } else {
        return isOpen
          ? prev.filter((item) => item !== title)
          : [...prev, title];
      }
    });
  };

  useEffect(() => {
    const path = location.pathname;

    if (
      path.startsWith("/production-data/weighings") ||
      path.startsWith("/production-data/flock-loss")
    ) {
      setOpenItems(["Dane produkcyjne", "Pomiary cotygodniowe"]);
    } else if (path.startsWith("/sales")) {
      setOpenItems(["Sprzedaże"]);
    } else if (path.startsWith("/feeds")) {
      setOpenItems(["Pasze"]);
    } else if (path.startsWith("/expenses")) {
      setOpenItems(["Koszty"]);
    } else if (path.startsWith("/production-data")) {
      setOpenItems(["Dane produkcyjne"]);
    } else if (path.startsWith("/gas")) {
      setOpenItems(["Gaz"]);
    } else if (path.startsWith("/employees")) {
      setOpenItems(["Pracownicy"]);
    } else if (path.startsWith("/data")) {
      setOpenItems(["Dane"]);
    } else if (path.startsWith("/settings")) {
      setOpenItems(["Ustawienia"]);
    } else if (path.startsWith("/summary")) {
      setOpenItems(["Podsumowanie"]);
    } else {
      setOpenItems([]);
    }
  }, [location.pathname]);

  const headingStyles = {
    px: 2,
    pt: 3,
    pb: 1,
    color: "neutral.400",
    fontSize: "0.75rem",
    fontWeight: "bold",
    textTransform: "uppercase",
  };

  const contentList = (
    <List sx={{ width: "100%", backgroundColor: "primary.dark" }}>
      <Typography sx={headingStyles}>Główne</Typography>
      {hasPermission("dashboard:view") && (
        <SidebarMenuItem to="/" title="Strona główna" icon={<IoHome />} />
      )}

      <Typography sx={headingStyles}>Produkcja i Finanse</Typography>
      {hasPermission("accounting:view") && (
        <SidebarMenuItem
          to="/accounting"
          title="Księgowość"
          icon={<FaFileInvoiceDollar />}
          badgeLabel="Nowa"
          notificationCount={notifications?.accountingInvoices?.count}
          notificationPriority={notifications?.accountingInvoices?.priority}
        />
      )}
      {hasPermission("insertions:view") && (
        <SidebarMenuItem
          to="/insertions"
          title="Wstawienia"
          icon={<FaClone />}
        />
      )}
      {hasPermission("sales:view") && (
        <SidebarMenuItem
          to="/sales"
          title="Sprzedaże"
          icon={<IoCard />}
          isOpen={openItems.includes("Sprzedaże")}
          onClick={() => handleItemClick("Sprzedaże", true)}
          notificationCount={notifications?.salesInvoices.count}
          notificationPriority={notifications?.salesInvoices.priority}
        >
          <SidebarMenuItem to="/sales" title="Lista" icon={<FaList />} />
          {hasPermission("sales:invoices:view") && (
            <SidebarMenuItem
              to="/sales/invoices"
              title="Faktury sprzedażowe"
              icon={<FaFileInvoiceDollar />}
              notificationCount={notifications?.salesInvoices.count}
              notificationPriority={notifications?.salesInvoices.priority}
            />
          )}
          {hasPermission("sales:settings:manage") && (
            <SidebarMenuItem
              to="/sales/settings"
              title="Ustawienia pól"
              icon={<FaGear />}
            />
          )}
        </SidebarMenuItem>
      )}
      {hasPermission("feeds:view") && (
        <SidebarMenuItem
          to="/feeds"
          title="Pasze"
          icon={<FaJarWheat />}
          isOpen={openItems.includes("Pasze")}
          onClick={() => handleItemClick("Pasze", true)}
          notificationCount={notifications?.feedDeliveries.count}
          notificationPriority={notifications?.feedDeliveries.priority}
        >
          {hasPermission("feeds:deliveries:view") && (
            <SidebarMenuItem
              to="/feeds/deliveries"
              title="Dostawy pasz"
              icon={<FaTruck />}
              notificationCount={notifications?.feedDeliveries.count}
              notificationPriority={notifications?.feedDeliveries.priority}
            />
          )}
          {hasPermission("feeds:prices:view") && (
            <SidebarMenuItem
              to="/feeds/prices"
              title="Ceny pasz"
              icon={<FaArrowTrendUp />}
            />
          )}
          {hasPermission("feeds:payments:view") && (
            <SidebarMenuItem
              to="/feeds/payments"
              title="Przelewy"
              icon={<FaFileInvoiceDollar />}
            />
          )}
        </SidebarMenuItem>
      )}
      {hasPermission("expenses:view") && (
        <SidebarMenuItem
          to="/expenses"
          title="Koszty"
          icon={<FaMoneyBill />}
          isOpen={openItems.includes("Koszty")}
          onClick={() => handleItemClick("Koszty", true)}
        >
          {hasPermission("expenses:production:view") && (
            <SidebarMenuItem
              to="/expenses/production"
              title="Koszty produkcyjne"
              icon={<MdFactory />}
            />
          )}
          {hasPermission("expenses:advances:view") && (
            <SidebarMenuItem
              to="/expenses/advances"
              title="Ewidencja zaliczek"
              icon={<FaHandHoldingUsd />}
            />
          )}
          {hasPermission("expenses:contractors:view") && (
            <SidebarMenuItem
              to="/expenses/contractors"
              title="Kontrahenci"
              icon={<FaHandshake />}
            />
          )}
          {hasPermission("expenses:types:manage") && (
            <SidebarMenuItem
              to="/expenses/types"
              title="Typy wydatków"
              icon={<MdPayments />}
            />
          )}
        </SidebarMenuItem>
      )}

      <Typography sx={headingStyles}>Operacje</Typography>
      {hasPermission("productiondata:view") && (
        <SidebarMenuItem
          to="/production-data"
          title="Dane produkcyjne"
          icon={<FaIndustry />}
          isOpen={openItems.includes("Dane produkcyjne")}
          onClick={() => handleItemClick("Dane produkcyjne", true)}
        >
          {hasPermission("productiondata:failures:view") && (
            <SidebarMenuItem
              to="/production-data/failures"
              title="Upadki i wybrakowania"
              icon={<MdTrendingDown />}
            />
          )}
          {hasPermission("productiondata:remainingfeed:view") && (
            <SidebarMenuItem
              to="/production-data/remaining-feed"
              title="Pozostała pasza"
              icon={<FaJarWheat />}
            />
          )}
          {hasPermission("productiondata:transferfeed:view") && (
            <SidebarMenuItem
              to="/production-data/transfer-feed"
              title="Pasza przeniesiona"
              icon={<FaBuildingWheat />}
            />
          )}
          {hasPermission("productiondata:weeklymeasurements:view") && (
            <SidebarMenuItem
              to="production-data/weekly-measurements"
              title="Pomiary cotygodniowe"
              icon={<MdCalendarViewWeek />}
              isOpen={openItems.includes("Pomiary cotygodniowe")}
              onClick={() => handleItemClick("Pomiary cotygodniowe")}
            >
              {hasPermission("productiondata:weighings:view") && (
                <SidebarMenuItem
                  to="/production-data/weighings"
                  title="Masy ciała"
                  icon={<FaWeight />}
                />
              )}
              {hasPermission("productiondata:flockloss:view") && (
                <SidebarMenuItem
                  to="/production-data/flock-loss"
                  title="Pomiary upadków i wybrakowań"
                  icon={<FaChartLine />}
                />
              )}
            </SidebarMenuItem>
          )}
          {hasPermission("productiondata:irzplus:view") && (
            <SidebarMenuItem
              to="/production-data/irzplus"
              title="IRZplus"
              icon={<FaBuilding />}
            />
          )}
        </SidebarMenuItem>
      )}
      {hasPermission("gas:view") && (
        <SidebarMenuItem
          to="/gas"
          title="Gaz"
          icon={<MdPropane />}
          isOpen={openItems.includes("Gaz")}
          onClick={() => handleItemClick("Gaz", true)}
        >
          {hasPermission("gas:deliveries:view") && (
            <SidebarMenuItem
              to="/gas/deliveries"
              title="Dostawy gazu"
              icon={<FaTruck />}
            />
          )}
          {hasPermission("gas:consumptions:view") && (
            <SidebarMenuItem
              to="/gas/consumptions"
              title="Zużycie gazu"
              icon={<FaBurn />}
            />
          )}
        </SidebarMenuItem>
      )}
      {hasPermission("hatcherynotes:view") && (
        <SidebarMenuItem
          to="/hatcheries-notes"
          title="Wylęgarnie - notatki"
          icon={<MdNotes />}
        />
      )}
      {hasPermission("employees:view") && (
        <SidebarMenuItem
          to="/employees"
          title="Pracownicy"
          icon={<MdPeopleAlt />}
          isOpen={openItems.includes("Pracownicy")}
          onClick={() => handleItemClick("Pracownicy", true)}
          notificationCount={notifications?.employees.count}
          notificationPriority={notifications?.employees.priority}
        >
          {hasPermission("employees:list:view") && (
            <SidebarMenuItem
              to="/employees"
              title="Kadry"
              icon={<FaAddressCard />}
              notificationCount={notifications?.employees.count}
              notificationPriority={notifications?.employees.priority}
            />
          )}
          {hasPermission("employees:payslips:view") && (
            <SidebarMenuItem
              to="/employees/payslips"
              title="Rozliczenie wypłat"
              icon={<FaFileInvoiceDollar />}
            />
          )}
        </SidebarMenuItem>
      )}

      <Typography sx={headingStyles}>Raporty</Typography>
      {hasPermission("summary:view") && (
        <SidebarMenuItem
          to="/summary"
          title="Podsumowanie"
          icon={<MdDashboard />}
          isOpen={openItems.includes("Podsumowanie")}
          onClick={() => handleItemClick("Podsumowanie", true)}
        >
          {hasPermission("summary:productionanalysis:view") && (
            <SidebarMenuItem
              to="/summary/production-analysis"
              title="Analiza produkcyjna"
              icon={<MdBarChart />}
            />
          )}
          {hasPermission("summary:financialanalysis:view") && (
            <SidebarMenuItem
              to="/summary/financial-analysis"
              title="Analiza finansowa"
              icon={<FaCalculator />}
            />
          )}
        </SidebarMenuItem>
      )}

      <Typography sx={headingStyles}>System</Typography>
      {hasPermission("data:view") && (
        <SidebarMenuItem
          to="/data"
          title="Dane"
          icon={<FaDatabase />}
          isOpen={openItems.includes("Dane")}
          onClick={() => handleItemClick("Dane", true)}
        >
          {hasPermission("data:taxbusinessentities:manage") && (
            <SidebarMenuItem
              to="/data/tax-business-entities"
              title="Podmioty"
              icon={<FaBuilding />}
            />
          )}
          {hasPermission("data:farms:manage") && (
            <SidebarMenuItem
              to="/data/farms"
              title="Fermy"
              icon={<PiFarmFill />}
            />
          )}
          {hasPermission("data:houses:manage") && (
            <SidebarMenuItem
              to="/data/houses"
              title="Kurniki"
              icon={<FaHouse />}
            />
          )}
          {hasPermission("data:hatcheries:manage") && (
            <SidebarMenuItem
              to="/data/hatcheries"
              title="Wylęgarnie"
              icon={<FaWarehouse />}
            />
          )}
          {hasPermission("data:slaughterhouses:manage") && (
            <SidebarMenuItem
              to="/data/slaughterhouses"
              title="Ubojnie"
              icon={<MdFactory />}
            />
          )}
          {hasPermission("data:utilizationplants:manage") && (
            <SidebarMenuItem
              to="/data/utilization-plants"
              title="Zakłady utylizacyjne"
              icon={<FaIndustry />}
            />
          )}
        </SidebarMenuItem>
      )}
      {hasPermission("settings:view") && (
        <SidebarMenuItem
          to="/settings"
          isOpen={openItems.includes("Ustawienia")}
          onClick={() => handleItemClick("Ustawienia", true)}
          title="Ustawienia"
          icon={<MdSettings />}
        >
          {hasPermission("settings:users:view") && (
            <SidebarMenuItem
              to="/settings/users"
              title="Użytkownicy"
              icon={<FaUserCog />}
            />
          )}
          {hasPermission("settings:cycles:manage") && (
            <SidebarMenuItem
              to="/settings/cycles"
              title="Cykle"
              icon={<MdTimelapse />}
            />
          )}
          <SidebarMenuItem
            to="/settings/invoice-assignment-rules"
            title="Reguły faktur (pracownicy)"
            icon={<FaGear />}
          />
          <SidebarMenuItem
            to="/settings/invoice-module-assignment-rules"
            title="Reguły faktur (moduły)"
            icon={<FaGear />}
          />
          <SidebarMenuItem
            to="/settings/invoice-farm-assignment-rules"
            title="Reguły faktur (lokalizacje)"
            icon={<FaGear />}
          />
        </SidebarMenuItem>
      )}
    </List>
  );

  return (
    <Drawer
      anchor="left"
      sx={{ zIndex: (theme) => theme.zIndex.appBar + 100 }}
      variant={lgUp ? "permanent" : "temporary"}
      open={open}
      onClose={onClose}
      PaperProps={{
        sx: {
          color: "primary.light",
          width: 280,
          backgroundColor: "primary.dark",
        },
      }}
    >
      <Box sx={{ display: "flex", flexDirection: "column", height: "100%" }}>
        <div>
          <Box sx={{ p: 3, display: "flex", justifyContent: "center" }}>
            <img
              src={LogoWhite}
              alt="Logo"
              style={{ width: "100px", height: "auto" }}
            />
          </Box>
          <div className="flex items-center m-3 p-3 rounded-lg bg-gray-100 select-none">
            <Avatar src={auth.userData?.avatarPath} alt="User Avatar" />
            <div className="ml-3 flex flex-col flex-grow">
              <Typography variant="h6">{auth.userData?.name}</Typography>
              <Typography variant="body2">{auth.userData?.login}</Typography>
            </div>
          </div>
          {contentList}
        </div>
      </Box>
    </Drawer>
  );
};

export default DashboardSidebar;
