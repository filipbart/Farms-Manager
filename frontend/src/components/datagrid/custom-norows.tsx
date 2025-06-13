import { Typography } from "@mui/material";
import React from "react";
import { MdManageSearch } from "react-icons/md";

const NoRowsOverlay: React.FC = () => {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        textAlign: "center",
        margin: "20px",
      }}
    >
      <MdManageSearch size={40} />
      <Typography variant="h6">Brak danych</Typography>
    </div>
  );
};

export default NoRowsOverlay;
