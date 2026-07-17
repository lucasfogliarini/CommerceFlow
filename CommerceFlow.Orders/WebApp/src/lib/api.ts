import { Address, Product, ODataResponse, CreateOrderRequest } from "@/types";

const API_BASE = "/api";
export const PRODUCTS_PER_PAGE = 10;

export interface ProductFilters {
  search?: string;
  minimumPrice?: number;
  maximumPrice?: number;
  priceOrder?: "asc" | "desc";
}

export interface ProductPage {
  products: Product[];
  total: number;
}

export async function fetchProducts(filters: ProductFilters = {}): Promise<Product[]> {
  const page = await fetchProductPage(filters);
  return page.products;
}

export async function fetchProductPage(filters: ProductFilters = {}, pageNumber = 1): Promise<ProductPage> {
  const clauses: string[] = [];
  const search = filters.search?.trim();

  if (search) {
    const escapedSearch = search.replaceAll("'", "''").toLowerCase();
    clauses.push(`contains(tolower(Name),'${escapedSearch}') or contains(tolower(Description),'${escapedSearch}')`);
  }

  if (filters.minimumPrice !== undefined) {
    clauses.push(`UnitPrice ge ${filters.minimumPrice}`);
  }

  if (filters.maximumPrice !== undefined) {
    clauses.push(`UnitPrice le ${filters.maximumPrice}`);
  }

  const query = new URLSearchParams();
  if (clauses.length > 0) {
    query.set("$filter", clauses.join(" and "));
  }
  if (filters.priceOrder) {
    query.set("$orderby", `UnitPrice ${filters.priceOrder}`);
  }
  query.set("$top", PRODUCTS_PER_PAGE.toString());
  query.set("$skip", ((pageNumber - 1) * PRODUCTS_PER_PAGE).toString());
  query.set("$count", "true");

  const queryString = query.toString();
  const res = await fetch(`${API_BASE}/products${queryString ? `?${queryString}` : ""}`, {
    cache: "no-store",
  });

  if (!res.ok) {
    throw new Error(`Failed to fetch products: ${res.status}`);
  }

  const data: ODataResponse<Product> | Product[] = await res.json();
  const products = Array.isArray(data) ? data : data.value;

  return {
    products,
    total: Array.isArray(data) ? products.length : data["@odata.count"] ?? products.length,
  };
}

export async function getCustomerOrders(token?: string) {
  const res = await fetch(`${API_BASE}/orders`, {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
  });

  if (!res.ok) throw new Error(`Failed to fetch orders: ${res.status}`);
  return res.json();
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

export async function approvePayment(orderNumber: string, paymentReference: string, token?: string): Promise<Response> {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}/orders/${encodeURIComponent(orderNumber)}/approve-payment`, {
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
