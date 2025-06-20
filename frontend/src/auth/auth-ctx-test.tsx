import React, {
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react";
import useCookie from "react-use-cookie";
import AxiosWrapper from "../utils/axios/wrapper";
import { useLocation, useNavigate } from "react-router-dom";
import { useRouter } from "../router/use-router";
import type UserModel from "../common/interfaces/user-model";
import { jwtDecode } from "jwt-decode";
import { AuthService } from "../services/auth-service";
import { toast } from "react-toastify";
import { RouteName } from "../router/route-names";
import axios from "axios";
import { GlobalContext } from "../context/global/global-context";
import { UserService } from "../services/user-service";

interface AuthContextProps {
  children: any;
}

interface AuthContextModel {
  userData?: UserModel;
  logout: () => void;
  setAuthToken: (token: string) => void;
  removeAuthToken: () => void;
  fetchUserData: () => Promise<void>;
  isAuthenticated: () => boolean;
  token?: string | null;
}

export const AuthContext = createContext({} as AuthContextModel);

export const AuthProvider: React.FC<AuthContextProps> = ({ children }) => {
  const nav = useNavigate();
  const { getRoute } = useRouter();

  const globalCtx = useContext(GlobalContext);
  const location = useLocation();

  const refreshUserAsync = async () => {
    const response = await UserService.getUserAsync();
    if (response.success && response.responseData) {
      setUser(response.responseData);
    } else {
      removeAuthToken();
    }
  };

  const removeAuthToken = () => {
    clearUserToken();
    clearAuthTask();
  };

  const logout = () => {
    setUser(undefined);
    removeAuthToken();
    nav(getRoute(RouteName.Login));
  };

  const setAuthTask = () => {
    if (authInterval) {
      clearInterval(authInterval);
    }

    const interval = setInterval(() => savedCallback?.current(), 1000);
    setAuthInterval(interval);
  };

  const setToken = (token: string) => {
    setUserToken(token);
    AxiosWrapper.setAuthToken(token);

    setAuthTask();
  };

  const clearAuthTask = () => {
    if (authInterval) {
      clearInterval(authInterval);
    }

    setAuthInterval(undefined);
  };

  const authLoading = async () => {
    if (authToken) {
      setToken(authToken);
      await refreshUserAsync();
    }

    globalCtx.setPageLoaded();
  };

  useEffect(() => {
    authLoading();
  }, []);

  const authIntervalHandler = async () => {
    if (!authToken) return;

    const decodedToken: any = jwtDecode(authToken as string);
    const tokenExpDate = decodedToken.exp;
    const currentTime = Date.now() / 1000;
    const difference = Math.floor((tokenExpDate - currentTime) / 1000);

    if (difference <= 0) {
      logout();
      return;
    }

    if (difference < 60) {
      if (authInterval) {
        clearInterval(authInterval);
      }
      const response = await AuthService.refreshTokenAsync();
      if (response.success && response.responseData) {
        setToken(response.responseData.accessToken);
        toast.success("Sesja wydłużona pomyślnie");
      } else {
        logout();
        toast.error(
          "Nie udało się odświeżyć sesji. Proszę zalogować się ponownie."
        );
      }
    }
  };

  const savedCallback = useRef(authIntervalHandler);
  const [authToken, setUserToken, clearUserToken] = useCookie(
    "access_token",
    ""
  );
  const [user, setUser] = useState<UserModel>();
  const [authInterval, setAuthInterval] = useState<NodeJS.Timeout>();

  useEffect(() => {
    axios.interceptors.response.use(
      (response) => {
        return response;
      },
      (error) => {
        if (error.response.status == 401) {
          logout();
        } else if (error.response.status == 403) {
          nav(getRoute(RouteName.Forbidden));
        } else {
          if (error.response.status.toString().startsWith("5")) {
            nav(getRoute(RouteName.InternalServerError));
          }
        }

        return Promise.reject(error);
      }
    );
  }, []);

  useEffect(() => {
    if (!authToken && location.pathname !== "/login") {
      nav(getRoute(RouteName.Login));
      toast.warning("Musisz być zalogowany, aby uzyskać dostęp do tej strony.");
    }
  }, [authToken]);

  useEffect(() => {
    savedCallback.current = authIntervalHandler;
  }, [authIntervalHandler]);

  const isAuthenticated = () => !!user && !!authToken;

  return (
    <AuthContext.Provider
      value={{
        userData: user,
        logout,
        setAuthToken: setToken,
        removeAuthToken,
        fetchUserData: refreshUserAsync,
        isAuthenticated,
        token: authToken,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
