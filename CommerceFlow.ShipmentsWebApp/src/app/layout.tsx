import type { Metadata } from "next";
import "./globals.css";
import { KeycloakProvider } from "@/components/KeycloakProvider";
import Sidebar from "@/components/Sidebar";
import Topbar from "@/components/Topbar";

export const metadata: Metadata = {
  title: "RoutePulse | Logistics Operations",
  description: "RoutePulse logistics command center for shipment operations.",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>
        <KeycloakProvider>
          <div className="app-shell">
            <Sidebar />
            <div className="workspace"><Topbar /><main>{children}</main></div>
          </div>
        </KeycloakProvider>
      </body>
    </html>
  );
}
