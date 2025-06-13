import { createTheme } from "@mui/material/styles";
import { plPL } from "@mui/x-data-grid/locales";
import { plPL as pickersPlPL } from "@mui/x-date-pickers/locales";
import { plPL as corePlPL } from "@mui/material/locale";
import type {} from "@mui/x-data-grid/themeAugmentation";

const theme = createTheme(
  {
    palette: {
      mode: "light",
      primary: {
        main: "#0D1B2A",
        dark: "#1B263B",
        light: "#415A77",
        contrastText: "#E0E1DD",
      },
      secondary: {
        main: "#778DA9",
        contrastText: "#0D1B2A",
      },
      background: {
        default: "#ffffff",
        paper: "#ffffff",
      },
      text: {
        primary: "#0D1B2A",
        secondary: "#415A77",
      },
      DataGrid: {
        bg: "#f1f5f9",
      },
    },
    typography: {
      fontFamily: "DM Sans",
      h1: { fontWeight: 700 },
      h2: { fontWeight: 600 },
      h3: { fontWeight: 500 },
      body1: { fontSize: "1rem" },
    },
    components: {
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 12,
            textTransform: "none",
            padding: "8px 16px",
          },
        },
      },
      MuiDataGrid: {
        styleOverrides: {
          root: {
            borderColor: "#374151",
            ".MuiDataGrid-filler": {
              border: "none !important",
            },
          },
          toolbar: {
            border: "none",
          },
          columnHeader: {
            border: "none !important",
          },
          columnHeaders: {
            border: "none",
            borderBottom: "1px solid",
            borderBottomColor: "#374151",
            borderTop: "1px solid",
            borderTopColor: "#374151",
          },
          cell: {
            backgroundColor: "#fff",
            color: "#374151",
            border: "none",
            borderBottom: "1px solid",
            borderBottomColor: "#374151",
            ":focus": {
              outline: "none",
            },
          },

          row: {
            "&:last-of-type .MuiDataGrid-cell": {
              borderBottom: "none",
            },
          },
          columnSeparator: {
            color: "#374151",
            opacity: 1,
          },
          footerContainer: {
            borderTop: "1px solid #374151 !important",
          },
          scrollbar: {
            backgroundColor: "#E0E1DD",
          },
        },
      },
    },
  },
  plPL,
  pickersPlPL,
  corePlPL
);

export default theme;
