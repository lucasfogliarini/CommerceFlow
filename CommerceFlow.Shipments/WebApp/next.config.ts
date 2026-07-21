import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  env: {
    NEXT_PUBLIC_NOTIFICATION_HUB_URL: `${process.env.SHIPMENTS_API_URL}/hubs/notifications`,
    NEXT_PUBLIC_KEYCLOAK_URL: process.env.KEYCLOAK_URL,
  },
};

export default nextConfig;
