export type ShipmentStatus =
  | "Created"
  | "CarrierAssigned"
  | "Packed"
  | "Dispatched"
  | "Delivered"
  | "Cancelled";

export interface Shipment {
  id: string;
  orderNumber: string;
  createdAt: string;
  status: ShipmentStatus;
  address?: {
    street?: string;
    number?: string;
    complement?: string;
    neighborhood?: string;
    city?: string;
    state?: string;
    zipCode?: string;
  };
  carrier?: { name?: string };
  tracking?: {
    trackingCode?: string;
    events?: Array<{ occurredAt: string; description: string; location: string }>;
  };
}
