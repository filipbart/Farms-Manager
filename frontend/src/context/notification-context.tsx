import { createContext, useContext } from "react";
import type { NotificationData } from "../models/common/notifications";

interface NotificationContextValue {
  notifications: NotificationData | undefined;
  refetch: () => void;
  isRefreshing: boolean;
}

export const NotificationContext = createContext<NotificationContextValue>({
  notifications: undefined,
  refetch: () => {},
  isRefreshing: false,
} as NotificationContextValue);

export const useNotifications = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error(
      "useNotifications must be used within a NotificationProvider"
    );
  }
  return context;
};
