"use client";

import { useEffect, useState } from "react";
import { Product } from "@/types";
import { fetchProducts } from "@/lib/api";
import ProductCard from "@/components/ProductCard";

export default function ProductsPage() {
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

  return (
    <>
      <div className="page-header">
        <h1>
          Nossos <span className="text-gradient">Produtos</span>
        </h1>
        <p>Explore todo o nosso catálogo</p>
      </div>

      <section className="products-section" style={{ paddingTop: 0 }}>
        <div className="container">
          {loading ? (
            <div className="loading-grid">
              {[...Array(8)].map((_, i) => (
                <div key={i} className="loading-card" />
              ))}
            </div>
          ) : products.length > 0 ? (
            <>
              <p
                style={{
                  color: "var(--text-secondary)",
                  marginBottom: "24px",
                  fontSize: "0.95rem",
                }}
              >
                {products.length} produto{products.length !== 1 ? "s" : ""}{" "}
                encontrado{products.length !== 1 ? "s" : ""}
              </p>
              <div className="products-grid">
                {products.map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>
            </>
          ) : (
            <div style={{ textAlign: "center", padding: "80px 0" }}>
              <div style={{ fontSize: "4rem", marginBottom: "16px", opacity: 0.3 }}>
                📦
              </div>
              <h2 style={{ marginBottom: "8px" }}>Nenhum produto encontrado</h2>
              <p style={{ color: "var(--text-secondary)" }}>
                Verifique se a API está rodando em{" "}
                <code style={{ color: "var(--accent-cyan)" }}>
                  localhost:2008
                </code>
              </p>
            </div>
          )}
        </div>
      </section>
    </>
  );
}
