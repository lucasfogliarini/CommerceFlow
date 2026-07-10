"use client";

import Link from "next/link";
import { useCart } from "@/components/CartProvider";
import CartItemRow from "@/components/CartItem";

export default function CartPage() {
  const { items, totalPrice, clearCart } = useCart();

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(price);
  };

  if (items.length === 0) {
    return (
      <div className="cart-page">
        <div className="container">
          <div className="cart-empty">
            <div className="cart-empty-icon">🛒</div>
            <h2>Seu carrinho está vazio</h2>
            <p>Adicione produtos para começar suas compras!</p>
            <Link href="/products" className="btn btn-primary">
              🛍️ Ver Produtos
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <div className="container">
        <div className="cart-header">
          <h1 style={{ fontSize: "1.8rem", fontWeight: 800 }}>
            🛒 Meu <span className="text-gradient">Carrinho</span>
          </h1>
        </div>

        <div className="cart-layout">
          <div>
            {items.map((item, index) => (
              <div
                key={item.product.id}
                style={{ animationDelay: `${index * 0.05}s` }}
              >
                <CartItemRow item={item} />
              </div>
            ))}

            <div style={{ marginTop: "16px" }}>
              <button
                className="btn btn-danger btn-sm"
                onClick={clearCart}
              >
                🗑️ Limpar Carrinho
              </button>
            </div>
          </div>

          <div className="cart-summary">
            <h3>Resumo do Pedido</h3>

            <div className="cart-summary-row">
              <span>
                Itens ({items.reduce((s, i) => s + i.quantity, 0)})
              </span>
              <span>{formatPrice(totalPrice)}</span>
            </div>

            <div className="cart-summary-row">
              <span>Frete</span>
              <span style={{ color: "var(--accent-emerald)" }}>Grátis</span>
            </div>

            <div className="cart-summary-total">
              <span>Total</span>
              <span>{formatPrice(totalPrice)}</span>
            </div>

            <Link href="/checkout" className="btn btn-primary btn-lg">
              Finalizar Pedido →
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
