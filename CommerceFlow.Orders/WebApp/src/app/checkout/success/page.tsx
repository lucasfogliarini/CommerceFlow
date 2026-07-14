import Link from "next/link";

export default function SuccessPage() {
  return (
    <div className="success-page">
      <div className="success-card">
        <div className="success-icon">✓</div>
        <h1>
          Pedido <span className="text-gradient">Confirmado!</span>
        </h1>
        <p>
          Seu pedido foi criado com sucesso e está sendo processado.
          Você receberá atualizações sobre o status da entrega.
        </p>
        <div style={{ display: "flex", gap: "12px", justifyContent: "center", flexWrap: "wrap" }}>
          <Link href="/products" className="btn btn-primary">
            🛍️ Continuar Comprando
          </Link>
          <Link href="/" className="btn btn-secondary">
            ← Voltar ao Início
          </Link>
        </div>
      </div>
    </div>
  );
}
