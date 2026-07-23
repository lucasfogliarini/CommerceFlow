"use client";

import { type FormEvent, useEffect, useState } from "react";
import { completePacking, deliverShipment, getShipments, registerTrackingEvent } from "@/lib/shipments";
import { useKeycloak } from "@/components/KeycloakProvider";
import type { Shipment } from "@/types/shipment";

const statusLabel = (status: string) => status.replace(/([A-Z])/g, " $1").trim();
const eventDateLabel = (occurredAt: string) => new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(new Date(occurredAt));

export default function ShipmentsPage() {
  const { keycloak, authenticated, initialized, login } = useKeycloak();
  const [shipments, setShipments] = useState<Shipment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [busyId, setBusyId] = useState<string>();
  const [trackingShipment, setTrackingShipment] = useState<Shipment>();
  const [expandedTrackingIds, setExpandedTrackingIds] = useState<Set<string>>(new Set());

  async function loadShipments() {
    setLoading(true); setError("");
    try { setShipments(await getShipments(keycloak?.token)); }
    catch (reason) { setError(reason instanceof Error ? reason.message : "Unable to load shipments"); }
    finally { setLoading(false); }
  }

  useEffect(() => {
    if (!authenticated) return;

    let active = true;
    getShipments(keycloak?.token)
      .then((data) => { if (active) setShipments(data); })
      .catch((reason: unknown) => { if (active) setError(reason instanceof Error ? reason.message : "Unable to load shipments"); })
      .finally(() => { if (active) setLoading(false); });

    return () => { active = false; };
  }, [authenticated, keycloak]);

  async function runAction(shipment: Shipment, action: "packing" | "deliver") {
    setBusyId(shipment.id); setError("");
    try {
      if (action === "packing") await completePacking(shipment.id, keycloak?.token);
      else await deliverShipment(shipment.id, keycloak?.token);
      await loadShipments();
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Shipment action failed"); }
    finally { setBusyId(undefined); }
  }

  async function submitTracking(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!trackingShipment) return;
    const data = new FormData(event.currentTarget);
    setBusyId(trackingShipment.id); setError("");
    try {
      await registerTrackingEvent(trackingShipment.id, String(data.get("description")), String(data.get("location")), keycloak?.token);
      setTrackingShipment(undefined); await loadShipments();
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Unable to register tracking event"); }
    finally { setBusyId(undefined); }
  }

  if (!initialized || !authenticated) return <div className="dashboard simple-page"><h1>Shipment operations</h1><p>Sign in with your CommerceFlow account to manage shipments.</p><button className="dispatch-button" disabled={!initialized} onClick={() => void login()}>{initialized ? "Sign in" : "Connecting..."}</button></div>;

  return <div className="dashboard simple-page">
    <section className="page-heading"><div><p className="eyebrow">Shipment operations</p><h1>Shipments</h1><p>Manage packing, tracking events, and deliveries.</p></div><button className="refresh-button" type="button" onClick={() => void loadShipments()}>Refresh</button></section>
    {error && <p className="message error">{error}</p>}
    {loading ? <p className="message">Loading shipments...</p> : shipments.length === 0 ? <p className="message">No shipments found.</p> : <section className="panel shipments-table"><div className="shipment-table-header"><span>Shipment</span><span>Destination</span><span>Carrier</span><span>Status</span><span>Actions</span></div>{shipments.map((shipment) => { const trackingExpanded = expandedTrackingIds.has(shipment.id); const eventCount = shipment.tracking?.events?.length ?? 0; return <article className="shipment-table-row" key={shipment.id}><div><strong>{shipment.id}</strong><small>Order {shipment.orderNumber}</small>{shipment.tracking?.trackingCode && <small>Tracking {shipment.tracking.trackingCode}</small>}</div><div>{shipment.address?.city ?? "—"}{shipment.address?.state ? `, ${shipment.address.state}` : ""}</div><div>{shipment.carrier?.name ?? "Not assigned"}</div><div><span className={`status ${shipment.status}`}>{statusLabel(shipment.status)}</span></div><div className="shipment-actions">{shipment.status === "CarrierAssigned" && <button disabled={busyId === shipment.id} onClick={() => void runAction(shipment, "packing")}>Complete packing</button>}{shipment.status === "Dispatched" && <><button disabled={busyId === shipment.id} onClick={() => setTrackingShipment(shipment)}>Register tracking</button><button disabled={busyId === shipment.id} className="deliver" onClick={() => void runAction(shipment, "deliver")}>Deliver</button></>}</div><div className="tracking-events"><button type="button" className="tracking-events-toggle" aria-expanded={trackingExpanded} aria-controls={`tracking-events-${shipment.id}`} onClick={() => setExpandedTrackingIds((ids) => { const nextIds = new Set(ids); if (nextIds.has(shipment.id)) nextIds.delete(shipment.id); else nextIds.add(shipment.id); return nextIds; })}><span>Tracking events ({eventCount})</span><span aria-hidden="true">{trackingExpanded ? "−" : "+"}</span></button>{trackingExpanded && <div id={`tracking-events-${shipment.id}`}>{eventCount ? <ol>{shipment.tracking?.events?.map((trackingEvent) => <li key={`${trackingEvent.occurredAt}-${trackingEvent.description}`}><time dateTime={trackingEvent.occurredAt}>{eventDateLabel(trackingEvent.occurredAt)}</time><span><b>{trackingEvent.description}</b><small>{trackingEvent.location}</small></span></li>)}</ol> : <p>No tracking events registered.</p>}</div>}</div></article>; })}</section>}
    {trackingShipment && <div className="modal-backdrop" role="presentation"><form className="tracking-modal" onSubmit={submitTracking}><h2>Register tracking event</h2><p>{trackingShipment.id}</p><label>Description<select name="description" required defaultValue=""><option value="" disabled>Select a description</option><option>Pedido coletado pela transportadora</option><option>Em trânsito para o centro de distribuição</option><option>Chegou ao centro de distribuição</option><option>Saiu para entrega</option><option>Tentativa de entrega realizada</option><option>Entrega concluída</option></select></label><label>Location<select name="location" required defaultValue=""><option value="" disabled>Select a location</option><option>Porto Alegre</option><option>Canoas</option><option>Gramado</option><option>Xangri-Lá</option><option>Cachoeira do Sul</option></select></label><div><button type="button" onClick={() => setTrackingShipment(undefined)}>Cancel</button><button className="dispatch-button" disabled={busyId === trackingShipment.id} type="submit">Save event</button></div></form></div>}
  </div>;
}
