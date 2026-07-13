"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useKeycloak } from "@/components/KeycloakProvider";
import { getCustomerOrders } from "@/lib/api";
import { OrderSummary } from "@/types";

export default function OrdersPage() {
  const router = useRouter();
  const { keycloak, authenticated, initialized, error: authError, login } = useKeycloak();
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (initialized && !authenticated && !authError) {
      void login();
    }

  }, [authenticated, authError, initialized, login]);

  useEffect(() => {
    if (authenticated && keycloak?.token) {
      getCustomerOrders(keycloak.token)
        .then(setOrders)
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

  const latestOrders = [...orders].sort(
    (first, second) => new Date(second.createdAt).getTime() - new Date(first.createdAt).getTime()
  );

  return (
    <div className="account-page">
      <div className="account-container container">
        <p className="account-breadcrumb">Minha conta / Pedidos</p>
        <h1 className="account-title">Meus Pedidos</h1>
        {error && <div className="account-error">⚠️ {error}</div>}

        <section className="orders-list" aria-label="Lista de pedidos">
          {latestOrders.length ? latestOrders.map((order) => (
            <article className="order-summary-card" key={order.id}>
              <div>
                <p className="order-summary-number">Pedido #{order.number}</p>
                <p>{order.itemsCount} {order.itemsCount === 1 ? "item" : "itens"}</p>
                <p>Comprado em {formatOrderDate(order.createdAt)}</p>
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

function formatOrderDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR", { dateStyle: "short", timeStyle: "short" }).format(new Date(value));
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
