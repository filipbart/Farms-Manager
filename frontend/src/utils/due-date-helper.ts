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
  isCorrection?: boolean,
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

/**
 * Calculates the CSS class name for accounting invoices based on due date
 * Uses different thresholds: 14+ days = no color, 13-8 = yellow, 7-4 = orange, 3-0 and overdue = red
 * @param dueDate - The due date string
 * @param paymentDate - The payment date string (null if not paid)
 * @returns CSS class name for row styling
 */
export const getAccountingDueDateClassName = (
  dueDate?: string | null,
  paymentDate?: string | null,
): string => {
  // Don't apply colors if already paid or no due date
  if (paymentDate || !dueDate) {
    return "";
  }

  const today = dayjs().startOf("day");
  const due = dayjs(dueDate).startOf("day");
  const daysUntilDue = due.diff(today, "day");

  // 14 lub więcej dni - bez koloru
  if (daysUntilDue >= 14) {
    return "";
  }

  // Po terminie lub 1-3 dni do terminu - czerwony (High)
  if (daysUntilDue <= 3) {
    return "payment-overdue";
  }

  // 4-7 dni do terminu - pomarańczowy (Medium)
  if (daysUntilDue <= 7) {
    return "payment-due-soon";
  }

  // 8-13 dni do terminu - żółty (Low)
  return "payment-due-warning";
};
