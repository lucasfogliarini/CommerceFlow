"use client";

import { useEffect, useState, useRef, use } from "react";
import { useRouter } from "next/navigation";
import { approvePayment } from "@/lib/api";

export default function PaymentPage({ params }: { params: Promise<{ orderId: string }> }) {
  const router = useRouter();
  const { orderId } = use(params);

  const [keycloak, setKeycloak] = useState<any>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
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

  const handlePayment = async () => {
    setLoading(true);
    setError("");

    try {
      const token = keycloak?.token;
      // Simulando um payment reference (ex: gerado por gateway de pagamento)
      const paymentReference = crypto.randomUUID();
      
      const res = await approvePayment(orderId, paymentReference, token);

      if (!res.ok) {
        const data = await res.json().catch(() => null);
        throw new Error(data?.error || `Erro ao aprovar pagamento (${res.status})`);
      }

      router.push("/checkout/success");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao processar o pagamento.");
    } finally {
      setLoading(false);
    }
  };

  if (!authenticated) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        Autenticando...
      </div>
    );
  }

  return (
    <div className="checkout-page">
      <div className="container" style={{ maxWidth: "600px", margin: "0 auto", textAlign: "center", paddingTop: "60px" }}>
        <h1 style={{ fontSize: "2rem", marginBottom: "16px" }}>💳 Pagamento do Pedido</h1>
        <p style={{ color: "var(--text-secondary)", marginBottom: "32px" }}>
          ID do Pedido: <strong>{orderId}</strong>
        </p>

        {error && (
          <div style={{ marginBottom: "20px", color: "var(--accent-rose)" }}>
            ⚠️ {error}
          </div>
        )}

        <button
          onClick={handlePayment}
          disabled={loading}
          className="btn btn-primary btn-lg"
          style={{ width: "100%" }}
        >
          {loading ? (
            <><span className="spinner" /> Processando Pagamento...</>
          ) : (
            "Simular Pagamento Aprovado"
          )}
        </button>
      </div>
    </div>
  );
}
