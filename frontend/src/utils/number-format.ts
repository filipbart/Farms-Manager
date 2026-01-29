/**
 * Formats a number with thousand separators (spaces) - Polish format
 * @param value - The number to format
 * @returns Formatted string with spaces as thousand separators
 */
export const formatNumberWithSpaces = (value: string | number): string => {
  if (value === "" || value === null || value === undefined) return "";

  const numStr = String(value);

  // Detect which separator is used (comma or dot)
  const hasComma = numStr.includes(",");
  const separator = hasComma ? "," : ".";

  // Check if value ends with separator (user is typing decimal part)
  const endsWithSeparator = numStr.endsWith(",") || numStr.endsWith(".");

  // Normalize to dot for processing, then split
  const normalized = numStr.replace(",", ".");
  const parts = normalized.split(".");
  const integerPart = parts[0].replace(/\s/g, "");
  const decimalPart = parts[1] || "";

  // Add spaces as thousand separators
  const formatted = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, " ");

  // Preserve trailing separator if user just typed it
  if (endsWithSeparator && !decimalPart) {
    return `${formatted}${numStr.slice(-1)}`;
  }

  // Use the original separator (comma or dot) in output
  return decimalPart ? `${formatted}${separator}${decimalPart}` : formatted;
};

/**
 * Parses a formatted number string back to plain number
 * @param value - The formatted string to parse
 * @returns Plain number string without spaces, with comma replaced by dot
 */
export const parseFormattedNumber = (value: string): string => {
  if (!value) return "";
  // Remove spaces and replace comma with dot (Polish decimal separator to standard)
  return value.replace(/\s/g, "").replace(",", ".");
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
