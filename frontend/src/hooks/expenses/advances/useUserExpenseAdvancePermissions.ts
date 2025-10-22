import { useState, useEffect } from "react";
import { ExpenseAdvancePermissionsService } from "../../../services/expense-advance-permissions-service";
import type { 
  UserExpenseAdvancePermissions
} from "../../../models/expenses/advances/expense-advance-permissions";
import { ExpenseAdvancePermissionType } from "../../../models/expenses/advances/expense-advance-permissions";

interface UseUserExpenseAdvancePermissionsResult {
  permissions: UserExpenseAdvancePermissions | null;
  loading: boolean;
  error: string | null;
  hasEditPermission: (employeeId: string) => boolean;
}

export const useUserExpenseAdvancePermissions = (
  userId: string | undefined
): UseUserExpenseAdvancePermissionsResult => {
  const [permissions, setPermissions] = useState<UserExpenseAdvancePermissions | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!userId) {
      setPermissions(null);
      setLoading(false);
      setError(null);
      return;
    }

    const fetchPermissions = async () => {
      setLoading(true);
      setError(null);
      
      try {
        const response = await ExpenseAdvancePermissionsService.getUserExpenseAdvancePermissions(userId);
        if (response.success && response.responseData) {
          setPermissions(response.responseData);
        } else {
          setError("Failed to fetch permissions");
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : "Unknown error occurred");
      } finally {
        setLoading(false);
      }
    };

    fetchPermissions();
  }, [userId]);

  const hasEditPermission = (employeeId: string): boolean => {
    if (!permissions) return false;
    
    return permissions.permissions.some(
      permission => 
        permission.expenseAdvanceId === employeeId && 
        permission.permissionType === ExpenseAdvancePermissionType.Edit
    );
  };

  return {
    permissions,
    loading,
    error,
    hasEditPermission,
  };
};
