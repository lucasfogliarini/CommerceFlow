"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useKeycloak } from "@/components/KeycloakProvider";
import { getMyAccount } from "@/lib/api";
import { AccountResponse } from "@/types";

export default function OrdersPage() {
  const router = useRouter();
  const { keycloak, authenticated, initialized, error: authError, login } = useKeycloak();
  const [account, setAccount] = useState<AccountResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (initialized && !authenticated && !authError) {
      void login();
    }
  }, [authenticated, authError, initialized, login]);

  useEffect(() => {
    if (authenticated && keycloak?.token) {
      getMyAccount(keycloak.token)
        .then(setAccount)
        .catch((reason: Error) => setError(reason.message || "Erro ao carregar os pedidos."))
        .finally(() => setLoading(false));
    }
  }, [authenticated, keycloak]);

  if (!initialized || !authenticated) {
    return <div className="checkout-page account-status-message">{authError ? "Não foi possível autenticar." : "Autenticando..."}</div>;
  }

  if (loading) {
    return <div className="checkout-page account-status-message"><span className="spinner" /> Carregando pedidos...</div>;
  }

  return (
    <div className="account-page">
      <div className="account-container container">
        <p className="account-breadcrumb">Minha conta / Pedidos</p>
        <h1 className="account-title">Meus Pedidos</h1>
        {error && <div className="account-error">⚠️ {error}</div>}

        <section className="orders-list" aria-label="Lista de pedidos">
          {account?.orders.length ? account.orders.map((order) => (
            <article className="order-summary-card" key={order.id}>
              <div>
                <p className="order-summary-number">Pedido #{order.number}</p>
                <p>{order.itemsCount} {order.itemsCount === 1 ? "item" : "itens"}</p>
              </div>
              <div className="order-summary-action">
                <span>{getStatusLabel(order.status)}</span>
                <strong>{formatPrice(order.totalAmount)}</strong>
                {(order.status === "Created" || order.status === "WaitingForPayment") && (
                  <button className="btn btn-primary" onClick={() => router.push(`/payment/${order.id}`)}>Pagar</button>
                )}
              </div>
            </article>
          )) : <div className="account-empty-state"><h2>Você ainda não possui pedidos</h2><p>Seus pedidos aparecerão aqui após a compra.</p></div>}
        </section>
      </div>
    </div>
  );
}

function formatPrice(price: number) {
  return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(price);
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    Created: "Criado",
    WaitingForPayment: "Aguardando pagamento",
    PaymentApproved: "Pagamento aprovado",
    PaymentRejected: "Pagamento rejeitado",
    PaymentExpired: "Pagamento expirado",
    ReadyForShipment: "Pronto para envio",
    Dispatched: "Enviado",
    Delivered: "Entregue",
    Cancelled: "Cancelado",
  };
  return labels[status] || status;
}
