import { CssBaseline, ThemeProvider } from "@mui/material";
import "./App.css";
import DefaultRouter from "./router/default-router";
import theme from "./theme/theme";

import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";

import "dayjs/locale/pl";
import { RouterProvider } from "./router/use-router";
import {
  GlobalContext,
  GlobalContextProvider,
} from "./context/global/global-context";
import Loading from "./components/loading/loading";
import { AuthContextProvider } from "./auth/auth-context-provider";
import { NotificationProvider } from "./context/notification-context-provider";
import { PermissionsProvider } from "./context/permission-context-provider";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minut
      retry: 1, // Spróbuj ponownie 1 raz w razie błędu
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <RouterProvider>
        <GlobalContextProvider>
          <AuthContextProvider>
            <PermissionsProvider>
              <NotificationProvider>
                <QueryClientProvider client={queryClient}>
                  <LocalizationProvider
                    dateAdapter={AdapterDayjs}
                    adapterLocale="pl"
                  >
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
                </QueryClientProvider>
              </NotificationProvider>
            </PermissionsProvider>
          </AuthContextProvider>
        </GlobalContextProvider>
      </RouterProvider>
    </ThemeProvider>
  );
}

export default App;
