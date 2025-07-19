import { TextField, type TextFieldProps } from "@mui/material";
import Loading from "../loading/loading";
import React from "react";

interface LoadingTextFieldProps extends Omit<TextFieldProps, "value"> {
  loading: boolean;
  value?: string;
  children?: React.ReactNode;
}

const LoadingTextField: React.FC<LoadingTextFieldProps> = ({
  loading,
  value,
  children,
  ...textFieldProps
}) => {
  return loading ? (
    <div
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100%",
        width: "100%",
      }}
    >
      <Loading height="0" size={10} />
    </div>
  ) : (
    <TextField {...textFieldProps} value={value}>
      {children}
    </TextField>
  );
};

export default LoadingTextField;
