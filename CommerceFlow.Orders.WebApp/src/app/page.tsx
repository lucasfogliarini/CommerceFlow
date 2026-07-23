"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Product } from "@/types";
import { fetchProducts } from "@/lib/api";
import ProductCard from "@/components/ProductCard";

export default function HomePage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProducts()
      .then((data) => {
        const items = Array.isArray(data) ? data : [];
        setProducts(items);
      })
      .catch(() => setProducts([]))
      .finally(() => setLoading(false));
  }, []);

  const featured = products.slice(0, 8);

  return (
    <>
      {/* Hero Section */}
      <section className="hero">
        <div className="hero-bg">
          <div className="hero-orb hero-orb-1" />
          <div className="hero-orb hero-orb-2" />
          <div className="hero-orb hero-orb-3" />
        </div>
        <div className="hero-content">
          <div className="hero-badge">
            <span className="hero-badge-dot" />
            Novidades disponíveis
          </div>
          <h1 className="hero-title">
            Compre com{" "}
            <span className="hero-title-gradient">estilo e confiança</span>
          </h1>
          <p className="hero-description">
            Descubra nossa curadoria de produtos premium. Qualidade excepcional,
            entrega rápida e a melhor experiência de compra online.
          </p>
          <div className="hero-actions">
            <Link href="/products" className="btn btn-primary btn-lg">
              🛍️ Ver Produtos
            </Link>
            <Link href="/cart" className="btn btn-secondary btn-lg">
              🛒 Meu Carrinho
            </Link>
          </div>
        </div>
      </section>

      {/* Featured Products */}
      <section className="products-section">
        <div className="container">
          <div className="products-header">
            <h2 className="section-title">Produtos em Destaque</h2>
            <p className="section-subtitle">
              Os mais populares da nossa loja
            </p>
          </div>

          {loading ? (
            <div className="loading-grid">
              {[...Array(4)].map((_, i) => (
                <div key={i} className="loading-card" />
              ))}
            </div>
          ) : featured.length > 0 ? (
            <div className="products-grid">
              {featured.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          ) : (
            <div style={{ textAlign: "center", padding: "60px 0" }}>
              <p style={{ fontSize: "1.1rem", color: "var(--text-secondary)" }}>
                Nenhum produto encontrado.
              </p>
            </div>
          )}

          {featured.length > 0 && (
            <div style={{ textAlign: "center", marginTop: "40px" }}>
              <Link href="/products" className="btn btn-secondary">
                Ver todos os produtos →
              </Link>
            </div>
          )}
        </div>
      </section>
    </>
  );
}
