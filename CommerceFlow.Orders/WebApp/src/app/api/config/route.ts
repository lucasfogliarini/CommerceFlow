import { NextResponse } from "next/server";

export async function GET() {
  const ordersApiUrl = process.env.ORDERS_API_URL;
  const keycloakUrl = process.env.KEYCLOAK_URL;

  if (!ordersApiUrl || !keycloakUrl) {
    return NextResponse.json(
      { error: "Missing ORDERS_API_URL or KEYCLOAK_URL" },
      { status: 500 }
    );
  }

  return NextResponse.json({
    notificationHubUrl: `${ordersApiUrl}/hubs/notifications`,
    keycloakUrl,
  });
}
