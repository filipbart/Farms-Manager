import { TextField, type TextFieldProps } from "@mui/material";
import Loading from "../loading/loading"; // <- dostosuj do swojej ścieżki
import React from "react";

interface LoadingTextFieldProps extends Omit<TextFieldProps, "value"> {
  loading: boolean;
  value: string;
}

const LoadingTextField: React.FC<LoadingTextFieldProps> = ({
  loading,
  value,
  ...textFieldProps
}) => {
  return loading ? (
    <Loading height="0" size={10} />
  ) : (
    <TextField
      {...textFieldProps}
      value={value}
      slotProps={{
        input: { readOnly: true },
        ...(textFieldProps.slotProps ?? {}),
      }}
    />
  );
};

export default LoadingTextField;
