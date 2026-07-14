"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useCart } from "./CartProvider";
import { useKeycloak } from "./KeycloakProvider";
import NotificationBell from "./NotificationBell";
import { useState } from "react";

export default function Navbar() {
  const pathname = usePathname();
  const { totalItems } = useCart();
  const { keycloak, authenticated, initialized, login, logout } = useKeycloak();
  const [menuOpen, setMenuOpen] = useState(false);
  const [accountMenuOpen, setAccountMenuOpen] = useState(false);
  const accountName = keycloak?.tokenParsed?.preferred_username || keycloak?.tokenParsed?.name || "Minha conta";

  const links = [
    { href: "/", label: "Início" },
    { href: "/products", label: "Produtos" },
  ];

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link href="/" className="navbar-logo">
          <span className="navbar-logo-icon" aria-hidden="true">CF</span>
          <span className="navbar-logo-text"><span>Commerce</span>Flow</span>
        </Link>

        <button
          className="navbar-hamburger"
          onClick={() => setMenuOpen(!menuOpen)}
          aria-label={menuOpen ? "Fechar menu" : "Abrir menu"}
          aria-expanded={menuOpen}
        >
          {menuOpen ? "✕" : "☰"}
        </button>

        <ul className={`navbar-links ${menuOpen ? "open" : ""}`}>
          {links.map((link) => (
            <li key={link.href}>
              <Link
                href={link.href}
                className={`navbar-link ${pathname === link.href ? "active" : ""}`}
                onClick={() => setMenuOpen(false)}
              >
                {link.label}
              </Link>
            </li>
          ))}
          <li>
            <NotificationBell />
          </li>
          <li>
            <Link
              href="/cart"
              className="navbar-cart"
              onClick={() => setMenuOpen(false)}
            >
              🛒 Carrinho
              {totalItems > 0 && (
                <span className="navbar-cart-badge">{totalItems}</span>
              )}
            </Link>
          </li>
          <li className="navbar-account-menu">
            {authenticated ? (
              <>
                <button
                  type="button"
                  className="navbar-account-trigger"
                  aria-haspopup="menu"
                  aria-expanded={accountMenuOpen}
                  aria-controls="account-navigation-menu"
                  onClick={() => setAccountMenuOpen((open) => !open)}
                  onKeyDown={(event) => {
                    if (event.key === "Escape") setAccountMenuOpen(false);
                  }}
                >
                  <span className="navbar-account-avatar" aria-hidden="true">{accountName.charAt(0).toUpperCase()}</span>
                  <span className="navbar-account-name">{accountName}</span>
                  <span aria-hidden="true">▾</span>
                </button>
                {accountMenuOpen && (
                  <ul id="account-navigation-menu" className="navbar-account-dropdown" role="menu">
                    <li role="none"><Link href="/account" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Minha Conta</Link></li>
                    <li role="none"><Link href="/orders" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Pedidos</Link></li>
                    <li role="none"><Link href="/addresses" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Endereços</Link></li>
                    <li role="none"><Link href="/account#payments" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Pagamentos</Link></li>
                    <li role="none"><Link href="/account#refunds" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Reembolsos</Link></li>
                    <li role="none"><Link href="/account#wishlist" role="menuitem" onClick={() => setAccountMenuOpen(false)}>Lista de desejos</Link></li>
                    <li role="none"><button type="button" className="navbar-account-logout" role="menuitem" onClick={() => void logout()}>Sair</button></li>
                  </ul>
                )}
              </>
            ) : (
              <button
                type="button"
                className="navbar-sign-in"
                disabled={!initialized}
                onClick={() => void login()}
              >
                {initialized ? "Entrar" : "Carregando..."}
              </button>
            )}
          </li>
        </ul>
      </div>
    </nav>
  );
}
