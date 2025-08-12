import { useState, useCallback, useEffect, useContext } from "react";
import { toast } from "react-toastify";
import type { NotificationData } from "../models/common/notifications";
import { UserService } from "../services/user-service";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import { NotificationContext } from "./notification-context";
import { AuthContext } from "../auth/auth-context";

export const NotificationProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const authCtx = useContext(AuthContext);
  const [notifications, setNotifications] = useState<
    NotificationData | undefined
  >();
  const [isRefreshing, setIsRefreshing] = useState(false);

  const fetchNotifications = useCallback(async () => {
    setIsRefreshing(true);
    try {
      await handleApiResponse(
        () => UserService.getNotifications(),
        (data) => {
          if (data && data.responseData) {
            setNotifications(data.responseData.data);
          }
        },
        undefined,
        "Błąd podczas pobierania alertów"
      );
    } catch {
      toast.error("Błąd podczas odświeżania powiadomień");
    } finally {
      setIsRefreshing(false);
    }
  }, []);

  useEffect(() => {
    if (authCtx.isAuthenticated()) {
      fetchNotifications();
    } else {
      setNotifications(undefined);
    }
  }, [authCtx.isAuthenticated, fetchNotifications]);

  const value = {
    notifications,
    isRefreshing,
    fetchNotifications,
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  );
};
