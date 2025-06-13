import { ThemeProvider } from "@emotion/react";
import { CssBaseline } from "@mui/material";
import "./App.css";
import DefaultRouter from "./default-router";
import theme from "./theme/theme";

import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";

import "dayjs/locale/pl";

function App() {
  return (
    <ThemeProvider theme={theme}>
      <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="pl">
        <CssBaseline />
        <DefaultRouter />
      </LocalizationProvider>
    </ThemeProvider>
  );
}

export default App;
