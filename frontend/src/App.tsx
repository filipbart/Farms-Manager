import { CssBaseline, ThemeProvider } from "@mui/material";
import "./App.css";
import DefaultRouter from "./router/default-router";
import theme from "./theme/theme";

import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";

import "dayjs/locale/pl";
import { RouterProvider } from "./router/use-router";
import { AuthContextProvider } from "./auth/auth-context";
import {
  GlobalContext,
  GlobalContextProvider,
} from "./context/global/global-context";
import Loading from "./components/loading/loading";

function App() {
  return (
    <ThemeProvider theme={theme}>
      <RouterProvider>
        <GlobalContextProvider>
          <AuthContextProvider>
            <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="pl">
              <GlobalContext.Consumer>
                {(ctx) => {
                  if (!ctx.state.pageLoaded) {
                    return <Loading />;
                  }

                  return <DefaultRouter />;
                }}
              </GlobalContext.Consumer>
              <CssBaseline />
            </LocalizationProvider>
          </AuthContextProvider>
        </GlobalContextProvider>
      </RouterProvider>
    </ThemeProvider>
  );
}

export default App;
