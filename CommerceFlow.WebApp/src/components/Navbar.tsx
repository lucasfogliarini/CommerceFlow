"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useCart } from "./CartProvider";
import { useState } from "react";

export default function Navbar() {
  const pathname = usePathname();
  const { totalItems } = useCart();
  const [menuOpen, setMenuOpen] = useState(false);

  const links = [
    { href: "/", label: "Início" },
    { href: "/products", label: "Produtos" },
  ];

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link href="/" className="navbar-logo">
          <span className="navbar-logo-icon">⚡</span>
          <span>CommerceFlow</span>
        </Link>

        <button
          className="navbar-hamburger"
          onClick={() => setMenuOpen(!menuOpen)}
          aria-label="Toggle menu"
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
        </ul>
      </div>
    </nav>
  );
}
