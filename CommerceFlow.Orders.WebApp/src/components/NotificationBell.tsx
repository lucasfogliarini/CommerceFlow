"use client";

import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { useKeycloak } from "./KeycloakProvider";

type Notification = { orderId: string; message: string };
type RuntimeConfig = { notificationHubUrl: string };

export default function NotificationBell() {
  const { keycloak, authenticated } = useKeycloak();
  const notificationsEnabled = true;
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    if (!notificationsEnabled || !authenticated || !keycloak?.token) return;

    let connection: ReturnType<HubConnectionBuilder["build"]> | null = null;

    const startConnection = async () => {
      const runtimeConfigResponse = await fetch("/api/config", { cache: "no-store" });
      if (!runtimeConfigResponse.ok) {
        return;
      }

      const runtimeConfig = (await runtimeConfigResponse.json()) as RuntimeConfig;
      connection = new HubConnectionBuilder()
        .withUrl(runtimeConfig.notificationHubUrl, {
          accessTokenFactory: () => keycloak.token ?? "",
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Warning)
        .build();

      connection.on("ReceiveNotification", (notification: Notification) => {
        setNotifications((current) => [notification, ...current].slice(0, 20));
        setUnreadCount((current) => current + 1);
      });

      await connection.start();
    };

    void startConnection();

    return () => {
      if (connection) {
        void connection.stop();
      }
    };
  }, [authenticated, keycloak, notificationsEnabled]);

  if (!notificationsEnabled || !authenticated) return null;

  return <div className="notification-bell"><button type="button" className="notification-bell-trigger" aria-label="Notificações" aria-expanded={open} onClick={() => { setOpen(!open); setUnreadCount(0); }}><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 9a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9M10 21h4" /></svg>{unreadCount > 0 && <b>{unreadCount}</b>}</button>{open && <div className="notification-menu">{notifications.length ? notifications.map((notification, index) => <p key={`${notification.orderId}-${index}`}>{notification.message}</p>) : <p>Nenhuma notificação.</p>}</div>}</div>;
}
