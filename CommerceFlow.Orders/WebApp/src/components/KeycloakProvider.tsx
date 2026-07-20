"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import type Keycloak from "keycloak-js";

interface KeycloakContextValue {
  keycloak: Keycloak | null;
  authenticated: boolean;
  initialized: boolean;
  error: Error | null;
  login: () => Promise<void>;
  logout: () => Promise<void>;
}

const KeycloakContext = createContext<KeycloakContextValue | undefined>(undefined);

export function KeycloakProvider({ children }: { children: React.ReactNode }) {
  const [keycloak, setKeycloak] = useState<Keycloak | null>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const [initialized, setInitialized] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    let active = true;

    import("keycloak-js")
      .then(async ({ default: KeycloakClient }) => {
        const client = new KeycloakClient({
          url: process.env.NEXT_PUBLIC_KEYCLOAK_URL || "http://localhost:2006/",
          realm: "commerceflow",
          clientId: "commerceflow",
        });
        const isAuthenticated = await client.init({ onLoad: "check-sso" });

        if (active) {
          setKeycloak(client);
          setAuthenticated(isAuthenticated);
          setInitialized(true);
        }
      })
      .catch((reason: unknown) => {
        if (active) {
          setError(reason instanceof Error ? reason : new Error("Keycloak initialization failed"));
          setInitialized(true);
        }
      });

    return () => {
      active = false;
    };
  }, []);

  const login = useCallback(async () => {
    if (!keycloak) {
      throw new Error("Keycloak is not initialized");
    }

    await keycloak.login();
  }, [keycloak]);

  const logout = useCallback(async () => {
    if (!keycloak) throw new Error("Keycloak is not initialized");
    await keycloak.logout({ redirectUri: window.location.origin });
  }, [keycloak]);

  const value = useMemo(
    () => ({ keycloak, authenticated, initialized, error, login, logout }),
    [keycloak, authenticated, initialized, error, login, logout]
  );

  return <KeycloakContext.Provider value={value}>{children}</KeycloakContext.Provider>;
}

export function useKeycloak() {
  const context = useContext(KeycloakContext);

  if (!context) {
    throw new Error("useKeycloak must be used within a KeycloakProvider");
  }

  return context;
}
