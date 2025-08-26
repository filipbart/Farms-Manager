import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default ({ mode }: { mode: string }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return defineConfig({
    base: "/panel/",
    plugins: [react(), tailwindcss()],
    server: {
      port: Number(env.SERVER_PORT) || 3000,
      allowedHosts: [env.VITE_ALLOWED_HOSTS],
      proxy: {
        "/api": {
          target: env.VITE_API_BASE_URL,
          changeOrigin: true,
          ws: true,
        },
      },
    },
  });
};
