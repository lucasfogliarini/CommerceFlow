"use client";

import { FormEvent, useEffect, useState } from "react";
import { useKeycloak } from "@/components/KeycloakProvider";
import { createAddress, getAddresses, removeAddress, updateAddress } from "@/lib/api";
import { Address } from "@/types";

const emptyAddress: Address = {
  street: "",
  number: "",
  city: "",
  state: "",
  zipCode: "",
  country: "Brasil",
};

export default function AddressesPage() {
  const { keycloak, authenticated, initialized, error: authError, login } = useKeycloak();
  const [addresses, setAddresses] = useState<Address[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
  const [draft, setDraft] = useState<Address>(emptyAddress);
  const [formOpen, setFormOpen] = useState(false);

  useEffect(() => {
    if (initialized && !authenticated && !authError) {
      void login();
    }
  }, [authenticated, authError, initialized, login]);

  useEffect(() => {
    if (authenticated && keycloak?.token) {
      getAddresses(keycloak.token)
        .then(setAddresses)
        .catch((reason: Error) => setError(reason.message || "Erro ao carregar os endereços."))
        .finally(() => setLoading(false));
    }
  }, [authenticated, keycloak]);

  function openAddForm() {
    setEditingIndex(null);
    setDraft(emptyAddress);
    setFormOpen(true);
  }

  function openEditForm(index: number) {
    setEditingIndex(index);
    setDraft(addresses[index]);
    setFormOpen(true);
  }

  async function saveAddress(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    try {
      if (editingIndex === null) {
        await createAddress(draft, keycloak?.token);
      } else {
        await updateAddress(draft, keycloak?.token);
      }
      setAddresses(await getAddresses(keycloak?.token));
      setEditingIndex(null);
      setDraft(emptyAddress);
      setFormOpen(false);
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Erro ao salvar o endereço.");
    }
  }

  async function deleteAddress(index: number) {
    if (!window.confirm("Este endereço será excluído permanentemente e não poderá ser recuperado. Deseja continuar?")) {
      return;
    }

    try {
      const address = addresses[index];
      if (!address.id) throw new Error("Endereço inválido.");
      await removeAddress(address.id, keycloak?.token);
      setAddresses(await getAddresses(keycloak?.token));
    } catch (reason) {
      setError(reason instanceof Error ? reason.message : "Erro ao excluir o endereço.");
    }
  }

  if (!initialized || !authenticated) {
    return <div className="checkout-page account-status-message">{authError ? "Não foi possível autenticar." : "Autenticando..."}</div>;
  }

  if (loading) {
    return <div className="checkout-page account-status-message"><span className="spinner" /> Carregando endereços...</div>;
  }

  return (
    <div className="account-page">
      <div className="account-container container">
        <p className="account-breadcrumb">Minha conta / Endereços</p>
        <h1 className="account-title">Seus endereços</h1>
        {error && <div className="account-error">⚠️ {error}</div>}

        <div className="address-grid">
          <button className="address-add-card" type="button" onClick={openAddForm}>
            <span aria-hidden="true">+</span>
            Adicionar endereço
          </button>
          {addresses.map((address, index) => (
            <article className="address-card" key={`${address.street}-${address.number}-${index}`}>
              {index === 0 && <span className="address-default">Endereço padrão</span>}
               <h2>{keycloak?.tokenParsed?.name || keycloak?.tokenParsed?.preferred_username || "Meu endereço"}</h2>
              <address>
                {address.street}, {address.number}<br />
                {address.city}, {address.state} {address.zipCode}<br />
                {address.country}
              </address>
              <div className="address-actions">
                <button type="button" onClick={() => openEditForm(index)}>Alterar</button>
                <button type="button" onClick={() => deleteAddress(index)}>Excluir</button>
              </div>
            </article>
          ))}
        </div>

        {formOpen && (
          <form className="address-form" onSubmit={saveAddress}>
            <h2>{editingIndex === null ? "Adicionar endereço" : "Alterar endereço"}</h2>
            <div className="form-grid">
              <label className="form-group span-2">Rua<input className="form-input" value={draft.street} onChange={(event) => setDraft({ ...draft, street: event.target.value })} required /></label>
              <label className="form-group">Número<input className="form-input" value={draft.number} onChange={(event) => setDraft({ ...draft, number: event.target.value })} required /></label>
              <label className="form-group">CEP<input className="form-input" value={draft.zipCode} onChange={(event) => setDraft({ ...draft, zipCode: event.target.value })} required /></label>
              <label className="form-group">Cidade<input className="form-input" value={draft.city} onChange={(event) => setDraft({ ...draft, city: event.target.value })} required /></label>
              <label className="form-group">Estado<input className="form-input" value={draft.state} onChange={(event) => setDraft({ ...draft, state: event.target.value })} required /></label>
              <label className="form-group span-2">País<input className="form-input" value={draft.country} onChange={(event) => setDraft({ ...draft, country: event.target.value })} required /></label>
            </div>
            <div className="address-form-actions"><button className="btn btn-primary" type="submit">Salvar endereço</button><button className="btn" type="button" onClick={() => { setEditingIndex(null); setDraft(emptyAddress); setFormOpen(false); }}>Cancelar</button></div>
          </form>
        )}
      </div>
    </div>
  );
}
