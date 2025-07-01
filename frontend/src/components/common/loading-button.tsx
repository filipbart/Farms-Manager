import React from "react";
import { Button, type ButtonProps } from "@mui/material";
import Loading from "../loading/loading";

interface LoadingButtonProps extends ButtonProps {
  loading: boolean;
  loadingSize?: number;
  height?: string;
}

const LoadingButton: React.FC<LoadingButtonProps> = ({
  loading,
  loadingSize = 10,
  height = "0",
  children,
  ...buttonProps
}) => {
  if (loading) {
    return (
      <div style={{ marginLeft: "0.25rem" }}>
        <Loading height={height} size={loadingSize} />
      </div>
    );
  }

  return <Button {...buttonProps}>{children}</Button>;
};

export default LoadingButton;
