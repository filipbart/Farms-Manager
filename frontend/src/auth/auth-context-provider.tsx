import React, {
  useContext,
  useEffect,
  useRef,
  useState,
  useCallback,
} from "react";
import useCookie from "react-use-cookie";
import { jwtDecode } from "jwt-decode";
import axios from "axios";
import { toast } from "react-toastify";
import { useNavigate, useLocation } from "react-router-dom";
import { AuthContext } from "./auth-context";
import AxiosWrapper from "../utils/axios/wrapper";
import { useRouter } from "../router/use-router";
import { RouteName } from "../router/route-names";
import { AuthService } from "../services/auth-service";
import { UserService } from "../services/user-service";
import { GlobalContext } from "../context/global/global-context";
import type UserModel from "../common/interfaces/user-model";

export const AuthContextProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [authToken, setUserToken, clearUserToken] = useCookie(
    "access_token",
    ""
  );
  const [user, setUser] = useState<UserModel>();
  const [authInterval, setAuthInterval] = useState<NodeJS.Timeout | null>(null);
  const savedCallback = useRef<() => void>(() => {});
  const nav = useNavigate();
  const location = useLocation();
  const { getRoute } = useRouter();
  const { setPageLoaded: setPageLaoded } = useContext(GlobalContext);

  const isInitialLoadDone = useRef(false);
  const isLoggingOut = useRef(false);

  const startSessionChecker = () => {
    if (authInterval) clearInterval(authInterval);
    const interval = setInterval(() => savedCallback.current(), 1000);
    setAuthInterval(interval);
  };

  const stopSessionChecker = () => {
    if (authInterval) clearInterval(authInterval);
    setAuthInterval(null);
  };

  const setToken = useCallback(
    (token: string) => {
      setUserToken(token);
      AxiosWrapper.setAuthToken(token);
      startSessionChecker();
    },
    [setUserToken]
  );

  const removeAuthToken = useCallback(() => {
    clearUserToken();
    AxiosWrapper.setAuthToken("");
    stopSessionChecker();
  }, [clearUserToken]);

  const logout = useCallback(async () => {
    if (isLoggingOut.current) return;
    isLoggingOut.current = true;

    setUser(undefined);
    removeAuthToken();
    nav(getRoute(RouteName.Login));

    try {
      await AuthService.logout();
    } catch (error) {
      console.error(
        "Nie udało się wylogować na serwerze (token mógł wygasnąć):",
        error
      );
    } finally {
      isLoggingOut.current = false;
    }
  }, [removeAuthToken, nav, getRoute]);

  const fetchUserData = useCallback(async () => {
    const response = await UserService.getUser();

    if (response.success && response.responseData) {
      setUser(response.responseData);
    } else {
      await logout();
    }
  }, [logout]);

  const sessionCheck = useCallback(async () => {
    if (!authToken) return;

    try {
      const { exp }: any = jwtDecode(authToken);
      const now = Date.now() / 1000;
      const remaining = exp - now;

      if (remaining <= 0) {
        await logout();
      } else if (remaining < 60) {
        stopSessionChecker();

        const res = await AuthService.refreshToken();
        if (res.success && res.responseData?.accessToken) {
          setToken(res.responseData.accessToken);
          toast.success("Sesja odświeżona");
        } else {
          await logout();
          toast.error("Nie udało się odświeżyć sesji.");
        }
      }
    } catch {
      await logout();
    }
  }, [authToken, logout, setToken]);

  const initialLoad = useCallback(async () => {
    if (isInitialLoadDone.current) return;
    isInitialLoadDone.current = true;

    if (authToken) {
      setToken(authToken);
      await fetchUserData();
    }

    setPageLaoded();
  }, [authToken, fetchUserData, setPageLaoded, setToken]);

  useEffect(() => {
    savedCallback.current = sessionCheck;
  }, [sessionCheck]);

  useEffect(() => {
    initialLoad();
  }, [initialLoad]);

  useEffect(() => {
    if (!authToken && location.pathname !== "/login") {
      toast.warning("Musisz być zalogowany");
      nav(getRoute(RouteName.Login));
    }
  }, [authToken, location.pathname, nav, getRoute]);

  useEffect(() => {
    const interceptor = axios.interceptors.response.use(
      (res) => res,
      (err) => {
        const status = err?.response?.status;
        if (status === 401) logout();
        else if (status === 403) nav(getRoute(RouteName.Forbidden));
        else if (status === 404) toast.error("Nie znaleziono zasobu");

        return Promise.reject(err);
      }
    );

    return () => axios.interceptors.response.eject(interceptor);
  }, [logout, nav, getRoute]);

  const isAuthenticated = useCallback(
    () => !!user && !!authToken,
    [user, authToken]
  );

  return (
    <AuthContext.Provider
      value={{
        userData: user,
        logout,
        setAuthToken: setToken,
        removeAuthToken,
        fetchUserData,
        isAuthenticated,
        token: authToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
