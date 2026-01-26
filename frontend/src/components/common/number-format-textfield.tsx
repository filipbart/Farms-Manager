import React, { useState, useEffect, useRef } from "react";
import { TextField } from "@mui/material";
import type { TextFieldProps } from "@mui/material";
import { formatNumber, parseFormattedNumber } from "../../utils/number-format";

interface NumberFormatTextFieldProps extends Omit<
  TextFieldProps,
  "value" | "onChange" | "type"
> {
  value: number | null | undefined;
  onChange: (value: number | null) => void;
  minimumFractionDigits?: number;
  maximumFractionDigits?: number;
  allowNegative?: boolean;
}

const NumberFormatTextField: React.FC<NumberFormatTextFieldProps> = ({
  value,
  onChange,
  minimumFractionDigits = 2,
  maximumFractionDigits = 2,
  allowNegative = false,
  onBlur,
  ...props
}) => {
  const [displayValue, setDisplayValue] = useState("");
  const [isFocused, setIsFocused] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const lastValidValue = useRef<string>("");

  // Update display value when prop value changes
  useEffect(() => {
    if (!isFocused) {
      if (value !== null && value !== undefined && !isNaN(value)) {
        const formatted = formatNumber(
          value,
          minimumFractionDigits,
          maximumFractionDigits,
        );
        setDisplayValue(formatted);
        lastValidValue.current = formatted;
      } else {
        setDisplayValue("");
        lastValidValue.current = "";
      }
    }
  }, [value, minimumFractionDigits, maximumFractionDigits, isFocused]);

  const handleFocus = (event: React.FocusEvent<HTMLInputElement>) => {
    setIsFocused(true);

    // When focused, show the raw number without formatting
    if (value !== null && value !== undefined && !isNaN(value)) {
      // Show with up to 2 decimal places, but without thousands separator
      setDisplayValue(value.toFixed(2).replace(".", ","));
    }

    if (props.onFocus) {
      props.onFocus(event);
    }
  };

  const handleBlur = (event: React.FocusEvent<HTMLInputElement>) => {
    setIsFocused(false);

    // Parse and format the value on blur
    const parsed = parseFormattedNumber(displayValue);

    if (parsed === null) {
      onChange(null);
      setDisplayValue("");
    } else {
      // Apply negative restriction if needed
      const finalValue = allowNegative ? parsed : Math.abs(parsed);
      onChange(finalValue);

      const formatted = formatNumber(
        finalValue,
        minimumFractionDigits,
        maximumFractionDigits,
      );
      setDisplayValue(formatted);
    }

    if (onBlur) {
      onBlur(event);
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = event.target.value;

    // Allow empty input
    if (newValue === "") {
      setDisplayValue("");
      return;
    }

    // Create regex pattern based on settings
    const pattern = allowNegative ? /^-?\d*[,.]?\d{0,2}$/ : /^\d*[,.]?\d{0,2}$/;

    // Validate the input format
    if (pattern.test(newValue)) {
      setDisplayValue(newValue);
      lastValidValue.current = newValue;
    } else if (newValue.startsWith("-") && !allowNegative) {
      // Don't allow negative if not permitted
      setDisplayValue(newValue.substring(1));
    }
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    // Allow backspace, delete, tab, escape, enter
    if ([8, 9, 27, 13].indexOf(event.keyCode) !== -1) {
      return;
    }

    // Allow Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    if (
      [65, 67, 86, 88].indexOf(event.keyCode) !== -1 &&
      (event.ctrlKey || event.metaKey)
    ) {
      return;
    }

    // Ensure that it is a number or decimal separator
    if (
      (event.shiftKey || event.keyCode < 48 || event.keyCode > 57) &&
      (event.keyCode < 96 || event.keyCode > 105) &&
      event.keyCode !== 188 && // comma
      event.keyCode !== 190 && // period
      event.keyCode !== 110
    ) {
      // numpad decimal
      event.preventDefault();
    }
  };

  return (
    <TextField
      {...props}
      value={displayValue}
      onChange={handleChange}
      onFocus={handleFocus}
      onBlur={handleBlur}
      onKeyDown={handleKeyDown}
      inputRef={inputRef}
      type="text"
      inputProps={{
        ...props.inputProps,
        style: { textAlign: "right", ...props.inputProps?.style },
      }}
    />
  );
};

export default NumberFormatTextField;
