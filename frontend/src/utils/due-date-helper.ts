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

  const today = dayjs().startOf("day");
  const due = dayjs(dueDate).startOf("day");
  const daysUntilDue = due.diff(today, "day");

  // Dzień terminu lub po terminie (0 dni lub mniej) - czerwony
  if (daysUntilDue <= 0) {
    return "payment-overdue";
  }
  
  // 3-1 dni do końca - żółty
  if (daysUntilDue >= 1 && daysUntilDue <= 3) {
    return "payment-due-soon";
  }
  
  // 7-4 dni do końca - niebieski
  if (daysUntilDue >= 4 && daysUntilDue <= 7) {
    return "payment-due-warning";
  }

  // Więcej niż 7 dni - bez koloru
  return "";
};
