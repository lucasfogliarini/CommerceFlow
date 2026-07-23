"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import type Keycloak from "keycloak-js";

interface KeycloakContextValue {
  keycloak: Keycloak | null;
  authenticated: boolean;
  initialized: boolean;
  login: () => Promise<void>;
  logout: () => Promise<void>;
}

const KeycloakContext = createContext<KeycloakContextValue | undefined>(undefined);

type RuntimeConfig = {
  keycloakUrl: string;
};

export function KeycloakProvider({ children }: { children: React.ReactNode }) {
  const [keycloak, setKeycloak] = useState<Keycloak | null>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    let active = true;

    import("keycloak-js")
      .then(async ({ default: KeycloakClient }) => {
        const runtimeConfigResponse = await fetch("/api/config", { cache: "no-store" });
        if (!runtimeConfigResponse.ok) {
          throw new Error("Failed to load runtime config");
        }

        const runtimeConfig = (await runtimeConfigResponse.json()) as RuntimeConfig;

        const client = new KeycloakClient({
          url: runtimeConfig.keycloakUrl,
          realm: "commerceflow",
          clientId: "commerceflow",
        });
        const isAuthenticated = await client.init({
          onLoad: "check-sso"
        });

        if (active) {
          setKeycloak(client);
          setAuthenticated(isAuthenticated);
          setInitialized(true);
        }
      })
      .catch(() => {
        if (active) setInitialized(true);
      });

    return () => {
      active = false;
    };
  }, []);

  const login = useCallback(async () => {
    if (!keycloak) {
      return;
    }

    await keycloak.login();
  }, [keycloak]);

  const logout = useCallback(async () => {
    if (!keycloak) {
      return;
    }

    await keycloak.logout({ redirectUri: window.location.origin });
  }, [keycloak]);

  const value = useMemo(
    () => ({ keycloak, authenticated, initialized, login, logout }),
    [keycloak, authenticated, initialized, login, logout]
  );

  return <KeycloakContext.Provider value={value}>{children}</KeycloakContext.Provider>;
}

export function useKeycloak() {
  const context = useContext(KeycloakContext);

  if (!context) throw new Error("useKeycloak must be used within a KeycloakProvider");

  return context;
}
