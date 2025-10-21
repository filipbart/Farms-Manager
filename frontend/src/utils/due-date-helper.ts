import dayjs from "dayjs";

/**
 * Calculates the CSS class name based on due date and payment status
 * @param dueDate - The due date string
 * @param paymentDate - The payment date string (null if not paid)
 * @param isCorrection - Whether this is a correction (corrections don't get colored)
 * @returns CSS class name for row styling
 */
export const getDueDateClassName = (
  dueDate?: string | null,
  paymentDate?: string | null,
  isCorrection?: boolean
): string => {
  // Don't apply colors to corrections or if already paid
  if (isCorrection || paymentDate || !dueDate) {
    return "";
  }

  const today = dayjs();
  const due = dayjs(dueDate);
  const daysUntilDue = due.diff(today, "day");

  // Po terminie (overdue) - czerwony
  if (daysUntilDue < 0) {
    return "payment-overdue";
  }
  
  // 3 dni do końca - żółty
  if (daysUntilDue <= 3) {
    return "payment-due-soon";
  }
  
  // 7-4 dni do końca - niebieski
  if (daysUntilDue <= 7) {
    return "payment-due-warning";
  }

  // Więcej niż 7 dni - bez koloru
  return "";
};
