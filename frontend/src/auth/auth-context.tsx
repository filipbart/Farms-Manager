import { createContext } from "react";
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
