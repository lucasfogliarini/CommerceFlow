"use client";

import { useCallback, useEffect, useState } from "react";
import { useKeycloak } from "@/components/KeycloakProvider";
import { approvePayment, getCustomerOrders } from "@/lib/api";
import { OrderSummary } from "@/types";

export default function OrdersPage() {
  const { keycloak, authenticated, initialized, error: authError, login } = useKeycloak();
  const [orders, setOrders] = useState<OrderSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState("");
  const [expandedOrderId, setExpandedOrderId] = useState<string | null>(null);
  const [payingOrderNumber, setPayingOrderNumber] = useState<string | null>(null);

  useEffect(() => {
    if (initialized && !authenticated && !authError) {
      void login();
    }

  }, [authenticated, authError, initialized, login]);

  const loadOrders = useCallback(async (isRefresh = false) => {
    if (!keycloak?.token) {
      return;
    }

    if (isRefresh) {
      setRefreshing(true);
    } else {
      setLoading(true);
    }

    setError("");

    try {
      setOrders(await getCustomerOrders(keycloak.token));
    } catch (reason) {
      setError(reason instanceof Error ? reason.message || "Erro ao carregar os pedidos." : "Erro ao carregar os pedidos.");
    } finally {
      if (isRefresh) {
        setRefreshing(false);
      } else {
        setLoading(false);
      }
    }
  }, [keycloak?.token]);

  const handlePayment = async (orderNumber: string) => {
    setPayingOrderNumber(orderNumber);
    setError("");
    try {
      const response = await approvePayment(orderNumber, crypto.randomUUID(), keycloak?.token);
      if (!response.ok) throw new Error(`Erro ao aprovar pagamento (${response.status})`);
      await loadOrders(true);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Erro ao processar o pagamento.");
    } finally {
      setPayingOrderNumber(null);
    }
  };

  useEffect(() => {
    if (authenticated && keycloak?.token) {
      void loadOrders();
    }
  }, [authenticated, keycloak?.token, loadOrders]);

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
        <div className="orders-header">
          <h1 className="account-title">Meus Pedidos</h1>
          <button className="btn btn-secondary" type="button" onClick={() => void loadOrders(true)} disabled={refreshing}>
            {refreshing ? "Atualizando..." : "Atualizar pedidos"}
          </button>
        </div>
        {error && <div className="account-error">⚠️ {error}</div>}

        <section className="orders-list" aria-label="Lista de pedidos">
          {latestOrders.length ? latestOrders.map((order) => (
            <article className="order-summary-card" key={order.id}>
              <div>
                <p className="order-summary-number">
                  Pedido #{order.number}
                  <button
                    type="button"
                    className="order-id-copy"
                    aria-label="Copiar ID do pedido"
                    title="Copiar ID do pedido"
                    onClick={() => void navigator.clipboard.writeText(order.id)}
                  >
                    ⧉
                  </button>
                </p>
                <p>{order.items.length} {order.items.length === 1 ? "item" : "itens"}</p>
                <p>Comprado em {formatOrderDate(order.createdAt)}</p>
                <button className="order-items-toggle" type="button" onClick={() => setExpandedOrderId((current) => current === order.id ? null : order.id)} aria-expanded={expandedOrderId === order.id}>
                  {expandedOrderId === order.id ? "Ocultar itens" : "Ver itens"}
                </button>
              </div>
              <div className="order-summary-action">
                <span className={`order-status order-status-${order.status.toLowerCase()}`}>{getStatusLabel(order.status)}</span>
                <strong>{formatPrice(order.totalAmount)}</strong>
                {(order.status === "Created" || order.status === "WaitingForPayment") && (
                  <button className="btn btn-primary" disabled={payingOrderNumber === order.number} onClick={() => void handlePayment(order.number)}>{payingOrderNumber === order.number ? "Pagando..." : "Pagar"}</button>
                )}
              </div>
              {expandedOrderId === order.id && (
                <div className="order-items-details">
                  {order.items.map((item) => (
                    <div key={`${order.id}-${item.productName}`} className="order-item-detail">
                      <span>{item.productName}</span>
                      <span>{item.quantity} × {formatPrice(item.unitPrice)}</span>
                      <strong>{formatPrice(item.quantity * item.unitPrice)}</strong>
                    </div>
                  ))}
                </div>
              )}
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
