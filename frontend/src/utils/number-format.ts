/**
 * Formats a number with thousand separators (spaces) - Polish format
 * @param value - The number to format
 * @returns Formatted string with spaces as thousand separators
 */
export const formatNumberWithSpaces = (value: string | number): string => {
  if (value === "" || value === null || value === undefined) return "";

  const numStr = String(value);
  const parts = numStr.split(".");
  const integerPart = parts[0].replace(/\s/g, "");
  const decimalPart = parts[1] || "";

  // Add spaces as thousand separators
  const formatted = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, " ");

  return decimalPart ? `${formatted}.${decimalPart}` : formatted;
};

/**
 * Parses a formatted number string back to plain number
 * @param value - The formatted string to parse
 * @returns Plain number string without spaces
 */
export const parseFormattedNumber = (value: string): string => {
  if (!value) return "";
  return value.replace(/\s/g, "");
};

/**
 * Formats input value as user types, maintaining cursor position
 * @param value - Current input value
 * @param previousValue - Previous input value
 * @param cursorPosition - Current cursor position
 * @returns Object with formatted value and new cursor position
 */
export const formatInputNumber = (
  value: string,
  previousValue: string,
  cursorPosition: number,
): { formattedValue: string; newCursorPosition: number } => {
  if (!value) {
    return { formattedValue: "", newCursorPosition: 0 };
  }

  // Parse the current value
  const parsed = parseFormattedNumber(value);

  if (parsed === "") {
    return { formattedValue: "", newCursorPosition: 0 };
  }

  // Format the number
  const formatted = formatNumberWithSpaces(parsed);

  // Calculate new cursor position
  const diff = formatted.length - previousValue.length;
  const newCursorPosition = Math.max(0, cursorPosition + diff);

  return { formattedValue: formatted, newCursorPosition };
};
