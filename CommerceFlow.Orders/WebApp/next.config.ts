import type { NextConfig } from "next";
import path from "node:path";

const nextConfig: NextConfig = {
  output: "standalone",
  env: {
    NEXT_PUBLIC_NOTIFICATION_HUB_URL: `${process.env.ORDERS_API_URL}/hubs/notifications`,
    NEXT_PUBLIC_KEYCLOAK_URL: process.env.KEYCLOAK_URL,
  },
  turbopack: {
    root: path.resolve(__dirname),
  },
};

export default nextConfig;
