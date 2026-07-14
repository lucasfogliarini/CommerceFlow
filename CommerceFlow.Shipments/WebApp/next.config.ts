import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  env: {
    NEXT_PUBLIC_NOTIFICATION_HUB_URL: `${process.env.COMMERCEFLOW_API_URL}/hubs/notifications`,
  },
};

export default nextConfig;
