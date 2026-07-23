import { NextResponse } from "next/server";

export async function GET() {
  const shipmentsApiUrl = process.env.SHIPMENTS_API_URL;
  const keycloakUrl = process.env.KEYCLOAK_URL;

  if (!shipmentsApiUrl || !keycloakUrl) {
    return NextResponse.json(
      { error: "Missing SHIPMENTS_API_URL or KEYCLOAK_URL" },
      { status: 500 }
    );
  }

  return NextResponse.json({
    notificationHubUrl: `${shipmentsApiUrl}/hubs/notifications`,
    keycloakUrl,
  });
}
