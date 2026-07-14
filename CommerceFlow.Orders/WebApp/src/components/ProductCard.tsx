"use client";

import { Product } from "@/types";
import { useCart } from "./CartProvider";
import { useState } from "react";
import { useRouter } from "next/navigation";

interface ProductCardProps {
  product: Product;
}

export default function ProductCard({ product }: ProductCardProps) {
  const { addItem, items } = useCart();
  const router = useRouter();
  const [justAdded, setJustAdded] = useState(false);

  const isInCart = items.some((item) => item.product.id === product.id);

  const handleAdd = () => {
    addItem(product);
    setJustAdded(true);
    setTimeout(() => setJustAdded(false), 1200);
  };

  const handleBuyNow = () => {
    addItem(product);
    router.push("/checkout");
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(price);
  };

  return (
    <div className="product-card">
      <div className="product-card-image">
        <img src={`/images/${product.slug}.svg`} alt={product.name} />
      </div>
      <div className="product-card-body">
        {product.slug && (
          <div className="product-card-category">{product.slug}</div>
        )}
        <h3 className="product-card-name">{product.name}</h3>
        {product.description && (
          <p className="product-card-description">{product.description}</p>
        )}
        <div className="product-card-footer">
          <span className="product-card-price">
            {formatPrice(product.unitPrice)}
          </span>
          <div className="product-card-actions">
            <button
              className={`product-card-add-btn ${justAdded ? "added" : ""}`}
              onClick={handleAdd}
            >
              {justAdded ? "✓ Adicionado" : isInCart ? "+ Mais 1" : "🛒 Adicionar"}
            </button>
            <button className="product-card-buy-now-btn" onClick={handleBuyNow}>
              Comprar em 1 clique
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
