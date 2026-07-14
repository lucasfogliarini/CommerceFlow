import type { Shipment } from "@/types/shipment";

const baseUrl = "/api/shipments";

function headers(token?: string, json = false) {
  return { ...(json ? { "Content-Type": "application/json" } : {}), ...(token ? { Authorization: `Bearer ${token}` } : {}) };
}

async function request(path: string, options: RequestInit, token?: string) {
  const response = await fetch(`${baseUrl}${path}`, { ...options, headers: headers(token, options.body !== undefined) });
  if (!response.ok) throw new Error((await response.text()) || "Shipment action failed");
}

export async function getShipments(token?: string): Promise<Shipment[]> {
  const response = await fetch(baseUrl, { cache: "no-store", headers: headers(token) });
  if (response.status === 404) return [];
  if (!response.ok) throw new Error("Unable to load shipments");
  return response.json();
}

export function completePacking(id: string, token?: string) {
  return request(`/${id}/complete-packing`, { method: "PUT" }, token);
}

export function registerTrackingEvent(id: string, description: string, location: string, token?: string) {
  return request(`/${id}/tracking`, { method: "POST", body: JSON.stringify({ description, location }) }, token);
}

export function deliverShipment(id: string, token?: string) {
  return request(`/${id}/deliver`, { method: "PUT" }, token);
}
