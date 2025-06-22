import React from "react";
import { PulseLoader } from "react-spinners";

interface LoadingProps {
  height?: string;
  size?: number;
}

const Loading: React.FC<LoadingProps> = ({ height = "100vh", size = 30 }) => {
  return (
    <div
      className="loading-container"
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: height,
      }}
    >
      <PulseLoader color={"#0D1B2A"} loading size={size} />
    </div>
  );
};

export default Loading;
