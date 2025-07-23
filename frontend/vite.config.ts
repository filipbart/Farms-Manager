import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    // host: true,
    port: 3000,
    allowedHosts: ["a02041060697.ngrok-free.app"],
    proxy: {
      "/api": {
        target: "http://localhost:8082",
        changeOrigin: true,
        ws: true,
      },
    },
  },
});
