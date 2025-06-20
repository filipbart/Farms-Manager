import React from "react";
import { PulseLoader } from "react-spinners";

const Loading: React.FC = () => {
  return (
    <div
      className="loading-container"
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
      }}
    >
      <PulseLoader color={"#0D1B2A"} loading size={50} />
    </div>
  );
};

export default Loading;
