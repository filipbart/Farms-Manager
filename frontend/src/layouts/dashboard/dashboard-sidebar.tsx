import React from "react";
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
import { IoCard, IoHome, IoSettings } from "react-icons/io5";
import {
  FaClone,
  FaDatabase,
  FaFileInvoiceDollar,
  FaHandshake,
  FaIndustry,
  FaList,
  FaMoneyBill,
  FaTruck,
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
  MdPayments,
  MdPeopleAlt,
  MdTrendingDown,
} from "react-icons/md";
import { PiFarmFill } from "react-icons/pi";
import { useAuth } from "../../auth/useAuth";

interface DashboardSidebarProps {
  open: boolean;
  onClose: () => void;
}

const DashboardSidebar: React.FC<DashboardSidebarProps> = ({
  open,
  onClose,
}) => {
  const contentList = (
    <>
      <List sx={{ width: "100%", backgroundColor: "primary.dark" }}>
        <SidebarMenuItem to="/" title="Strona główna" icon={<IoHome />} />

        <SidebarMenuItem
          to="/insertions"
          title="Wstawienia"
          icon={<FaClone />}
        />
        <SidebarMenuItem to="/sales" title="Sprzedaże" icon={<IoCard />}>
          <SidebarMenuItem to="/sales" title="Lista" icon={<FaList />} />
          <SidebarMenuItem
            to="/sales/settings"
            title="Ustawienia pól"
            icon={<FaGear />}
          />
        </SidebarMenuItem>

        <SidebarMenuItem to="/feeds" title="Pasze" icon={<FaJarWheat />}>
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

        <SidebarMenuItem to="/expenses" title="Koszty" icon={<FaMoneyBill />}>
          <SidebarMenuItem
            to="/expenses/production"
            title="Koszty produkcyjne"
            icon={<MdFactory />}
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
            to="/production-data/moved-feeds"
            title="Pasza przeniesiona"
            icon={<FaBuildingWheat />}
          />
          <SidebarMenuItem
            to="/production-data/body-weights"
            title="Masy ciała"
            icon={<FaWeight />}
          />
        </SidebarMenuItem>
        <SidebarMenuItem to="/data" title="Dane" icon={<FaDatabase />}>
          <SidebarMenuItem
            to="/data/farms"
            title="Fermy"
            icon={<PiFarmFill />}
          />
          <SidebarMenuItem
            to="/data/houses"
            title="Kurniki"
            icon={<FaHouse />}
          />
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
          to="/users"
          title="Użytkownicy"
          icon={<MdPeopleAlt />}
        />
        <SidebarMenuItem
          to="/settings"
          title="Ustawienia"
          icon={<IoSettings />}
        />
      </List>
    </>
  );

  const lgUp = useMediaQuery((theme: any) => theme.breakpoints.up("lg"), {
    defaultMatches: true,
    noSsr: true,
  });

  const auth = useAuth();

  return (
    <Drawer
      anchor="left"
      sx={{ zIndex: (theme) => theme.zIndex.appBar + 100 }}
      variant={lgUp ? "permanent" : "temporary"}
      open={open}
      onClose={onClose}
      slotProps={{
        paper: {
          sx: {
            color: "primary.light",
            width: 280,
          },
        },
      }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          height: "100%",
          backgroundColor: "primary.dark",
        }}
      >
        <div>
          <Box sx={{ p: 3 }}>
            <img
              className="justify-center mx-auto"
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
