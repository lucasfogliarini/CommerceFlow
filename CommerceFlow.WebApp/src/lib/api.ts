import { Address, Product, ODataResponse, CreateOrderRequest } from "@/types";

const API_BASE = "/api";

export async function fetchProducts(): Promise<Product[]> {
  const res = await fetch(`${API_BASE}/products`, {
    cache: "no-store",
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch products: ${res.status}`);
  }

  const data: ODataResponse<Product> = await res.json();
  return data.value ?? data;
}

export async function createOrder(order: CreateOrderRequest, token?: string): Promise<Response> {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}/orders`, {
    method: "POST",
    headers,
    body: JSON.stringify(order),
  });

  return res;
}

export async function approvePayment(orderId: string, paymentReference: string, token?: string): Promise<Response> {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}/orders/${orderId}/approve-payment`, {
    method: "PUT",
    headers,
    body: JSON.stringify({ paymentReference }),
  });

  return res;
}

export async function getMyAccount(token?: string) {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}/account/me`, {
    method: "GET",
    headers,
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch account: ${res.status}`);
  }

  return await res.json();
}

export async function getAddresses(token?: string): Promise<Address[]> {
  const res = await fetch(`${API_BASE}/addresses`, { headers: token ? { Authorization: `Bearer ${token}` } : {} });
  if (!res.ok) throw new Error(`Failed to fetch addresses: ${res.status}`);
  return res.json();
}

export async function updateAddress(address: Address, token?: string) {
  if (!address.id) throw new Error("Address ID is required");
  const res = await fetch(`${API_BASE}/addresses/${address.id}`, { method: "PUT", headers: { "Content-Type": "application/json", ...(token ? { Authorization: `Bearer ${token}` } : {}) }, body: JSON.stringify(address) });
  if (!res.ok) throw new Error(`Failed to update address: ${res.status}`);
}

export async function removeAddress(addressId: string, token?: string) {
  const res = await fetch(`${API_BASE}/addresses/${addressId}`, { method: "DELETE", headers: token ? { Authorization: `Bearer ${token}` } : {} });
  if (!res.ok) throw new Error(`Failed to remove address: ${res.status}`);
}

export async function createAddress(address: Address, token?: string): Promise<Address> {
  const res = await fetch(`${API_BASE}/addresses`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...(token ? { Authorization: `Bearer ${token}` } : {}) },
    body: JSON.stringify(address),
  });
  if (!res.ok) throw new Error(`Failed to create address: ${res.status}`);
  return res.json();
}
