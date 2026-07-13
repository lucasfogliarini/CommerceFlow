"use client";

import { useEffect, useState, use } from "react";
import { useRouter } from "next/navigation";
import { useKeycloak } from "@/components/KeycloakProvider";
import { approvePayment } from "@/lib/api";

export default function PaymentPage({ params }: { params: Promise<{ orderId: string }> }) {
  const router = useRouter();
  const { orderId } = use(params);

  const { keycloak, authenticated, initialized, error: authError, login } = useKeycloak();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (initialized && !authenticated && !authError) {
      void login();
    }
  }, [authenticated, authError, initialized, login]);

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

  if (!initialized || !authenticated) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        {authError ? "Não foi possível autenticar." : "Autenticando..."}
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
