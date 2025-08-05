import { createContext, useContext } from "react";
import type { NotificationData } from "../models/common/notifications";

interface NotificationContextValue {
  notifications: NotificationData | null;
  isRefreshing: boolean;
  refetch: () => void;
}

export const NotificationContext =
  createContext<NotificationContextValue | null>(null);

export const useNotifications = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error(
      "useNotifications must be used within a NotificationProvider"
    );
  }
  return context;
};
