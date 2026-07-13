"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useKeycloak } from "@/components/KeycloakProvider";
import { getMyAccount } from "@/lib/api";
import { AccountResponse } from "@/types";

export default function AccountPage() {
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

  if (!initialized || !authenticated) {
    return (
      <div className="checkout-page" style={{ padding: "40px", textAlign: "center" }}>
        {authError ? "Não foi possível autenticar." : "Autenticando..."}
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
    <div className="account-page">
      <div className="account-container container">
        <p className="account-breadcrumb">Minha conta</p>
        <h1 className="account-title">Minha Conta</h1>

        {error && <div className="account-error">⚠️ {error}</div>}

        <section className="account-profile-card" aria-labelledby="account-profile-title">
          <div className="account-profile-avatar" aria-hidden="true">
            {(account?.customer?.name || account?.email || "C").charAt(0).toUpperCase()}
          </div>
          <div>
            <p className="account-breadcrumb">Perfil</p>
            <h2 id="account-profile-title">{account?.customer?.name || "Sua conta CommerceFlow"}</h2>
            <p>{account?.customer?.email || account?.email}</p>
          </div>
        </section>

        <section className="account-details-card" aria-labelledby="account-details-title">
          <h2 id="account-details-title">Dados da conta</h2>
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
        </section>

        <section className="account-feature-grid" aria-label="Recursos da conta">
          <Link href="/orders"><span aria-hidden="true">▤</span><strong>Pedidos</strong><small>Acompanhe e consulte seus pedidos.</small></Link>
          <a id="payments" href="#payments"><span aria-hidden="true">▣</span><strong>Pagamentos</strong><small>Formas de pagamento e transações.</small></a>
          <a id="refunds" href="#refunds"><span aria-hidden="true">↺</span><strong>Reembolsos</strong><small>Acompanhe seus reembolsos.</small></a>
          <a id="wishlist" href="#wishlist"><span aria-hidden="true">♡</span><strong>Lista de desejos</strong><small>Produtos salvos para depois.</small></a>
        </section>

      </div>
    </div>
  );
}
