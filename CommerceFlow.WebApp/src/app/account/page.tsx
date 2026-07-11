"use client";

import { useEffect, useState, useRef } from "react";
import { useRouter } from "next/navigation";
import { getMyAccount } from "@/lib/api";
import { AccountResponse } from "@/types";

export default function AccountPage() {
  const router = useRouter();

  const [keycloak, setKeycloak] = useState<any>(null);
  const [authenticated, setAuthenticated] = useState(false);
  const [account, setAccount] = useState<AccountResponse | null>(null);
  const [loading, setLoading] = useState(true);
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

  useEffect(() => {
    if (authenticated && keycloak?.token) {
      getMyAccount(keycloak.token)
        .then((data) => {
          setAccount(data);
        })
        .catch((err) => {
          setError(err.message || "Erro ao carregar os dados da conta.");
        })
        .finally(() => {
          setLoading(false);
        });
    }
  }, [authenticated, keycloak]);

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(price);
  };

  const getStatusLabel = (status: string) => {
    const labels: Record<string, string> = {
      "Created": "Criado",
      "WaitingForPayment": "Aguardando Pagamento",
      "PaymentApproved": "Pagamento Aprovado",
      "PaymentRejected": "Pagamento Rejeitado",
      "PaymentExpired": "Pagamento Expirado",
      "ReadyForShipment": "Pronto para Envio",
      "Dispatched": "Enviado",
      "Delivered": "Entregue",
      "Cancelled": "Cancelado"
    };
    return labels[status] || status;
  };

  if (!authenticated) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        Autenticando...
      </div>
    );
  }

  if (loading) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        <span className="spinner" /> Carregando conta...
      </div>
    );
  }

  return (
    <div className="checkout-page">
      <div className="container" style={{ maxWidth: "800px", margin: "0 auto", paddingTop: "40px" }}>
        <h1 style={{ fontSize: "2rem", marginBottom: "32px", fontWeight: 800 }}>
          👤 <span className="text-gradient">Minha Conta</span>
        </h1>

        {error && (
          <div style={{ marginBottom: "20px", color: "var(--accent-rose)", padding: "16px", background: "rgba(244, 63, 94, 0.1)", borderRadius: "var(--radius-md)" }}>
            ⚠️ {error}
          </div>
        )}

        <div style={{ marginBottom: "40px", background: "var(--bg-card)", padding: "24px", borderRadius: "var(--radius-lg)", border: "1px solid var(--border-color)" }}>
          <h2 style={{ fontSize: "1.2rem", marginBottom: "16px" }}>Dados Pessoais</h2>
          {account?.customer ? (
            <div>
              <p><strong>Nome:</strong> {account.customer.name}</p>
              <p><strong>E-mail:</strong> {account.customer.email}</p>
            </div>
          ) : (
            <div>
              <p><strong>E-mail de Login:</strong> {account?.email}</p>
              <p style={{ color: "var(--text-secondary)", fontSize: "0.9rem", marginTop: "8px" }}>
                Perfil completo não encontrado. Ele será criado ou sincronizado automaticamente em sua primeira compra.
              </p>
            </div>
          )}
        </div>

        <div>
          <h2 style={{ fontSize: "1.5rem", marginBottom: "20px" }}>Meus Pedidos</h2>
          
          {account?.orders && account.orders.length > 0 ? (
            <div style={{ display: "flex", flexDirection: "column", gap: "16px" }}>
              {account.orders.map((order) => (
                <div key={order.id} style={{ display: "flex", justifyContent: "space-between", alignItems: "center", background: "var(--bg-card)", padding: "20px", borderRadius: "var(--radius-lg)", border: "1px solid var(--border-color)" }}>
                  <div>
                    <h3 style={{ fontSize: "1.1rem", marginBottom: "8px" }}>Pedido #{order.number}</h3>
                    <p style={{ color: "var(--text-secondary)", fontSize: "0.9rem", marginBottom: "4px" }}>
                      Status: <strong style={{ color: "var(--text-primary)" }}>{getStatusLabel(order.status)}</strong>
                    </p>
                    <p style={{ color: "var(--text-secondary)", fontSize: "0.9rem" }}>
                      {order.itemsCount} {order.itemsCount === 1 ? 'item' : 'itens'} | {formatPrice(order.totalAmount)}
                    </p>
                  </div>
                  <div>
                    {order.status === "WaitingForPayment" || order.status === "Created" ? (
                      <button 
                        onClick={() => router.push(`/payment/${order.id}`)}
                        className="btn btn-primary"
                      >
                        💳 Pagar
                      </button>
                    ) : (
                      <span style={{ padding: "8px 16px", background: "var(--bg-secondary)", borderRadius: "var(--radius-md)", fontSize: "0.9rem", color: "var(--text-secondary)" }}>
                        {getStatusLabel(order.status)}
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p style={{ color: "var(--text-secondary)" }}>Você ainda não possui pedidos.</p>
          )}
        </div>

      </div>
    </div>
  );
}
