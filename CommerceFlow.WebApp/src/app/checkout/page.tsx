"use client";

import { useState, useEffect, useRef } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useCart } from "@/components/CartProvider";
import { createOrder } from "@/lib/api";
import { Address, CreateOrderRequest } from "@/types";

export default function CheckoutPage() {
  const router = useRouter();
  const { items, totalPrice, clearCart } = useCart();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const [keycloak, setKeycloak] = useState<any>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const isRun = useRef(false);

  useEffect(() => {
    if (typeof window !== "undefined" && !isRun.current) {
      isRun.current = true;
      import("keycloak-js").then(({ default: Keycloak }) => {
        const kc = new Keycloak({
          url: "http://localhost:2006/",
          realm: "commerceflow",
          clientId: "commerceflow",
        });
        kc.init({ onLoad: "login-required" })
          .then((auth) => {
            setKeycloak(kc);
            setAuthenticated(auth);
          })
          .catch(() => console.error("Keycloak init failed"));
      });
    }
  }, []);

  const [address, setAddress] = useState<Address>({
    street: "",
    number: "",
    city: "",
    state: "",
    zipCode: "",
    country: "Brasil",
  });

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(price);
  };

  const handleInputChange = (field: keyof Address, value: string) => {
    setAddress((prev) => ({ ...prev, [field]: value }));
  };

  const isValid = () => {
    return (
      address.street.trim() &&
      address.street.trim() &&
      address.number.trim() &&
      address.city.trim() &&
      address.state.trim() &&
      address.zipCode.trim() &&
      address.country.trim() &&
      items.length > 0
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isValid()) return;

    setLoading(true);
    setError("");

    const orderRequest: CreateOrderRequest = {
      shippingAddress: address,
      items: items.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity,
      })),
    };

    try {
      const token = keycloak?.token;
      const res = await createOrder(orderRequest, token);

      if (!res.ok) {
        const data = await res.json().catch(() => null);
        throw new Error(data?.error || `Erro ao criar pedido (${res.status})`);
      }
      
      const orderId = await res.json();

      clearCart();
      router.push(`/payment/${orderId}`);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Erro ao processar o pedido."
      );
    } finally {
      setLoading(false);
    }
  };

  if (items.length === 0) {
    return (
      <div className="checkout-page">
        <div className="container" style={{ textAlign: "center", paddingTop: "60px" }}>
          <div style={{ fontSize: "4rem", marginBottom: "16px", opacity: 0.3 }}>
            🛒
          </div>
          <h2 style={{ marginBottom: "8px" }}>Carrinho vazio</h2>
          <p style={{ color: "var(--text-secondary)", marginBottom: "24px" }}>
            Adicione produtos antes de finalizar o pedido.
          </p>
          <Link href="/products" className="btn btn-primary">
            🛍️ Ver Produtos
          </Link>
        </div>
      </div>
    );
  }

  if (!authenticated) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        Autenticando...
      </div>
    );
  }

  return (
    <div className="checkout-page">
      <div className="container">
        <div className="cart-header">
          <h1 style={{ fontSize: "1.8rem", fontWeight: 800 }}>
            📋 <span className="text-gradient">Checkout</span>
          </h1>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="checkout-layout">
            {/* Form */}
            <div>
              {/* Shipping Address */}
              <div className="checkout-form">
                <h2>🚚 Endereço de Entrega</h2>
                <div className="form-grid">
                  <div className="form-group span-2">
                    <label className="form-label">Rua</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="Nome da rua"
                      value={address.street}
                      onChange={(e) =>
                        handleInputChange("street", e.target.value)
                      }
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Número</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="123"
                      value={address.number}
                      onChange={(e) =>
                        handleInputChange("number", e.target.value)
                      }
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label className="form-label">CEP</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="00000-000"
                      value={address.zipCode}
                      onChange={(e) =>
                        handleInputChange("zipCode", e.target.value)
                      }
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Cidade</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="São Paulo"
                      value={address.city}
                      onChange={(e) =>
                        handleInputChange("city", e.target.value)
                      }
                      required
                    />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Estado</label>
                    <input
                      type="text"
                      className="form-input"
                      placeholder="SP"
                      value={address.state}
                      onChange={(e) =>
                        handleInputChange("state", e.target.value)
                      }
                      required
                    />
                  </div>
                  <div className="form-group span-2">
                    <label className="form-label">País</label>
                    <input
                      type="text"
                      className="form-input"
                      value={address.country}
                      onChange={(e) =>
                        handleInputChange("country", e.target.value)
                      }
                      required
                    />
                  </div>
                </div>
              </div>

              {error && (
                <div
                  style={{
                    marginTop: "16px",
                    padding: "14px 20px",
                    background: "rgba(244, 63, 94, 0.1)",
                    border: "1px solid rgba(244, 63, 94, 0.2)",
                    borderRadius: "var(--radius-md)",
                    color: "var(--accent-rose)",
                    fontSize: "0.9rem",
                  }}
                >
                  ⚠️ {error}
                </div>
              )}
            </div>

            {/* Summary */}
            <div className="cart-summary">
              <h3>Resumo do Pedido</h3>

              {items.map((item) => (
                <div
                  key={item.product.id}
                  className="cart-summary-row"
                  style={{ fontSize: "0.85rem" }}
                >
                  <span style={{ flex: 1, minWidth: 0, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                    {item.product.name} × {item.quantity}
                  </span>
                  <span style={{ marginLeft: "12px", flexShrink: 0 }}>
                    {formatPrice(item.product.unitPrice * item.quantity)}
                  </span>
                </div>
              ))}

              <div className="cart-summary-row" style={{ marginTop: "8px" }}>
                <span>Frete</span>
                <span style={{ color: "var(--accent-emerald)" }}>Grátis</span>
              </div>

              <div className="cart-summary-total">
                <span>Total</span>
                <span>{formatPrice(totalPrice)}</span>
              </div>

              <button
                type="submit"
                className="btn btn-success btn-lg"
                disabled={!isValid() || loading}
              >
                {loading ? (
                  <>
                    <span className="spinner" /> Processando...
                  </>
                ) : (
                  "✓ Confirmar Pedido"
                )}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
