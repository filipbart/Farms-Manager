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
import { IoCard, IoHome } from "react-icons/io5";
import {
  FaAddressCard,
  FaBurn,
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

  const [openItem, setOpenItem] = useState<string | null>(null);

  const handleItemClick = (title: string) => {
    setOpenItem((prev) => (prev === title ? null : title));
  };

  useEffect(() => {
    const path = location.pathname;
    if (path.startsWith("/sales")) setOpenItem("Sprzedaże");
    else if (path.startsWith("/feeds")) setOpenItem("Pasze");
    else if (path.startsWith("/expenses")) setOpenItem("Koszty");
    else if (path.startsWith("/production-data"))
      setOpenItem("Dane produkcyjne");
    else if (path.startsWith("/gas")) setOpenItem("Gaz");
    else if (path.startsWith("/employees")) setOpenItem("Pracownicy");
    else if (path.startsWith("/data")) setOpenItem("Dane");
    else if (path.startsWith("/settings")) setOpenItem("Ustawienia");
    else setOpenItem(null);
  }, [location.pathname]);

  const contentList = (
    <List sx={{ width: "100%", backgroundColor: "primary.dark" }}>
      <SidebarMenuItem to="/" title="Strona główna" icon={<IoHome />} />
      <SidebarMenuItem to="/insertions" title="Wstawienia" icon={<FaClone />} />

      <SidebarMenuItem
        to="/sales"
        title="Sprzedaże"
        icon={<IoCard />}
        isOpen={openItem === "Sprzedaże"}
        onClick={() => handleItemClick("Sprzedaże")}
      >
        <SidebarMenuItem to="/sales" title="Lista" icon={<FaList />} />
        <SidebarMenuItem
          to="/sales/invoices"
          title="Faktury sprzedażowe"
          icon={<FaFileInvoiceDollar />}
        />
        <SidebarMenuItem
          to="/sales/settings"
          title="Ustawienia pól"
          icon={<FaGear />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/feeds"
        title="Pasze"
        icon={<FaJarWheat />}
        isOpen={openItem === "Pasze"}
        onClick={() => handleItemClick("Pasze")}
      >
        <SidebarMenuItem
          to="/feeds/deliveries"
          title="Dostawy pasz"
          icon={<FaTruck />}
        />
        <SidebarMenuItem
          to="/feeds/prices"
          title="Ceny pasz"
          icon={<FaArrowTrendUp />}
        />
        <SidebarMenuItem
          to="/feeds/payments"
          title="Przelewy"
          icon={<FaFileInvoiceDollar />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/expenses"
        title="Koszty"
        icon={<FaMoneyBill />}
        isOpen={openItem === "Koszty"}
        onClick={() => handleItemClick("Koszty")}
      >
        <SidebarMenuItem
          to="/expenses/production"
          title="Koszty produkcyjne"
          icon={<MdFactory />}
        />
        <SidebarMenuItem
          to="/expenses/advances"
          title="Ewidencja zaliczek"
          icon={<FaHandHoldingUsd />}
        />
        <SidebarMenuItem
          to="/expenses/contractors"
          title="Kontrahenci"
          icon={<FaHandshake />}
        />
        <SidebarMenuItem
          to="/expenses/types"
          title="Typy wydatków"
          icon={<MdPayments />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/production-data"
        title="Dane produkcyjne"
        icon={<FaIndustry />}
        isOpen={openItem === "Dane produkcyjne"}
        onClick={() => handleItemClick("Dane produkcyjne")}
      >
        <SidebarMenuItem
          to="/production-data/failures"
          title="Upadki i wybrakowania"
          icon={<MdTrendingDown />}
        />
        <SidebarMenuItem
          to="/production-data/remaining-feed"
          title="Pozostała pasza"
          icon={<FaJarWheat />}
        />
        <SidebarMenuItem
          to="/production-data/transfer-feed"
          title="Pasza przeniesiona"
          icon={<FaBuildingWheat />}
        />
        <SidebarMenuItem
          to="/production-data/weighings"
          title="Masy ciała"
          icon={<FaWeight />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/gas"
        title="Gaz"
        icon={<MdPropane />}
        isOpen={openItem === "Gaz"}
        onClick={() => handleItemClick("Gaz")}
      >
        <SidebarMenuItem
          to="/gas/deliveries"
          title="Dostawy gazu"
          icon={<FaTruck />}
        />
        <SidebarMenuItem
          to="/gas/consumptions"
          title="Zużycie gazu"
          icon={<FaBurn />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/hatcheries-notes"
        title="Wylęgarnie - notatki"
        icon={<MdNotes />}
      />

      <SidebarMenuItem
        to="/employees"
        title="Pracownicy"
        icon={<MdPeopleAlt />}
        isOpen={openItem === "Pracownicy"}
        onClick={() => handleItemClick("Pracownicy")}
      >
        <SidebarMenuItem
          to="/employees"
          title="Kadry"
          icon={<FaAddressCard />}
        />
        <SidebarMenuItem
          to="/employees/payslips"
          title="Rozliczenie wypłat"
          icon={<FaFileInvoiceDollar />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/data"
        title="Dane"
        icon={<FaDatabase />}
        isOpen={openItem === "Dane"}
        onClick={() => handleItemClick("Dane")}
      >
        <SidebarMenuItem to="/data/farms" title="Fermy" icon={<PiFarmFill />} />
        <SidebarMenuItem to="/data/houses" title="Kurniki" icon={<FaHouse />} />
        <SidebarMenuItem
          to="/data/hatcheries"
          title="Wylęgarnie"
          icon={<FaWarehouse />}
        />
        <SidebarMenuItem
          to="/data/slaughterhouses"
          title="Ubojnie"
          icon={<MdFactory />}
        />
      </SidebarMenuItem>

      <SidebarMenuItem
        to="/settings"
        isOpen={openItem === "Ustawienia"}
        onClick={() => handleItemClick("Ustawienia")}
        title="Ustawienia"
        icon={<MdSettings />}
      >
        <SidebarMenuItem
          to="/settings/users"
          title="Użytkownicy"
          icon={<FaUserCog />}
        />
        <SidebarMenuItem
          to="/settings/cycles"
          title="Cykle"
          icon={<MdTimelapse />}
        />
      </SidebarMenuItem>
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
            <Avatar />
            <div className="ml-3 flex flex-col">
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
