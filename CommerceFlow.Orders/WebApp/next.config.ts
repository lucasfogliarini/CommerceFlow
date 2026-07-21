import type { NextConfig } from "next";
import path from "node:path";

const nextConfig: NextConfig = {
  output: "standalone",
  env: {
    NEXT_PUBLIC_NOTIFICATION_HUB_URL: `${process.env.COMMERCEFLOW_API_URL}/hubs/notifications`,
  },
  turbopack: {
    root: path.resolve(__dirname),
  },
};

export default nextConfig;
