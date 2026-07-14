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
            {(account?.customer?.name || keycloak?.tokenParsed?.name || keycloak?.tokenParsed?.preferred_username || account?.email || "C").charAt(0).toUpperCase()}
          </div>
          <div>
            <p className="account-breadcrumb">Perfil e dados da conta</p>
            <h2 id="account-profile-title">{account?.customer?.name || keycloak?.tokenParsed?.name || keycloak?.tokenParsed?.preferred_username || "Minha Conta"}</h2>
            <p><strong>E-mail:</strong> {account?.customer?.email || account?.email}</p>
          </div>
        </section>

        <section className="account-feature-grid" aria-label="Recursos da conta">
          <Link href="/orders"><span aria-hidden="true">▤</span><strong>Pedidos</strong><small>Acompanhe e consulte seus pedidos.</small></Link>
          <Link href="/addresses"><span aria-hidden="true">⌂</span><strong>Endereços</strong><small>Gerencie seus endereços de entrega.</small></Link>
          <a id="payments" href="#payments"><span aria-hidden="true">▣</span><strong>Pagamentos</strong><small>Formas de pagamento e transações.</small></a>
          <a id="refunds" href="#refunds"><span aria-hidden="true">↺</span><strong>Reembolsos</strong><small>Acompanhe seus reembolsos.</small></a>
          <a id="wishlist" href="#wishlist"><span aria-hidden="true">♡</span><strong>Lista de desejos</strong><small>Produtos salvos para depois.</small></a>
        </section>

      </div>
    </div>
  );
}
