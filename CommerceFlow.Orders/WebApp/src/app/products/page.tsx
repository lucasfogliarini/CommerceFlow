"use client";

import { useEffect, useState } from "react";
import { Product } from "@/types";
import { fetchProductPage, PRODUCTS_PER_PAGE, ProductFilters } from "@/lib/api";
import ProductCard from "@/components/ProductCard";

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [totalProducts, setTotalProducts] = useState(0);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [minimumPrice, setMinimumPrice] = useState("");
  const [maximumPrice, setMaximumPrice] = useState("");
  const [priceOrder, setPriceOrder] = useState<"" | "asc" | "desc">("");
  const [filters, setFilters] = useState<ProductFilters>({});
  const [currentPage, setCurrentPage] = useState(1);
  const [filterError, setFilterError] = useState("");

  useEffect(() => {
    fetchProductPage(filters, currentPage)
      .then((data) => {
        setProducts(data.products);
        setTotalProducts(data.total);
      })
      .catch(() => {
        setProducts([]);
        setTotalProducts(0);
      })
      .finally(() => setLoading(false));
  }, [filters, currentPage]);

  const handleFiltersSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const minimum = minimumPrice === "" ? undefined : Number(minimumPrice);
    const maximum = maximumPrice === "" ? undefined : Number(maximumPrice);

    if ((minimum !== undefined && (!Number.isFinite(minimum) || minimum < 0)) ||
        (maximum !== undefined && (!Number.isFinite(maximum) || maximum < 0))) {
      setFilterError("Informe preços válidos.");
      return;
    }

    if (minimum !== undefined && maximum !== undefined && minimum > maximum) {
      setFilterError("O preço mínimo não pode ser maior que o preço máximo.");
      return;
    }

    setFilterError("");
    setCurrentPage(1);
    setFilters({
      search: search || undefined,
      minimumPrice: minimum,
      maximumPrice: maximum,
      priceOrder: priceOrder || undefined,
    });
  };

  const clearFilters = () => {
    setSearch("");
    setMinimumPrice("");
    setMaximumPrice("");
    setPriceOrder("");
    setFilterError("");
    setCurrentPage(1);
    setFilters({});
  };

  const totalPages = Math.ceil(totalProducts / PRODUCTS_PER_PAGE);

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
          <form className="product-filters" onSubmit={handleFiltersSubmit}>
            <div className="form-group product-filters-search">
              <label className="form-label" htmlFor="product-search">Nome ou descrição</label>
              <input
                className="form-input"
                id="product-search"
                type="search"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Busque um produto"
              />
            </div>
            <div className="form-group">
              <label className="form-label" htmlFor="minimum-price">Preço mínimo</label>
              <input
                className="form-input"
                id="minimum-price"
                type="number"
                min="0"
                step="0.01"
                value={minimumPrice}
                onChange={(event) => setMinimumPrice(event.target.value)}
              />
            </div>
            <div className="form-group">
              <label className="form-label" htmlFor="maximum-price">Preço máximo</label>
              <input
                className="form-input"
                id="maximum-price"
                type="number"
                min="0"
                step="0.01"
                value={maximumPrice}
                onChange={(event) => setMaximumPrice(event.target.value)}
              />
            </div>
            <div className="form-group">
              <label className="form-label" htmlFor="price-order">Ordenar por preço</label>
              <select
                className="form-input"
                id="price-order"
                value={priceOrder}
                onChange={(event) => setPriceOrder(event.target.value as "" | "asc" | "desc")}
              >
                <option value="">Sem ordenação</option>
                <option value="asc">Menor preço</option>
                <option value="desc">Maior preço</option>
              </select>
            </div>
            <div className="product-filters-actions">
              <button className="btn btn-primary" type="submit">Filtrar</button>
              <button className="btn" type="button" onClick={clearFilters}>Limpar</button>
            </div>
            {filterError && <p className="form-error product-filters-error">{filterError}</p>}
          </form>
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
                {totalProducts} produto{totalProducts !== 1 ? "s" : ""}{" "}
                encontrado{totalProducts !== 1 ? "s" : ""}
              </p>
              <div className="products-grid">
                {products.map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>
              {totalPages > 1 && (
                <nav className="products-pagination" aria-label="Paginação de produtos">
                  <button
                    className="btn"
                    type="button"
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage((page) => page - 1)}
                  >
                    Anterior
                  </button>
                  <span>
                    Página {currentPage} de {totalPages}
                  </span>
                  <button
                    className="btn btn-primary"
                    type="button"
                    disabled={currentPage === totalPages}
                    onClick={() => setCurrentPage((page) => page + 1)}
                  >
                    Próxima
                  </button>
                </nav>
              )}
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
