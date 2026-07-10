"use client";

import { CartItem as CartItemType } from "@/types";
import { useCart } from "./CartProvider";

interface CartItemProps {
  item: CartItemType;
}

export default function CartItemRow({ item }: CartItemProps) {
  const { updateQuantity, removeItem } = useCart();

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(price);
  };

  const subtotal = item.product.unitPrice * item.quantity;

  return (
    <div className="cart-item">
      <div className="cart-item-image">
        {item.product.imageUrl ? (
          <img src={item.product.imageUrl} alt={item.product.name} />
        ) : (
          <span style={{ fontSize: "1.5rem", opacity: 0.3 }}>📦</span>
        )}
      </div>
      <div className="cart-item-info">
        <div className="cart-item-name">{item.product.name}</div>
        <div className="cart-item-price">
          {formatPrice(item.product.unitPrice)} cada
        </div>
        <div className="cart-item-controls">
          <button
            className="cart-item-qty-btn"
            onClick={() => updateQuantity(item.product.id, item.quantity - 1)}
          >
            −
          </button>
          <span className="cart-item-qty">{item.quantity}</span>
          <button
            className="cart-item-qty-btn"
            onClick={() => updateQuantity(item.product.id, item.quantity + 1)}
          >
            +
          </button>
        </div>
      </div>
      <div style={{ display: "flex", flexDirection: "column", alignItems: "flex-end", justifyContent: "space-between" }}>
        <button
          className="cart-item-remove"
          onClick={() => removeItem(item.product.id)}
          title="Remover"
        >
          ✕
        </button>
        <span className="cart-item-subtotal">{formatPrice(subtotal)}</span>
      </div>
    </div>
  );
}
