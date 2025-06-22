import React, {
  createContext,
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

import AxiosWrapper from "../utils/axios/wrapper";
import { useRouter } from "../router/use-router";
import { RouteName } from "../router/route-names";
import { AuthService } from "../services/auth-service";
import { UserService } from "../services/user-service";
import { GlobalContext } from "../context/global/global-context";
import type UserModel from "../common/interfaces/user-model";

interface AuthContextModel {
  userData?: UserModel;
  logout: () => void;
  setAuthToken: (token: string) => void;
  removeAuthToken: () => void;
  fetchUserData: () => Promise<void>;
  isAuthenticated: () => boolean;
  token?: string | null;
}

export const AuthContext = createContext<AuthContextModel>(
  {} as AuthContextModel
);

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

  const setToken = useCallback((token: string) => {
    setUserToken(token);
    AxiosWrapper.setAuthToken(token);
    startSessionChecker();
  }, []);

  const removeAuthToken = useCallback(() => {
    clearUserToken();
    stopSessionChecker();
  }, []);

  const logout = useCallback(() => {
    setUser(undefined);
    removeAuthToken();
    nav(getRoute(RouteName.Login));
  }, []);

  const fetchUserData = useCallback(async () => {
    const response = await UserService.getUserAsync();
    if (response.success && response.responseData) {
      setUser(response.responseData);
    } else {
      removeAuthToken();
    }
  }, []);

  const sessionCheck = useCallback(async () => {
    if (!authToken) return;

    try {
      const { exp }: any = jwtDecode(authToken);
      const now = Date.now() / 1000;
      const remaining = exp - now;

      if (remaining <= 0) {
        logout();
      } else if (remaining < 60) {
        stopSessionChecker();

        const res = await AuthService.refreshTokenAsync();
        if (res.success && res.responseData?.accessToken) {
          setToken(res.responseData.accessToken);
          toast.success("Sesja odświeżona");
        } else {
          logout();
          toast.error("Nie udało się odświeżyć sesji.");
        }
      }
    } catch {
      logout();
    }
  }, [authToken]);

  const startSessionChecker = () => {
    if (authInterval) clearInterval(authInterval);
    const interval = setInterval(() => savedCallback.current(), 1000);
    setAuthInterval(interval);
  };

  const stopSessionChecker = () => {
    if (authInterval) clearInterval(authInterval);
    setAuthInterval(null);
  };

  const initialLoad = useCallback(async () => {
    if (authToken) {
      setToken(authToken);
      await fetchUserData();
    }
    setPageLaoded();
  }, [authToken]);

  useEffect(() => {
    savedCallback.current = sessionCheck;
  }, [sessionCheck]);

  useEffect(() => {
    initialLoad();
  }, []);

  useEffect(() => {
    if (!authToken && location.pathname !== "/login") {
      console.log("wywołało");
      toast.warning("Musisz być zalogowany");
      nav(getRoute(RouteName.Login));
    }
  }, [authToken]);

  useEffect(() => {
    const interceptor = axios.interceptors.response.use(
      (res) => res,
      (err) => {
        const status = err?.response?.status;
        if (status === 401) logout();
        else if (status === 403) nav(getRoute(RouteName.Forbidden));
        else if (status === 404) toast.error("Nie znaleziono zasobu");
        else if (status >= 500) nav(getRoute(RouteName.InternalServerError));
        return Promise.reject(err);
      }
    );

    return () => axios.interceptors.response.eject(interceptor);
  }, []);

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
