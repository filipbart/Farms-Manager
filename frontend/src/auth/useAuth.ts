import { useAuthContext } from "./auth-context";

export const useAuth = () => {
  const { user, token, login, logout, isLoggedIn, isLoading } = useAuthContext();
  return { user, token, login, logout, isLoggedIn, isLoading };
};
