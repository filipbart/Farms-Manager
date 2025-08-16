import { useAuth } from "../auth/useAuth";
import { PermissionsContext } from "./permission-context";

export const PermissionsProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const auth = useAuth();

  const isAdmin = auth.userData?.isAdmin ?? false;
  const permissions = auth.userData?.permissions ?? [];
  const accessibleFarmIds = auth.userData?.accessibleFarmIds ?? [];

  const hasPermission = (permissionKey: string): boolean => {
    if (isAdmin) {
      return true;
    }
    return permissions.includes(permissionKey);
  };

  const value = { hasPermission, accessibleFarmIds, isAdmin };

  return (
    <PermissionsContext.Provider value={value}>
      {children}
    </PermissionsContext.Provider>
  );
};
