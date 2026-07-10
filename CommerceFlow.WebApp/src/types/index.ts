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
  customerId: string;
  shippingAddress: Address;
  items: CreateOrderItem[];
}

export interface CartItem {
  product: Product;
  quantity: number;
}
