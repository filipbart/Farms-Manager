import { createContext, useContext } from "react";

interface PermissionsContextType {
  hasPermission: (permissionKey: string) => boolean;
  accessibleFarmIds: string[];
  isAdmin: boolean;
}

export const PermissionsContext = createContext<PermissionsContextType | null>(
  null
);

export const usePermissions = () => {
  const context = useContext(PermissionsContext);
  if (!context) {
    throw new Error(
      "usePermissions musi być używany wewnątrz PermissionsProvider"
    );
  }
  return context;
};
