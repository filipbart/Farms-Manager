/**
 * Mapuje priorytet z backendu na klasę CSS dla podświetlenia wiersza
 */
export const getPriorityClassName = (
  priority?: "Low" | "Medium" | "High"
): string => {
  if (!priority) return "";

  switch (priority) {
    case "High":
      return "payment-overdue";
    case "Medium":
      return "payment-due-soon";
    case "Low":
      return "payment-due-warning";
    default:
      return "";
  }
};
