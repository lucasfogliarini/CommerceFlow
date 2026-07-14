import type { Metadata } from "next";
import "./globals.css";
import { CartProvider } from "@/components/CartProvider";
import { KeycloakProvider } from "@/components/KeycloakProvider";
import Navbar from "@/components/Navbar";
import Footer from "@/components/Footer";

export const metadata: Metadata = {
  title: "CommerceFlow — Loja Online",
  description:
    "Descubra produtos incríveis com a melhor experiência de compra. CommerceFlow — sua loja online premium.",
  keywords: ["ecommerce", "loja online", "compras", "produtos"],
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR">
      <body>
        <KeycloakProvider>
          <CartProvider>
            <Navbar />
            <main>{children}</main>
            <Footer />
          </CartProvider>
        </KeycloakProvider>
      </body>
    </html>
  );
}
