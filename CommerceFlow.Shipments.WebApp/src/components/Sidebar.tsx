"use client";

import Link from "next/link";
import { useKeycloak } from "./KeycloakProvider";

export default function Sidebar() {
  const { authenticated, initialized, keycloak, login, logout } = useKeycloak();
  const operator = keycloak?.tokenParsed?.preferred_username || keycloak?.tokenParsed?.name || "Operations team";

  return (
    <aside className="sidebar">
      <div className="brand">
        <span className="brand-mark">↗</span>
        <span>Route<span>Pulse</span></span>
      </div>

      <nav className="sidebar-nav" aria-label="Primary navigation">
        <p className="nav-label">Workspace</p>
        <Link href="/shipments" className="nav-item active">
          <span aria-hidden="true">▱</span>Shipments
        </Link>
      </nav>

      <div className="sidebar-bottom">
        <div className="operator-status"><span /> Network operating normally</div>
        {authenticated ? (
          <button className="profile-card" type="button" onClick={() => void logout()} title="Sign out">
            <span className="avatar">{operator.charAt(0).toUpperCase()}</span>
            <span><strong>{operator}</strong><small>Logistics operator</small></span>
            <span aria-hidden="true">⌄</span>
          </button>
        ) : (
          <button className="sign-in" type="button" disabled={!initialized} onClick={() => void login()}>
            {initialized ? "Operator sign in" : "Connecting..."}
          </button>
        )}
      </div>
    </aside>
  );
}
