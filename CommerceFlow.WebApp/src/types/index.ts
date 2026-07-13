export interface Product {
  id: string;
  slug: string;
  name: string;
  description: string;
  unitPrice: number;
  availableQuantity: number;
  imageUrl?: string;
}

export interface ODataResponse<T> {
  value: T[];
  "@odata.count"?: number;
  "@odata.nextLink"?: string;
}

export interface Address {
  id?: string;
  street: string;
  number: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface CreateOrderItem {
  quantity: number;
  productId: string;
}

export interface CreateOrderRequest {
  shippingAddress: Address;
  items: CreateOrderItem[];
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface Customer {
  id: string;
  email: string;
  name: string;
  address: Address;
}

export interface OrderSummary {
  id: string;
  number: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  items: OrderItemSummary[];
}

export interface OrderItemSummary {
  productName: string;
  unitPrice: number;
  quantity: number;
}

export interface AccountResponse {
  email: string;
  customer: Customer | null;
  orders: OrderSummary[];
}
