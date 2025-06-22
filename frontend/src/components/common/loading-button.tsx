import React from "react";
import { Button, type ButtonProps } from "@mui/material";
import Loading from "../loading/loading";

interface LoadingButtonProps extends ButtonProps {
  loading: boolean;
  loadingSize?: number;
}

const LoadingButton: React.FC<LoadingButtonProps> = ({
  loading,
  loadingSize = 10,
  children,
  ...buttonProps
}) => {
  if (loading) {
    return (
      <div style={{ marginLeft: "0.25rem" }}>
        <Loading height="0" size={loadingSize} />
      </div>
    );
  }

  return <Button {...buttonProps}>{children}</Button>;
};

export default LoadingButton;
