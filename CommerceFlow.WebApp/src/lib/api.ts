import { Product, ODataResponse, CreateOrderRequest } from "@/types";

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

export async function createOrder(order: CreateOrderRequest): Promise<Response> {
  const res = await fetch(`${API_BASE}/orders`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(order),
  });

  return res;
}
